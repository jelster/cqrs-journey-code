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

namespace Conference.Web.Public
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;
    using System.Web;
    using Infrastructure;
    using Infrastructure.Azure;
    using Infrastructure.Azure.EventSourcing;
    using Infrastructure.Azure.Messaging;
    using Infrastructure.Database;
    using Infrastructure.EventSourcing;
    using Infrastructure.Messaging;
    using Infrastructure.Messaging.Handling;
    using Infrastructure.Serialization;
    using Infrastructure.Sql.Database;
    using Microsoft.Practices.Unity;
    using Microsoft.WindowsAzure;
    using Payments;
    using Payments.Database;
    using Payments.Handlers;
    using Registration;
    using Registration.Handlers;

    partial class MvcApplication
    {
        private List<IProcessor> processors;

        static partial void OnCreateContainer(UnityContainer container)
        {
            var serializer = new JsonTextSerializer();
            container.RegisterInstance<ITextSerializer>(serializer);
            var metadata = new StandardMetadataProvider();
            container.RegisterInstance<IMetadataProvider>(metadata);

            // command bus

            var settings = InfrastructureSettings.Read(HttpContext.Current.Server.MapPath(@"~\bin\Settings.xml"));
            new ServiceBusConfig(settings.ServiceBus).Initialize();
            var commandBus = new CommandBus(new TopicSender(settings.ServiceBus, "conference/commands"), metadata, serializer);

            var synchronousCommandBus = new SynchronousCommandBusDecorator(commandBus);

            container.RegisterInstance<ICommandBus>(synchronousCommandBus);
            container.RegisterInstance<ICommandHandlerRegistry>(synchronousCommandBus);

            // support for inline command processing

            container.RegisterType<ICommandHandler, OrderCommandHandler>("OrderCommandHandler");
            container.RegisterType<ICommandHandler, ThirdPartyProcessorPaymentCommandHandler>("ThirdPartyProcessorPaymentCommandHandler");
            container.RegisterType<ICommandHandler, SeatAssignmentsHandler>("SeatAssignmentsHandler");

            container.RegisterType<DbContext, PaymentsDbContext>("payments", new TransientLifetimeManager(), new InjectionConstructor("Payments"));
            container.RegisterType<IDataContext<ThirdPartyProcessorPayment>, SqlDataContext<ThirdPartyProcessorPayment>>(
                new TransientLifetimeManager(),
                new InjectionConstructor(new ResolvedParameter<Func<DbContext>>("payments"), typeof(IEventBus)));

            container.RegisterType<IPricingService, PricingService>(new ContainerControlledLifetimeManager());

            var topicSender = new TopicSender(settings.ServiceBus, "conference/events");
            container.RegisterInstance<IMessageSender>(topicSender);
            var eventBus = new EventBus(topicSender, metadata, serializer);

            container.RegisterInstance<IEventBus>(eventBus);

            var eventSourcingAccount = CloudStorageAccount.Parse(settings.EventSourcing.ConnectionString);
            var eventStore = new EventStore(eventSourcingAccount, settings.EventSourcing.TableName);

            container.RegisterInstance<IEventStore>(eventStore);
            container.RegisterInstance<IPendingEventsQueue>(eventStore);
            container.RegisterType<IEventStoreBusPublisher, EventStoreBusPublisher>(new ContainerControlledLifetimeManager());
            container.RegisterType(typeof(IEventSourcedRepository<>), typeof(AzureEventSourcedRepository<>), new ContainerControlledLifetimeManager());

            // to satisfy the IProcessor requirements.
            container.RegisterType<IProcessor, PublisherProcessorAdapter>("EventStoreBusPublisher", new ContainerControlledLifetimeManager());
        }

        partial void OnStart()
        {
            var commandHandlerRegistry = this.container.Resolve<ICommandHandlerRegistry>();

            foreach (var commandHandler in this.container.ResolveAll<ICommandHandler>())
            {
                commandHandlerRegistry.Register(commandHandler);
            }

            this.processors = this.container.ResolveAll<IProcessor>().ToList();
            this.processors.ForEach(p => p.Start());
        }

        partial void OnStop()
        {
            this.processors.ForEach(p => p.Stop());
        }

        // to satisfy the IProcessor requirements.
        private class PublisherProcessorAdapter : IProcessor
        {
            private IEventStoreBusPublisher publisher;
            private CancellationTokenSource tokenSource;

            public PublisherProcessorAdapter(IEventStoreBusPublisher publisher)
            {
                this.publisher = publisher;
                this.tokenSource = new CancellationTokenSource();
            }

            public void Start()
            {
                this.publisher.Start(this.tokenSource.Token);
            }

            public void Stop()
            {
                this.tokenSource.Cancel();
            }
        }
    }
}