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

namespace Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Sample in-memory command bus that is asynchronous.
    /// </summary>
    public class MemoryCommandBus : ICommandBus, ICommandHandlerRegistry
    {
        private List<ICommandHandler> handlers = new List<ICommandHandler>();
        private List<Envelope<ICommand>> commands = new List<Envelope<ICommand>>();

        public MemoryCommandBus(params ICommandHandler[] handlers)
        {
            this.handlers.AddRange(handlers);
        }

        public void Register(ICommandHandler handler)
        {
            this.handlers.Add(handler);
        }

        public void SendInvoke(Envelope<ICommand> command)
        {
            if (command.Delay > TimeSpan.Zero)
                {
                    Thread.Sleep(command.Delay);
                }

                var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.Body.GetType());

                foreach (dynamic handler in this.handlers
                    .Where(x => handlerType.IsAssignableFrom(x.GetType())))
                {
                    var bodyObj = command.Body as dynamic;
                    handler.Handle(bodyObj);
                }
        }

        public void Send(Envelope<ICommand> command)
        {
            this.commands.Add(command);
            SendInvoke(command);
            //Task.Factory.StartNew(() => SendInvoke(command));
        }

        public void Send(IEnumerable<Envelope<ICommand>> commandsToSend)
        {
            foreach (var command in commandsToSend)
            {
                this.Send(command);
            }
        }

        public IEnumerable<Envelope<ICommand>> Commands
        {
            get { return this.commands; }
        }
    }
}
