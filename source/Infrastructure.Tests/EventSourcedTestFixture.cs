using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using Infrastructure.EventSourcing;
using Infrastructure.Messaging;
using Moq;
using Xunit;


namespace Infrastructure.Tests
{
    public class FakeEventSource : EventSourced
    {
        public int numberOfFakeEventsRaised { get; private set; }
        public int numberOfBarEventsRaised { get; private set; }
    
        public FakeEventSource(Guid id) : base(id)
        {
            base.Handles<FakeEvent>(OnFakeEvent);
            base.Handles<BarEvent>(OnBarEvent);
        }

        public void IncrementCounter()
        {
            Update(new FakeEvent());
        }

        public void GoToTheBar()
        {
            Update(new BarEvent());
        }

        private void OnBarEvent(BarEvent obj)
        {
            numberOfBarEventsRaised += 1;
        }

        private void OnFakeEvent(FakeEvent obj)
        {
            numberOfFakeEventsRaised += 1;
        }
    }

    public class FakeEvent : VersionedEvent{}

    public class BarEvent : VersionedEvent{}

    public class given_an_event_source
    {
        private readonly FakeEventSource sut = new FakeEventSource(Guid.NewGuid());
        
        [Fact]
        public void when_blocking_method_called_on_events_doesnt_block()
        {
            sut.IncrementCounter();
            Assert.True(sut.Events.Count() == 1);
        }

        [Fact]
        public void when_subcriber_subscribes_gets_all_events_replayed()
        {
            var pub = sut.EventStream.Publish();

            for (int i = 0; i < 10; i++)
            {
                sut.IncrementCounter();
            }

            pub.Subscribe(x => Assert.True(x.SourceId == sut.Id));
            var sub = pub.Count().Subscribe(x => Assert.True(x == 10));
            pub.Connect();

            Assert.True(true);
            sub.Dispose();
        }

        [Fact]
        public void when_event_is_raised_observers_are_notified()
        {
            FakeEvent eFakeEvent = null;
            var initialVersion = sut.Version;
            var sub = sut.EventStream.OfType<FakeEvent>().SubscribeOn(Scheduler.CurrentThread);

            sut.IncrementCounter();
            var subHandle = sub.Subscribe(x => { eFakeEvent = x; }, x => Assert.False(true));

            Assert.NotNull(eFakeEvent);
            Assert.True(eFakeEvent.Version == initialVersion + 1);
            Assert.True(eFakeEvent.Version == sut.Version);
            Assert.True(sut.Version > initialVersion);
        }

        [Fact]
        public void when_events_of_two_types_are_raised_only_appropriate_observers_are_notified()
        {
            FakeEvent eFakeEvent = null;
            BarEvent eBarEvent = null;
            var subHandler = sut.EventStream.OfType<FakeEvent>().SubscribeOn(Scheduler.CurrentThread).Subscribe(x => eFakeEvent = x);
            var barHandler = sut.EventStream.OfType<BarEvent>().SubscribeOn(Scheduler.CurrentThread).Subscribe(x => eBarEvent = x);

            sut.IncrementCounter();

            Assert.NotNull(eFakeEvent);
            Assert.Null(eBarEvent);

            sut.GoToTheBar();
            Assert.NotNull(eBarEvent);
            Assert.True(eFakeEvent.Version < sut.Version);
            Assert.True(sut.numberOfBarEventsRaised == 1);
            Assert.True(sut.numberOfFakeEventsRaised == 1);

            subHandler.Dispose();
            barHandler.Dispose();
        }

        [Fact]
        public void when_syndicated_subscribers_subscribe_should_not_receive_notifications_from_before_subscription()
        {
            List<IEvent> received = new List<IEvent>();
            sut.IncrementCounter();
            sut.GoToTheBar();
            
            var sub = sut.SyndicateEvent<FakeEvent>(received.Add);

            sut.IncrementCounter();

            Assert.Equal(1, received.Count);
            Assert.Equal(2, sut.numberOfFakeEventsRaised);
            Assert.Equal(1, sut.numberOfBarEventsRaised);
            
            sub.Dispose();
        }
    }

    public class given_an_event_repository_and_event_source
    {
        private IEventSourcedRepository<FakeEventSource> _eventRepos;
        private MockRepository mocks = new MockRepository(MockBehavior.Loose);

        private readonly FakeEventSource sut = new FakeEventSource(Guid.NewGuid());

        public given_an_event_repository_and_event_source()
        {
            var m = mocks.Create<IEventSourcedRepository<FakeEventSource>>(MockBehavior.Loose);
            _eventRepos = m.Object;
        }

        [Fact]
        public void when_event_is_published_repository_persists_data()
        {
            var m = Mock.Get(_eventRepos);
            var initialVersion = sut.Version;

            m.Setup(x => x.Save(It.IsAny<FakeEventSource>())).Callback(() => Thread.Sleep(1000));
            sut.IncrementCounter();
            var sub = sut.SyndicateEvent<FakeEvent>(x =>
                                                        {
                                                            Console.WriteLine("Save");
                                                            _eventRepos.Save(sut);
                                                        });

            sut.IncrementCounter();
            
            Assert.True(initialVersion == (sut.Version - 2));
            m.Verify(x => x.Save(sut), Times.Exactly(1));
        }
    }

    
}
