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

namespace Conference.Web.Public.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Web.Mvc;
    using Conference.Web.Public.Models;
    using Infrastructure.Messaging;
    using Payments.Contracts.Commands;
    using Registration.Commands;
    using Registration.ReadModel;

    public class RegistrationController : Controller
    {
        public const string ThirdPartyProcessorPayment = "thirdParty";
        public const string InvoicePayment = "invoice";
        private const int WaitTimeoutInSeconds = 5;

        private readonly ICommandBus commandBus;
        private readonly IOrderDao orderDao;
        private readonly IConferenceDao conferenceDao;

        public RegistrationController(ICommandBus commandBus, IOrderDao orderDao, IConferenceDao conferenceDao)
        {
            this.commandBus = commandBus;
            this.orderDao = orderDao;
            this.conferenceDao = conferenceDao;
        }

        [HttpGet]
        public ActionResult StartRegistration()
        {
            ViewBag.OrderId = Guid.NewGuid();

            return View(this.conferenceDao.GetPublishedSeatTypes(this.Conference.Id));
        }

        [HttpPost]
        public ActionResult StartRegistration(RegisterToConference command)
        {
            if (!ModelState.IsValid)
            {
                return StartRegistration();
            }

            // TODO: validate incoming seat types correspond to the conference.

            command.ConferenceId = this.Conference.Id;
            this.commandBus.Send(command);

            return RedirectToAction("SpecifyRegistrantAndPaymentDetails", new { conferenceCode = this.Conference.Code, orderId = command.OrderId });
        }

        [HttpGet]
        public ActionResult SpecifyRegistrantAndPaymentDetails(Guid orderId)
        {
            var order = this.WaitUntilUpdated(orderId);
            if (order == null)
            {
                return View("ReservationUnknown");
            }

            if (order.State == OrderDTO.States.Rejected)
            {
                return View("ReservationRejected");
            }

            var orderViewModel = this.CreateViewModel(order);

            // NOTE: we use the view bag to pass out of band details needed for the UI.
            this.ViewBag.ExpirationDateUTC = order.ReservationExpirationDate;

            // We just render the command which is later posted back.
            return View(
                new RegistrationViewModel
                {
                    RegistrantDetails = new AssignRegistrantDetails { OrderId = orderId },
                    Order = orderViewModel
                });
        }

        [HttpPost]
        public ActionResult SpecifyRegistrantAndPaymentDetails(AssignRegistrantDetails command, string paymentType)
        {
            if (!ModelState.IsValid)
            {
                return SpecifyRegistrantAndPaymentDetails(command.OrderId);
            }

            var order = this.orderDao.GetOrderDetails(command.OrderId);

            // TODO check conference and order exist.
            // TODO validate that order belongs to the user.

            if (order == null)
            {
                throw new ArgumentException();
            }

            switch (paymentType)
            {
                case ThirdPartyProcessorPayment:

                    return InitiateRegistrationWithThirdPartyProcessorPayment(command, command.OrderId);

                case InvoicePayment:
                    break;

                default:
                    break;
            }

            throw new InvalidOperationException();
        }

        [HttpGet]
        public ActionResult TransactionCompleted(Guid orderId, string transactionResult)
        {
            if (transactionResult == "accepted")
            {
                return RedirectToAction("ThankYou", new { conferenceCode = this.Conference.Code, orderId = orderId });
            }
            else
            {
                return RedirectToAction("SpecifyPaymentDetails", new { conferenceCode = this.Conference.Code, orderId = orderId });
            }
        }

        [HttpGet]
        public ActionResult DisplayOrderStatus(string conferenceCode, Guid orderId)
        {
            // TODO: What is this? There is no backing view for this action!
            return View();
        }

        [HttpGet]
        public ActionResult ThankYou(string conferenceCode, Guid orderId)
        {
            var order = this.orderDao.GetOrderDetails(orderId);

            return View(order);
        }

        private ActionResult InitiateRegistrationWithThirdPartyProcessorPayment(AssignRegistrantDetails command, Guid orderId)
        {
            var paymentCommand = CreatePaymentCommand(orderId);

            this.commandBus.Send(new ICommand[] { command, paymentCommand });

            var paymentAcceptedUrl = this.Url.Action("ThankYou", new { conferenceCode = this.Conference.Code, orderId });
            var paymentRejectedUrl = this.Url.Action("TransactionCompleted", new { conferenceCode = this.Conference.Code, orderId, transactionResult = "rejected" });

            return RedirectToAction(
                "ThirdPartyProcessorPayment",
                "Payment",
                new
                {
                    conferenceCode = this.Conference.Code,
                    paymentId = paymentCommand.PaymentId,
                    paymentAcceptedUrl,
                    paymentRejectedUrl
                });
        }

        private InitiateThirdPartyProcessorPayment CreatePaymentCommand(Guid orderId)
        {
            // TODO extract pricing outside the controller
            var seats = this.conferenceDao.GetPublishedSeatTypes(this.Conference.Id);
            var order = this.orderDao.GetOrderDetails(orderId);

            var items =
                from seat in seats
                join orderItem in order.Lines on seat.Id equals orderItem.SeatType
                select new { orderItem.SeatType, orderItem.ReservedSeats, seat.Price, ItemPrice = orderItem.ReservedSeats * seat.Price };

            var description = "Registration for " + this.Conference.Name;
            var totalAmount = items.Sum(i => i.ItemPrice);

            var paymentCommand =
                new InitiateThirdPartyProcessorPayment
                {
                    PaymentId = Guid.NewGuid(),
                    ConferenceId = this.Conference.Id,
                    PaymentSourceId = orderId,
                    Description = description,
                    TotalAmount = totalAmount
                };

            return paymentCommand;
        }

        private OrderViewModel CreateViewModel()
        {
            var seats = this.conferenceDao.GetPublishedSeatTypes(this.Conference.Id);
            var viewModel =
                new OrderViewModel
                {
                    ConferenceId = this.Conference.Id,
                    ConferenceCode = this.Conference.Code,
                    ConferenceName = this.Conference.Name,
                    Items = seats.Select(s => new OrderItemViewModel { SeatTypeId = s.Id, SeatTypeDescription = s.Description, Price = s.Price }).ToList(),
                };

            return viewModel;
        }

        private OrderViewModel CreateViewModel(OrderDTO order)
        {
            var viewModel = this.CreateViewModel();
            viewModel.Id = order.OrderId;

            // TODO check DTO matches view model

            foreach (var line in order.Lines)
            {
                var seat = viewModel.Items.First(s => s.SeatTypeId == line.SeatType);
                seat.Quantity = line.ReservedSeats;
            }

            return viewModel;
        }

        private OrderDTO WaitUntilUpdated(Guid orderId)
        {
            var deadline = DateTime.Now.AddSeconds(WaitTimeoutInSeconds);

            while (DateTime.Now < deadline)
            {
                var order = this.orderDao.GetOrderDetails(orderId);

                if (order != null && order.State != OrderDTO.States.Created)
                {
                    return order;
                }

                Thread.Sleep(500);
            }

            return null;
        }

        private ConferenceAliasDTO conference;
        protected ConferenceAliasDTO Conference
        {
            get
            {
                if (this.conference == null)
                {
                    var conferenceCode = (string)ControllerContext.RouteData.Values["conferenceCode"];
                    this.conference = this.conferenceDao.GetConferenceAlias(conferenceCode);
                }

                return this.conference;
            }
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);

            if (filterContext.Result is ViewResultBase)
            {
                this.ViewBag.Conference = this.Conference;
            }
        }
    }
}
