using System;
using System.Collections.Generic;
using System.Diagnostics;
using Infrastructure.Configuration;
using Microsoft.Practices.Unity;

namespace Infrastructure
{
    public static class ObjectFactory
    {
        private static IUnityContainer _container;

        public static void Initialize(IUnityContainer container)
        {
            _container = container;
        }
        
        public static T GetInstance<T>()
        {
            ThrowIfNotInitialized();
            return _container.Resolve<T>();
        }

        public static IEnumerable<T> GetAllInstances<T>()
        {
            ThrowIfNotInitialized();
            return _container.ResolveAll<T>();
        }

        public static void Reset()
        {
            ThrowIfNotInitialized();

            lock (_container)
            {
                _container.Dispose();
                _container = null;
                Debug.Assert(_container == null);

                Initialize(new UnityContainer());
            }
        }

        public static void InitializeFromConfiguration(MessagingConfiguration messagingConfiguration)
        {
            if (_container == null)
            {
                Initialize(new UnityContainer());
            }
            else
            {
                Reset();
            }
            foreach (var concreteHandler in messagingConfiguration.Mappings)
            {
                foreach (var subjectType in concreteHandler.Value)
                {
                    _container.RegisterType(subjectType, concreteHandler.Key, messagingConfiguration.Name);
                }
            }
        }

        private static void ThrowIfNotInitialized()
        {
            if (_container == null)
                throw new NullReferenceException("Object Factory has either been reset or is otherwise not initialized");
        }
    }
}
