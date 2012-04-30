using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Handling;

namespace Infrastructure.Configuration
{
    public static class StartUp
    {
         public static void InitializeMessagingComponents()
         {
             var regAsm = Assembly.Load("Registration");
             var allAsm = regAsm.GetReferencedAssemblies().Select(Assembly.Load).ToList().Where(loaded => !loaded.GlobalAssemblyCache).ToList();
             allAsm.Add(regAsm);

             var commandConfig = MessagingConfiguration.Initialize(typeof(ICommand), typeof(ICommandHandler), allAsm, "Commands");
             var eventConfig = MessagingConfiguration.Initialize(typeof(IEvent), typeof(IEventHandler), allAsm, "Events");
             
             // TODO: move this assertion into MessagingConfiguration.Initialize()?
             Debug.Assert(commandConfig.Handlers.Any() && eventConfig.Handlers.Any());
             
             
             //commandConfig.Handlers.GroupBy(x => x.GetInterfaces().Where(i => i.GetGenericArguments().Any()), (x, k) => new { k, Handles = x });
             //eventConfig.Handlers.GroupBy(x => x.GetInterfaces().Where(i => i.GetGenericArguments().Any()), (x, k) => new { k, Handles = x });
         }
    }
}