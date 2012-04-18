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

namespace Registration.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;

    public class SeatsReserved : IDomainEvent
    {
        private readonly Guid sourceId;
        private readonly int version;
        private readonly Guid reservationId;
        private readonly IEnumerable<SeatQuantity> reservationDetails;
        private readonly IEnumerable<SeatQuantity> availableSeatsChanged;

        public SeatsReserved(Guid sourceId, int version, Guid reservationId, IEnumerable<SeatQuantity> reservationDetails, IEnumerable<SeatQuantity> availableSeatsChanged)
        {
            this.sourceId = sourceId;
            this.version = version;
            this.reservationId = reservationId;
            this.reservationDetails = reservationDetails.ToList();
            this.availableSeatsChanged = availableSeatsChanged.ToList();
        }

        public Guid SourceId { get { return this.sourceId; } }

        public int Version { get { return this.version; } }

        public Guid ReservationId { get { return this.reservationId; } }

        public IEnumerable<SeatQuantity> ReservationDetails { get { return this.reservationDetails; } }

        public IEnumerable<SeatQuantity> AvailableSeatsChanged { get { return this.availableSeatsChanged; } }

    }
}