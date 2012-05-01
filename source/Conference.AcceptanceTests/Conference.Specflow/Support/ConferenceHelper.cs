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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Infrastructure.Azure;
using Infrastructure.Azure.Messaging;
using Infrastructure.Messaging;
using Infrastructure.Serialization;
using Newtonsoft.Json;
using Registration;
using Registration.Commands;
using TechTalk.SpecFlow;
using System.Data.Entity;
using Conference.Common.Entity;

namespace Conference.Specflow
{
    static class ConferenceHelper
    {
        static ConferenceHelper()
        {
            Database.DefaultConnectionFactory = new ServiceConfigurationSettingConnectionFactory(Database.DefaultConnectionFactory);
            Database.SetInitializer<ConferenceContext>(null);
        }

        public static ConferenceInfo PopulateConfereceData(Table table, string conferenceSlug)
        {
            ConferenceService svc = new ConferenceService(BuildEventBus());
            ConferenceInfo conference = svc.FindConference(conferenceSlug);

            if (null != conference)
            {
                if(conference.Seats.Count == 0)
                    svc.FindSeats(conference.Id).ToList().ForEach(s => conference.Seats.Add(s));
                return conference;
            }

            conference = BuildConferenceInfo(table, conferenceSlug);
            svc.CreateConference(conference);
            svc.Publish(conference.Id);
            // Wait for the events to be processed
            Thread.Sleep(Constants.WaitTimeout);
            return conference;
        }

        public static Guid ReserveSeats(ConferenceInfo conference, Table table)
        {
            List<SeatQuantity> seats = new List<SeatQuantity>();

            foreach (var row in table.Rows)
            {
                var seatInfo = conference.Seats.FirstOrDefault(s => s.Name == row["seat type"]);
                if (seatInfo != null)
                {
                    int qt = row.ContainsKey("quantity") ? Int32.Parse(row["quantity"]) : seatInfo.Quantity;
                    seats.Add(new SeatQuantity(seatInfo.Id, qt));
                }
            }

            var seatReservation = new MakeSeatReservation
            {
                ConferenceId = conference.Id,
                ReservationId = Guid.NewGuid(),
                Seats = seats
            };

            var commandBus = BuildCommandBus();
            commandBus.Send(seatReservation);

            // Wait for the events to be processed
            Thread.Sleep(Constants.WaitTimeout);

            return seatReservation.ReservationId;
        }

        public static void CancelSeatReservation(Guid conferenceId, Guid reservationId)
        {
            var seatReservation = new CancelSeatReservation 
            { 
                ConferenceId = conferenceId, 
                ReservationId = reservationId 
            };
            
            var commandBus = BuildCommandBus();
            commandBus.Send(seatReservation);
        }

        private static ConferenceInfo BuildConferenceInfo(Table seats, string conferenceSlug)
        {
            var conference = new ConferenceInfo()
            {
                Description = "Acceptance Tests CQRS summit 2012 conference (" + conferenceSlug + ")",
                Name = conferenceSlug,
                Slug = conferenceSlug,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                OwnerName = "test",
                OwnerEmail = "testEmail",
                IsPublished = true,
                WasEverPublished = true
            };

            foreach (var row in seats.Rows)
            {
                SeatInfo seat = new SeatInfo()
                {
                    Id = Guid.NewGuid(),
                    Description = row["seat type"],
                    Name = row["seat type"],
                    Price = Convert.ToDecimal(row["rate"].Replace("$", "")),
                    Quantity = Convert.ToInt32(row["quota"])
                };
                conference.Seats.Add(seat);
            }

            return conference;
        }

        private static IEventBus BuildEventBus()
        {
#if LOCAL
            // TODO: this WON'T work to integrate across both websites!
            // Populate upfront the Mgmt DB with SB instances before using this option
            return new MemoryEventBus();
#else
            return new EventBus(GetTopicSender("events"), new MetadataProvider(), new JsonTextSerializer());
#endif
        }

        private static ICommandBus BuildCommandBus()
        {
#if LOCAL
            // TODO: this WON'T work to integrate across both websites!
            // Populate upfront the Mgmt DB with SB instances before using this option
            return new MemoryCommandBus();
#else
            return new CommandBus(GetTopicSender("commands"), new MetadataProvider(), new JsonTextSerializer());
#endif
        }

        private static TopicSender GetTopicSender(string topic)
        {
            var settings = InfrastructureSettings.ReadMessaging("Settings.xml");
            return new TopicSender(settings, "conference/" + topic);
        }
    }
}
