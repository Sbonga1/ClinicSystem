using Clinic.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clinic.Controllers
{
    public class RateController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Rate
        [HttpPost]
        public ActionResult RateService(Appointment model)
        {
            string rate = model.SelectedRating;
            string drEmail = model.DRemail;
            
            try
            {

                var Appointment = db.Appointments.Where(x => x.Email == User.Identity.Name && x.Status2 == "Rate").FirstOrDefault();
                var serviceRate = new ServiceRating
                {
                    Email = drEmail,
                    Rate = int.Parse(rate),
                    AppointmentId = Appointment.Id.ToString()
                };
                Appointment.Status2 = " ";
                db.Entry(Appointment).State = EntityState.Modified;
                db.ServiceRatings.Add(serviceRate);
                db.SaveChanges();
                TempData["Rate Service Success"] = "Your Rate for " + drEmail + " is successfully submitted. ";
            }
            catch
            {
                TempData["Rate Service Failure"] = "Something went wrong while trying to submit your rating, Please try again later. ";
            }
            return RedirectToAction("MyAppointments", "Appointments");
        }
    }
}