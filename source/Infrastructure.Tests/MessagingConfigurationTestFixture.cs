using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Configuration;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Handling;
using Xunit;

namespace Infrastructure.Tests
{
    public class FakeCommand : ICommand
    {
        #region Implementation of ICommand

        public Guid Id { get; set; }

        public FakeCommand()
        {
            Id = Guid.NewGuid();
        }

        #endregion
    }
    public class OtherFakeCommand : ICommand
    {
        public Guid Id { get; set; }

    }
    public class FakeEventHandler : IEventHandler<FakeEvent>
    {
        public List<FakeEvent> ReceivedEvents = new List<FakeEvent>(); 
        public void Handle(FakeEvent @event)
        {
            ReceivedEvents.Add(@event);
        }
    }

    public class FakeCommandHandler : ICommandHandler<FakeCommand>
    {
        public List<FakeCommand> ReceivedCommands = new List<FakeCommand>();

        public void Handle(FakeCommand command)
        {
            ReceivedCommands.Add(command);
        }
    }

    public class FakeCompositeCommandHandler : ICommandHandler<FakeCommand>, ICommandHandler<OtherFakeCommand>
    {
        public List<ICommand> ReceivedCommands = new List<ICommand>(); 
        public void Handle(FakeCommand command) { ReceivedCommands.Add(command);}
        public void Handle(OtherFakeCommand command) { ReceivedCommands.Add(command); }
    }

    public class given_a_set_of_commands_and_events
    {
        private readonly MessagingConfiguration sut;

        public given_a_set_of_commands_and_events()
        {
            sut = MessagingConfiguration.Initialize(typeof (ICommand), typeof (ICommandHandler<>),
                                                    new[] {typeof (FakeCommand).Assembly}, "testCommands");

        }

        [Fact]
        public void when_initialized_named_config_should_not_be_null()
        {
            Assert.NotNull(sut);
            Assert.Equal("testCommands", sut.Name);
        }

        [Fact]
        public void when_configured_mappings_contains_handlers_and_message_types()
        {
            Assert.NotEmpty(sut.Handlers);
            Assert.NotEmpty(sut.Subject);

            Assert.Contains(typeof (FakeCommandHandler), sut.Handlers);
            Assert.Contains(typeof (FakeCommand), sut.Subject);
        }

        [Fact]
        public void when_object_factory_initialized_from_config_can_resolve_handler()
        {
            ObjectFactory.InitializeFromConfiguration(sut);

            var act = ObjectFactory.GetInstance<FakeCommandHandler>();

            Assert.NotNull(act);
            Assert.IsType<FakeCommandHandler>(act);
        }

        [Fact]
        public void when_multiple_configs_registered_returns_correct_config_by_name()
        {
            var otherConfig = MessagingConfiguration.Initialize(typeof (IEvent), typeof (IEventHandler<>),
                                                                new[] {typeof (FakeEvent).Assembly}, "testEvents");

            var actual = MessagingConfiguration.Current["testEvents"];
            Assert.NotNull(actual);
            Assert.Equal(otherConfig.Name, actual.Name);
            Assert.Contains(typeof(FakeEventHandler), actual.Handlers);
            Assert.True(otherConfig.Handlers.SequenceEqual(actual.Handlers));
            Assert.True(otherConfig.Subject.SequenceEqual(actual.Subject));
        }

        [Fact]
        public void when_handler_handles_multiple_types_gets_resolved_for_each()
        {
            ObjectFactory.InitializeFromConfiguration(sut);

            var a = ObjectFactory.GetAllInstances<ICommandHandler<FakeCommand>>();
            var b = ObjectFactory.GetAllInstances<ICommandHandler<OtherFakeCommand>>();

            Assert.NotEmpty(a);
            Assert.NotEmpty(b);

            Assert.Same(a.First().GetType(), b.First().GetType());
        }
    }
}
