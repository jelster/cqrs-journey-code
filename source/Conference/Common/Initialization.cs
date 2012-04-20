using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azure.Messaging;
using Microsoft.Practices.Unity;
namespace Common
{
    public class Initialization
    {
        public static void InitializeContainer(IUnityContainer container, IMessageSender sender, IMessageReceiver receiver)
        {
            //container.RegisterInstance<ICommandBus>(commandBus);
            //container.RegisterInstance<IEventBus>(eventBus);
            //container.RegisterInstance<ICommandHandlerRegistry>(commandProcessor);
            //container.RegisterInstance(commandProcessor);
        }
    }
}
