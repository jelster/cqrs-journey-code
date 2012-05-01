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

namespace Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Conference.Common.Utils;
    using Infrastructure.EventSourcing;
    using Registration.Events;

    public class Order : EventSourced
    {
        private static readonly TimeSpan ReservationAutoExpiration = TimeSpan.FromMinutes(15);

        private List<SeatQuantity> seats;
        private bool isConfirmed;

        protected Order(Guid id) : base(id)
        {
            base.Handles<OrderPlaced>(this.OnOrderPlaced);
            base.Handles<OrderUpdated>(this.OnOrderUpdated);
            base.Handles<OrderPartiallyReserved>(this.OnOrderPartiallyReserved);
            base.Handles<OrderReservationCompleted>(this.OnOrderReservationCompleted);
            base.Handles<OrderExpired>(this.OnOrderExpired);
            base.Handles<OrderPaymentConfirmed>(this.OnOrderPaymentConfirmed);
            base.Handles<OrderRegistrantAssigned>(this.OnOrderRegistrantAssigned);
        }

        public Order(Guid id, IEnumerable<IVersionedEvent> history) : this(id)
        {
            this.LoadFrom(history);
        }

        public Order(Guid id, Guid conferenceId, IEnumerable<OrderItem> items) : this(id)
        {
            this.Update(new OrderPlaced 
            {
                ConferenceId = conferenceId, 
                Seats = ConvertItems(items),
                ReservationAutoExpiration = DateTime.UtcNow.Add(ReservationAutoExpiration),
                AccessCode = HandleGenerator.Generate(6)
            });
        }

        public void UpdateSeats(IEnumerable<OrderItem> seats)
        {
            this.Update(new OrderUpdated { Seats = ConvertItems(seats) });
        }

        public void MarkAsReserved(DateTime expirationDate, IEnumerable<SeatQuantity> reservedSeats)
        {
            if (this.isConfirmed)
                throw new InvalidOperationException("Cannot modify a confirmed order.");

            var reserved = reservedSeats.ToList();

            // Is there an order item which didn't get an exact reservation?
            if (this.seats.Any(item => !reserved.Any(seat => seat.SeatType == item.SeatType && seat.Quantity == item.Quantity)))
            {
                this.Update(new OrderPartiallyReserved { ReservationExpiration = expirationDate, Seats = reserved.ToArray() });
            }
            else
            {
                this.Update(new OrderReservationCompleted { ReservationExpiration = expirationDate, Seats = reserved.ToArray() });
            }
        }

        public void Expire()
        {
            if (this.isConfirmed)
                throw new InvalidOperationException();

            this.Update(new OrderExpired());
        }

        public void ConfirmPayment()
        {
            this.Update(new OrderPaymentConfirmed());
        }

        public void AssignRegistrant(string firstName, string lastName, string email)
        {
            this.Update(new OrderRegistrantAssigned
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
            });
        }

        private void OnOrderPlaced(OrderPlaced e)
        {
            this.seats = e.Seats.ToList();
        }

        private void OnOrderUpdated(OrderUpdated e)
        {
            this.seats = e.Seats.ToList();
        }
        
        private void OnOrderPartiallyReserved(OrderPartiallyReserved e)
        {
            this.seats = e.Seats.ToList();
        }

        private void OnOrderReservationCompleted(OrderReservationCompleted e)
        {
            this.seats = e.Seats.ToList();
        }

        private void OnOrderExpired(OrderExpired e)
        {
        }

        private void OnOrderPaymentConfirmed(OrderPaymentConfirmed e)
        {
            this.isConfirmed = true;
        }

        private void OnOrderRegistrantAssigned(OrderRegistrantAssigned e)
        {
        }

        private static List<SeatQuantity> ConvertItems(IEnumerable<OrderItem> items)
        {
            return items.Select(x => new SeatQuantity(x.SeatType, x.Quantity)).ToList();
        }
    }
}
