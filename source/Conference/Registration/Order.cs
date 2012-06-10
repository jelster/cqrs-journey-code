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
    using AutoMapper;
    using Conference.Common.Utils;
    using Infrastructure.EventSourcing;
    using Registration.Events;

    public class Order : EventSourced
    {
        private static readonly TimeSpan ReservationAutoExpiration = TimeSpan.FromMinutes(15);

        private List<SeatQuantity> seats;
        private bool isConfirmed;
        private Guid conferenceId;

        static Order()
        {
            // Mapping old version of the OrderPaymentConfirmed event to the new version.
            // Currently it is being done explicitly by the consumer, but this one in particular could be done
            // at the deserialization level, as it is just a rename, not a functionality change.
            Mapper.CreateMap<OrderPaymentConfirmed, OrderConfirmed>();
        }

        protected Order(Guid id)
            : base(id)
        {
            base.Handles<OrderPlaced>(this.OnOrderPlaced);
            base.Handles<OrderUpdated>(this.OnOrderUpdated);
            base.Handles<OrderPartiallyReserved>(this.OnOrderPartiallyReserved);
            base.Handles<OrderReservationCompleted>(this.OnOrderReservationCompleted);
            base.Handles<OrderExpired>(this.OnOrderExpired);
            base.Handles<OrderPaymentConfirmed>(e => this.OnOrderConfirmed(Mapper.Map<OrderConfirmed>(e)));
            base.Handles<OrderConfirmed>(this.OnOrderConfirmed);
            base.Handles<OrderRegistrantAssigned>(this.OnOrderRegistrantAssigned);
            base.Handles<OrderTotalsCalculated>(this.OnOrderTotalsCalculated);
        }

        public Order(Guid id, IEnumerable<IVersionedEvent> history)
            : this(id)
        {
            this.LoadFrom(history);
        }

        public Order(Guid id, Guid conferenceId, IEnumerable<OrderItem> items, IPricingService pricingService)
            : this(id)
        {
            var all = ConvertItems(items);
            var totals = pricingService.CalculateTotal(conferenceId, all.AsReadOnly());

            this.Update(new OrderPlaced
            {
                ConferenceId = conferenceId,
                Seats = all,
                ReservationAutoExpiration = DateTime.UtcNow.Add(ReservationAutoExpiration),
                AccessCode = HandleGenerator.Generate(6)
            });
            this.Update(new OrderTotalsCalculated { Total = totals.Total, Lines = totals.Lines != null ? totals.Lines.ToArray() : null, IsFreeOfCharge = totals.Total == 0m });
        }

        public void UpdateSeats(IEnumerable<OrderItem> items, IPricingService pricingService)
        {
            var all = ConvertItems(items);
            var totals = pricingService.CalculateTotal(this.conferenceId, all.AsReadOnly());

            this.Update(new OrderUpdated { Seats = all });
            this.Update(new OrderTotalsCalculated { Total = totals.Total, Lines = totals.Lines != null ? totals.Lines.ToArray() : null, IsFreeOfCharge = totals.Total == 0m });
        }

        public void MarkAsReserved(IPricingService pricingService, DateTime expirationDate, IEnumerable<SeatQuantity> reservedSeats)
        {
            if (this.isConfirmed)
                throw new InvalidOperationException("Cannot modify a confirmed order.");

            var reserved = reservedSeats.ToList();

            // Is there an order item which didn't get an exact reservation?
            if (this.seats.Any(item => item.Quantity != 0 && !reserved.Any(seat => seat.SeatType == item.SeatType && seat.Quantity == item.Quantity)))
            {
                var totals = pricingService.CalculateTotal(this.conferenceId, reserved.AsReadOnly());

                this.Update(new OrderPartiallyReserved { ReservationExpiration = expirationDate, Seats = reserved.ToArray() });
                this.Update(new OrderTotalsCalculated { Total = totals.Total, Lines = totals.Lines != null ? totals.Lines.ToArray() : null, IsFreeOfCharge = totals.Total == 0m });
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

        public void Confirm()
        {
            this.Update(new OrderConfirmed());
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

        public SeatAssignments CreateSeatAssignments()
        {
            if (!this.isConfirmed)
                throw new InvalidOperationException("Cannot create seat assignments for an order that isn't paid yet.");

            return new SeatAssignments(this.Id, this.seats.AsReadOnly());
        }

        private void OnOrderPlaced(OrderPlaced e)
        {
            this.conferenceId = e.ConferenceId;
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

        private void OnOrderConfirmed(OrderConfirmed e)
        {
            this.isConfirmed = true;
        }

        private void OnOrderRegistrantAssigned(OrderRegistrantAssigned e)
        {
        }

        private void OnOrderTotalsCalculated(OrderTotalsCalculated e)
        {
        }

        private static List<SeatQuantity> ConvertItems(IEnumerable<OrderItem> items)
        {
            return items.Select(x => new SeatQuantity(x.SeatType, x.Quantity)).ToList();
        }
    }
}
