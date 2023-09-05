using Clinic.Models;
using Microsoft.Ajax.Utilities;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clinic.Controllers
{
    public class PayPalController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult CreatePayment(double CartTotal, string Payment = "Cart")
        {
            if (Payment == "Consultation")
            {
                Session["Payment"] = "Consultation";

                var CurrentUser = User.Identity.Name;
                double convertedTot = Math.Round(CartTotal / 14.357);
                int Rem = (int)(CartTotal % 14.357);
                string Cost = convertedTot.ToString() + "." + Rem;

                // Set up the PayPal API context
                var apiContext = PayPalConfig.GetAPIContext();

                // Retrieve the API credentials from configuration
                var clientId = ConfigurationManager.AppSettings["PayPalClientId"];
                var clientSecret = ConfigurationManager.AppSettings["PayPalClientSecret"];
                apiContext.Config = new Dictionary<string, string> { { "mode", "sandbox" } };
                var accessToken = new OAuthTokenCredential(clientId, clientSecret, apiContext.Config).GetAccessToken();
                apiContext.AccessToken = accessToken;

                // Create a new payment object
                var payment = new Payment
                {
                    intent = "sale",
                    payer = new Payer { payment_method = "paypal" },
                    transactions = new List<Transaction>
                {
            new Transaction
            {
                amount = new Amount
                {

                    total = Cost,
                    currency = "USD"
                },

                description = "Consultation Payment"
            }
        },
                    redirect_urls = new RedirectUrls
                    {
                        return_url = Url.Action("CompletePayment", "PayPal", null, Request.Url.Scheme),
                        cancel_url = Url.Action("CancelPayment", "PayPal", null, Request.Url.Scheme)
                    }
                };

                // Create the payment and get the approval URL
                var createdPayment = payment.Create(apiContext);
                var approvalUrl = createdPayment.links.FirstOrDefault(l => l.rel == "approval_url")?.href;

                // Redirect the user to the PayPal approval URL
                return Redirect(approvalUrl);
            }
            else
            {
                var CurrentUser = User.Identity.Name;
                double convertedTot = Math.Round(CartTotal / 14.357);
                int Rem = (int)(CartTotal % 14.357);
                string Cost = convertedTot.ToString() + "." + Rem;

                // Set up the PayPal API context
                var apiContext = PayPalConfig.GetAPIContext();

                // Retrieve the API credentials from configuration
                var clientId = ConfigurationManager.AppSettings["PayPalClientId"];
                var clientSecret = ConfigurationManager.AppSettings["PayPalClientSecret"];
                apiContext.Config = new Dictionary<string, string> { { "mode", "sandbox" } };
                var accessToken = new OAuthTokenCredential(clientId, clientSecret, apiContext.Config).GetAccessToken();
                apiContext.AccessToken = accessToken;

                // Create a new payment object
                var payment = new Payment
                {
                    intent = "sale",
                    payer = new Payer { payment_method = "paypal" },
                    transactions = new List<Transaction>
                {
            new Transaction
            {
                amount = new Amount
                {

                    total = Cost,
                    currency = "USD"
                },

                description = "Medicine Payment"
            }
        },
                    redirect_urls = new RedirectUrls
                    {
                        return_url = Url.Action("CompletePayment", "PayPal", null, Request.Url.Scheme),
                        cancel_url = Url.Action("CancelPayment", "PayPal", null, Request.Url.Scheme)
                    }
                };

                // Create the payment and get the approval URL
                var createdPayment = payment.Create(apiContext);
                var approvalUrl = createdPayment.links.FirstOrDefault(l => l.rel == "approval_url")?.href;

                // Redirect the user to the PayPal approval URL
                return Redirect(approvalUrl);
            }
        }


        public ActionResult CompletePayment(string paymentId, string token, string PayerID)
        {
            // Set up the PayPal API context
            var apiContext = PayPalConfig.GetAPIContext();

            // Execute the payment
            var paymentExecution = new PaymentExecution { payer_id = PayerID };
            var executedPayment = new Payment { id = paymentId }.Execute(apiContext, paymentExecution);

            // Process the payment completion
            // You can save the transaction details or perform other necessary actions

            // Redirect the user to a success page
            return RedirectToAction("PaymentSuccess");
        }

        public ActionResult CancelPayment()
        {
            // Handle the payment cancellation
            // You can redirect the user to a cancellation page or perform other necessary actions

            // Redirect the user to a cancellation page
            return RedirectToAction("PaymentCancelled");
        }

        public ActionResult PaymentSuccess()
        {
            string sesPay = Session["Payment"] as string;
            if (sesPay == null)
            {
                var order = new CustomerOrder();
                TryUpdateModel(order);
                order.CustomerUserName = User.Identity.Name;
                order.DateCreated = DateTime.Now;
                order.LastName = "Maz";
                order.FirstName = "Sbonga";
                order.Phone = "636509309";
                order.Address = "Ifafa glebe";
                order.City = "Umzinto";
                order.Country = "South Africa";
                order.State = "USA";
                order.PostalCode = "4200";
                order.Email = User.Identity.Name;
                order.Amount = 60;

                db.CustomerOrders.Add(order);
                db.SaveChanges();

                var cart = ShoppingCart.GetCart(this.HttpContext);
                cart.CreateOrder(order);
                db.SaveChanges();
            }
            else
            {
                string CurrentUser = User.Identity.Name;
                var Appoint = db.Appointments.Where(x => x.Email == CurrentUser && x.Status == "Awaiting Payment").FirstOrDefault();
                Appoint.Status = "Settled";
                Appoint.Status2 = "Rate";
                db.Entry(Appoint).State = EntityState.Modified;
                db.SaveChanges();
            }

            Session["Payment"] = null;
            return View();
        }

    }
}