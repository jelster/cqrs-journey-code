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

namespace Registration.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;

    public class OrderDTO
    {
        public enum States
        {
            Created = 0,
            PartiallyReserved = 1,
            ReservationCompleted = 2,
            Rejected = 3,
            Confirmed = 4,
        }

        public OrderDTO(Guid orderId, States state)
            : this()
        {
            this.OrderId = orderId;
            this.State = state;
        }

        protected OrderDTO()
        {
            this.Lines = new ObservableCollection<OrderItemDTO>();
        }

        [Key]
        public Guid OrderId { get; private set; }

        public DateTime? ReservationExpirationDate { get; set; }

        public virtual IList<OrderItemDTO> Lines { get; private set; }

        public int StateValue { get; private set; }

        [NotMapped]
        public States State
        {
            get { return (States)this.StateValue; }
            set { this.StateValue = (int)value; }
        }

        public string RegistrantEmail { get; internal set; }
        public string AccessCode { get; internal set; }
    }
}
