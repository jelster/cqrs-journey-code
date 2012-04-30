// ==============================================================================================================
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

using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Infrastructure.EventSourcing
{
    using System;
    using System.Collections.Generic;
    using Messaging;

    public abstract class EventSourced : IEventSourced
    {
        private readonly ISubject<IVersionedEvent> eventSubject = new ReplaySubject<IVersionedEvent>(Scheduler.Immediate); // TODO: should a buffer size be set here? 
        private readonly Dictionary<Type, Action<IVersionedEvent>> handlers = new Dictionary<Type, Action<IVersionedEvent>>();
        
        private readonly List<IVersionedEvent> pendingEvents = new List<IVersionedEvent>();
        
        private readonly Guid id;
        private int version = -1;

        protected EventSourced(Guid id)
        {
            this.id = id;
        }

        public Guid Id
        {
            get { return this.id; }
        }

        public int Version { get { return this.version; } }

        /// <summary>
        /// "Hot" Observable sequence of events. 
        /// Upon subscription, subscribers will be immediately notified of all past events and any future events raised.
        /// For a snapshot collection of events, consider the <seealso cref="Events"/> property.
        /// </summary>
        public IObservable<IVersionedEvent> EventStream { get { return eventSubject.AsObservable(); } } 

        /// <summary>
        /// A "Cold" observable - will only produce events that occured before time of invocation. See <see cref="EventStream"/> for a "Hot" observable
        /// </summary>
        public IEnumerable<IVersionedEvent> Events
        {
            get { return pendingEvents.AsEnumerable(); }
        }

        /// <summary>
        /// Configures a handler for an event. 
        /// </summary>
        protected void Handles<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            this.handlers.Add(typeof(TEvent), @event => handler((TEvent)@event));
        }

        protected void LoadFrom(IEnumerable<IVersionedEvent> pastEvents)
        {
            foreach (var versionedEvent in pastEvents)
            {
                Apply(versionedEvent, false);
            }
        }

        protected void Update(VersionedEvent e)
        {
            UpdateCore(e);
        }

        /// <summary>
        /// Provided to ensure that core versioning and event source mapping is consistent.
        /// </summary>
        /// <param name="e">Event being applied to object</param>
        private void UpdateCore(VersionedEvent e)
        {
            e.SourceId = this.Id;
            e.Version = this.version + 1;
            pendingEvents.Add(e);
            Apply(e);
        }
        private void Apply(IVersionedEvent e, bool notifySubscribers = true)
        {
            this.handlers[e.GetType()].Invoke(e);
            this.version = e.Version;
            
            if (notifySubscribers)
            {
                eventSubject.OnNext(e);
            }
        }
    }
}
