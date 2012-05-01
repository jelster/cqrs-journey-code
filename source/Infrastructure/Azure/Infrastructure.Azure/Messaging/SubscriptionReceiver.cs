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
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    /// <summary>
    /// Implements an asynchronous receiver of messages from an Azure 
    /// service bus topic subscription.
    /// </summary>
    public class SubscriptionReceiver : IMessageReceiver, IDisposable
    {
        private readonly TokenProvider tokenProvider;
        private readonly Uri serviceUri;
        private readonly MessagingSettings settings;
        private CancellationTokenSource cancellationSource;
        private SubscriptionClient client;
        private string subscription;
        private object lockObject = new object();

        /// <summary>
        /// Event raised whenever a message is received. Consumer of 
        /// the event is responsible for disposing the message when 
        /// appropriate.
        /// </summary>
        public event EventHandler<BrokeredMessageEventArgs> MessageReceived = (sender, args) => { };

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionReceiver"/> class, 
        /// automatically creating the topic and subscription if they don't exist.
        /// </summary>
        public SubscriptionReceiver(MessagingSettings settings, string topic, string subscription)
        {
            this.settings = settings;
            this.subscription = subscription;

            this.tokenProvider = TokenProvider.CreateSharedSecretTokenProvider(settings.TokenIssuer, settings.TokenAccessKey);
            this.serviceUri = ServiceBusEnvironment.CreateServiceUri(settings.ServiceUriScheme, settings.ServiceNamespace, settings.ServicePath);

            var messagingFactory = MessagingFactory.Create(this.serviceUri, tokenProvider);
            this.client = messagingFactory.CreateSubscriptionClient(topic, subscription);

            var manager = new NamespaceManager(this.serviceUri, this.tokenProvider);

            try
            {
                manager.CreateTopic(
                    new TopicDescription(topic)
                    {
                        RequiresDuplicateDetection = true,
                        DuplicateDetectionHistoryTimeWindow = TimeSpan.FromMinutes(30)
                    });
            }
            catch (MessagingEntityAlreadyExistsException)
            { }

            try
            {
                manager.CreateSubscription(topic, subscription);
            }
            catch (MessagingEntityAlreadyExistsException)
            { }
        }

        /// <summary>
        /// Starts the listener.
        /// </summary>
        public void Start()
        {
            lock (this.lockObject)
            {
                this.cancellationSource = new CancellationTokenSource();
                Task.Factory.StartNew(() => this.ReceiveMessages(this.cancellationSource.Token), this.cancellationSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
            }
        }

        /// <summary>
        /// Stops the listener.
        /// </summary>
        public void Stop()
        {
            lock (this.lockObject)
            {
                using (this.cancellationSource)
                {
                    if (this.cancellationSource != null)
                    {
                        this.cancellationSource.Cancel();
                        this.cancellationSource = null;
                    }
                }
            }
        }


        /// <summary>
        /// Stops the listener if it was started previously.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.Stop();
        }

        ~SubscriptionReceiver()
        {
            Dispose(false);
        }

        /// <summary>
        /// Receives the messages in an endless loop.
        /// </summary>
        private void ReceiveMessages(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                BrokeredMessage message = null;

                try
                {
                    // NOTE: we don't long-poll more than a few seconds as 
                    // we're already on a background thread and we want to 
                    // allow other threads/processes/machines to potentially 
                    // receive messages too.
                    message = this.client.Receive(TimeSpan.FromSeconds(10));
                }
                catch (TimeoutException)
                {
                    // Server may be down.
                }

                if (message == null)
                {
                    Thread.Sleep(100);
                    continue;
                }

                if (!cancellationToken.IsCancellationRequested)
                    this.MessageReceived(this, new BrokeredMessageEventArgs(message));

            }
        }
    }
}