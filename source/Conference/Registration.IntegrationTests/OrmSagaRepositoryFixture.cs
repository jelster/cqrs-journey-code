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

namespace Registration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using Common;
    using Moq;
    using Registration.Database;
    using Xunit;

    public class OrmProcessRepositoryFixture
    {
        public OrmProcessRepositoryFixture()
        {
            using (var context = new TestOrmProcessRepository(Mock.Of<ICommandBus>()))
            {
                if (context.Database.Exists()) context.Database.Delete();

                context.Database.Create();
            }
        }

        [Fact]
        public void WhenSavingEntity_ThenCanRetrieveIt()
        {
            var id = Guid.NewGuid();

            using (var context = new TestOrmProcessRepository(Mock.Of<ICommandBus>()))
            {
                var conference = new OrmTestProcess(id);
                context.Save(conference);
            }

            using (var context = new TestOrmProcessRepository(Mock.Of<ICommandBus>()))
            {
                var conference = context.Find<OrmTestProcess>(id);

                Assert.NotNull(conference);
            }
        }

        [Fact]
        public void WhenSavingEntityTwice_ThenCanReloadIt()
        {
            var id = Guid.NewGuid();

            using (var context = new TestOrmProcessRepository(Mock.Of<ICommandBus>()))
            {
                var conference = new OrmTestProcess(id);
                context.Save(conference);
            }

            using (var context = new TestOrmProcessRepository(Mock.Of<ICommandBus>()))
            {
                var conference = context.Find<OrmTestProcess>(id);
                conference.Title = "CQRS Journey";

                context.Save(conference);

                context.Entry(conference).Reload();

                Assert.Equal("CQRS Journey", conference.Title);
            }
        }

        [Fact]
        public void WhenEntityExposesEvent_ThenRepositoryPublishesIt()
        {
            var bus = new Mock<ICommandBus>();
            var commands = new List<ICommand>();

            bus.Setup(x => x.Send(It.IsAny<IEnumerable<Envelope<ICommand>>>()))
                .Callback<IEnumerable<Envelope<ICommand>>>(x => commands.AddRange(x.Select(e => e.Body)));

            var command = new TestCommand();

            using (var context = new TestOrmProcessRepository(bus.Object))
            {
                var aggregate = new OrmTestProcess(Guid.NewGuid());
                aggregate.AddCommand(command);
                context.Save(aggregate);
            }

            Assert.Equal(1, commands.Count);
            Assert.True(commands.Contains(command));
        }

        public class TestOrmProcessRepository : OrmProcessRepository
        {
            public TestOrmProcessRepository(ICommandBus commandBus)
                : base("TestOrmProcessRepository", commandBus)
            {
            }

            public DbSet<OrmTestProcess> TestProcesses { get; set; }
        }

        public class TestCommand : ICommand
        {
            public Guid Id { get; set; }
        }
    }

    public class OrmTestProcess : IAggregateRoot, ICommandPublisher
    {
        private List<Envelope<ICommand>> commands = new List<Envelope<ICommand>>();

        protected OrmTestProcess() { }

        public OrmTestProcess(Guid id)
        {
            this.Id = id;
        }

        public Guid Id { get; set; }
        public string Title { get; set; }

        public void AddCommand(ICommand command)
        {
            this.commands.Add(Envelope.Create(command));
        }

        public IEnumerable<Envelope<ICommand>> Commands { get { return this.commands; } }
    }
}
