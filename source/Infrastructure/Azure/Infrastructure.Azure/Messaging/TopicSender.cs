﻿// ==============================================================================================================
// Microsoft patterns & practices
// CQRS Journey project
// ==============================================================================================================
// ©2012 Microsoft. All rights reserved. Certain content used with permission from contributors
// http://cqrsjourney.github.com/contributors/members
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance 
// with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software distributed under the License is 
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and limitations under the License.
// ==============================================================================================================

namespace Infrastructure.Azure.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.ServiceBus;
    using Microsoft.Practices.TransientFaultHandling;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    /// <summary>
    /// Implements an asynchronous sender of messages to an Azure 
    /// service bus topic.
    /// </summary>
    public class TopicSender : IMessageSender
    {
        private readonly TokenProvider tokenProvider;
        private readonly Uri serviceUri;
        private readonly MessagingSettings settings;
        private readonly string topic;
        private readonly RetryPolicy retryPolicy;
        private readonly TopicClient topicClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="TopicSender"/> class, 
        /// automatically creating the given topic if it does not exist.
        /// </summary>
        public TopicSender(MessagingSettings settings, string topic)
        {
            this.settings = settings;
            this.topic = topic;

            this.tokenProvider = TokenProvider.CreateSharedSecretTokenProvider(settings.TokenIssuer, settings.TokenAccessKey);
            this.serviceUri = ServiceBusEnvironment.CreateServiceUri(settings.ServiceUriScheme, settings.ServiceNamespace, settings.ServicePath);

            try
            {
                new NamespaceManager(this.serviceUri, this.tokenProvider)
                    .CreateTopic(
                        new TopicDescription(topic)
                        {
                            RequiresDuplicateDetection = true,
                            DuplicateDetectionHistoryTimeWindow = TimeSpan.FromMinutes(30)
                        });
            }
            catch (MessagingEntityAlreadyExistsException)
            { }

            // TODO: This could be injected.
            var retryStrategy = new ExponentialBackoff(
                10,
                TimeSpan.FromMilliseconds(100),
                TimeSpan.FromSeconds(15),
                TimeSpan.FromSeconds(1));
            this.retryPolicy = new RetryPolicy<ServiceBusTransientErrorDetectionStrategy>(retryStrategy);

            var factory = MessagingFactory.Create(this.serviceUri, this.tokenProvider);
            this.topicClient = factory.CreateTopicClient(this.topic);
        }

        /// <summary>
        /// Asynchronously sends the specified message.
        /// </summary>
        public void SendAsync(BrokeredMessage message)
        {
            // TODO: what about retries? Watch-out for message reuse. Need to recreate it before retry.
            // Always send async.
            this.topicClient.BeginSend(message, ar =>
            {
                try
                {
                    this.topicClient.EndSend(ar);
                }
                finally
                {
                    message.Dispose();
                }
            }, null);
        }

        public void SendAsync(IEnumerable<BrokeredMessage> messages)
        {
            // TODO: batch/transactional sending?
            foreach (var message in messages)
            {
                this.SendAsync(message);
            }
        }

        public void Send(Func<BrokeredMessage> messageFactory)
        {
            var resetEvent = new ManualResetEvent(false);
            Exception exception = null;
            this.retryPolicy.ExecuteAction(
                ac =>
                    {
                        var message = messageFactory.Invoke();
                        this.topicClient.BeginSend(message, ac, message);
                    },
                ar =>
                    {
                        try
                        {
                            this.topicClient.EndSend(ar);
                        }
                        finally
                        {
                            using (ar.AsyncState as IDisposable) { }
                        }
                    },
                () => resetEvent.Set(),
                ex =>
                    {
                        exception = ex;
                        resetEvent.Set();
                    });

            resetEvent.WaitOne();
            if (exception != null)
            {
                throw exception;
            }
        }
    }
}
