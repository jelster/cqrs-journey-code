using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Infrastructure.Configuration
{
    public class MessagingConfiguration
    {
        public string Name { get; set; }
        public List<Type> Subject { get; set; }
        public List<Type> Handlers { get; set; }

        public Dictionary<Type, IEnumerable<Type>> Mappings
        {
            get
            {
                return this.Handlers.Select(x => new { Handler = x, HandledType = x.GetInterfaces().Where(i => i.GetGenericArguments().Any()) }).ToDictionary(x => x.Handler, x => x.HandledType);
            }
        }

        public static readonly ConcurrentDictionary<string, MessagingConfiguration> Current = new ConcurrentDictionary<string, MessagingConfiguration>();

        private static readonly object Sync = new Object();

        protected MessagingConfiguration(string name)
        {
            Subject = new List<Type>();
            Handlers = new List<Type>();
            Name = name;
        }

        public static MessagingConfiguration Initialize(Type cmdType, Type handType, IEnumerable<Assembly> commandDefinitions, string configName = "global")
        {
            // TODO: parameter validation before assignment
            var types = commandDefinitions.Select(x => x.GetTypes()).ToList();

            // discovery
            var filtered = types.SelectMany(t => t.Where(x => !x.IsInterface && x.GetInterfaces().Any(i => i.Name == handType.Name))).ToList();

            var commandTypes = types.SelectMany(t => t).Where(x => x.IsClass && x.GetInterface(cmdType.Name) != null).ToList();

            var instance = new MessagingConfiguration(configName);

            if (filtered.Any())
            {
                instance.Handlers.AddRange(filtered);
            }
            if (commandTypes.Any())
            {
                instance.Subject.AddRange(commandTypes);
            }

            lock (Sync)
            {
                Current.AddOrUpdate(configName, instance, (s, configuration) => instance); 
            }

            return instance;
        }
    }
}