using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.EventSourcing;
using Microsoft.Practices.Unity;
using Moq;

namespace Infrastructure.Tests
{
    public class TestStartup
    {
        public static void Configure(TestEventSource eventSourced)
        {
            var mocks = new MockRepository(MockBehavior.Loose);
            var con = new UnityContainer();
            var m = mocks.Create<IEventSourcedRepository<TestEventSource>>(MockBehavior.Loose);

            m.Setup(x => x.Save(It.IsAny<TestEventSource>()));
            m.Setup(x => x.Find(It.IsAny<Guid>())).Returns<Guid>(x => eventSourced);

            IEventSourcedRepository<TestEventSource> eventRepos = m.Object;

            con.RegisterType(typeof(IEventSourcedRepository<>), eventRepos.GetType(), new InjectionFactory(c => eventRepos));
            ObjectFactory.Reset();
            ObjectFactory.Initialize(con);

        }
    }
}
