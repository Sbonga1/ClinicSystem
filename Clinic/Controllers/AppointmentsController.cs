using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Clinic.Models;
using Microsoft.Ajax.Utilities;

namespace Clinic.Controllers
{
    public class AppointmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        
        
        // GET: Appointments
        public ActionResult Index()
        {
            return View(db.Appointments.ToList());
        }
        

        public ActionResult MyAppointments()
        {
            string user = User.Identity.Name;
            var appointments = db.Appointments;
            return View(appointments.Where(c => c.Email == user));
            
        }

        public ActionResult drAppointments()
        {
            string user = User.Identity.Name;
            var appointments = db.Appointments;
            return View(appointments.Where(c => c.DRemail == user && c.Status == "Approved"));
        }


        /**/
        [HttpPost]
        public ActionResult ApproveAppointment(string email, int appointmentId, string note)
        {
            try
            {
                var app = db.Appointments.Where(x => x.Id == appointmentId).FirstOrDefault();
                app.Status = "Approved";

                db.Entry(app).State = EntityState.Modified;

                
                string Name = app.Name;
                string Surname = app.Surname;
                // Prepare email message
                var email2 = new MailMessage();
                email2.From = new MailAddress("clinimanagement@outlook.com");
                email2.To.Add(email);
                email2.Subject = "Appointment Approved";

                email2.Body = "Dear " + Name + " " + Surname + "\n\nPlease note that your appointment has been Approved.\n " + note + "." +
            "\n\n---------------------------------------------------\n" +
            "Thank you. Your health is our concern." +
            "\n---------------------------------------------------" +
            "\n\nKind Regards,\nClinic";
                // Use the SMTP settings from web.config
                var smtpClient = new SmtpClient();

                // The SmtpClient will automatically use the settings from web.config
                smtpClient.Send(email2);




                db.SaveChanges();

                var responseData = new { message = "Appointment successfully Approved, Email Sent to client." };
                return Json(responseData);

            }
            catch (Exception ex)
            {
                // Handle errors and return an error response if needed
                return Json(new { error = ex.Message });
            }

        }


        /**/
        [HttpPost]
        public ActionResult DeclineAppointment(string email, int appointmentId, string reason)
        {
            try
            {
                var calenderEvent = db.calendarEvents.Where(x => x.Email == email && x.status == "Active").FirstOrDefault();
                var timeSlot = db.TimeSlots.Where(x => x.BookedBy == email && x.IsAvailable == false).FirstOrDefault();
                timeSlot.BookedBy = " ";
                timeSlot.IsAvailable = true;





                var app = (from x in db.Appointments
                           where x.Email == email && x.Status == "Awaiting Approval"
                           select x).FirstOrDefault();
                

                string Name = app.Name;
                string Surname = app.Surname;
                // Prepare email message
                var email2 = new MailMessage();
                email2.From = new MailAddress("clinimanagement@outlook.com");
                email2.To.Add(email);
                email2.Subject = "Appointment Declined";

                email2.Body = "Dear " + Name + " " + Surname + "\n\nWe regret to inform you that your appointment has been declined due to " + reason + "." +
            "\n\n---------------------------------------------------\n" +
            "Thank you. Your health is our concern." +
            "\n---------------------------------------------------" +
            "\n\nKind Regards,\nClinic";
                // Use the SMTP settings from web.config
                var smtpClient = new SmtpClient();

                // The SmtpClient will automatically use the settings from web.config
                smtpClient.Send(email2);


                app.Status = "Declined";
                db.Entry(timeSlot).State = EntityState.Modified;
                db.Entry(app).State = EntityState.Modified;
                db.calendarEvents.Remove(calenderEvent);
                db.SaveChanges();

                var responseData = new { message = "Appointment successfully Declined, Email Sent to User." };
                return Json(responseData);
            }
            catch (Exception ex)
            {
                // Handle errors and return an error response if needed
                return Json(new { error = ex.Message });
            }
        }
        public ActionResult BookAppointment()
        {
            var availableTimeSlots = db.TimeSlots
               .Where(slot => slot.IsAvailable)
               .ToList();

            return View(availableTimeSlots);
        }
        //[HttpPost]
        //public ActionResult BookAppointment(int timeSlotId)
        //{
        //    var timeSlot = db.TimeSlots.Find(timeSlotId);
        //    if (timeSlot != null)
        //    {
        //        string startTime = timeSlot.StartTime.ToString("HH:mm");
        //        string endTime = timeSlot.EndTime.ToString("HH:mm");
        //        var userId =User.Identity.Name;
        //        var appointment = new Appointment
        //        {
        //            Name = "Sbonga",
        //            PhoneNumber = "1223",
        //            Date = DateTime.Now.Date,
        //            StartTime = startTime,
        //            EndTime = endTime,
        //            DRemail = timeSlot.DoctorEmail,
        //            Email = userId
        //        };

                
        //        db.Appointments.Add(appointment);
        //        db.SaveChanges();

        //        return RedirectToAction("BookAppointment"); 
        //    }

        //    return RedirectToAction("BookAppointment");
        //}


        // GET: Appointments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // GET: Appointments/Create
        public ActionResult Create(string ClinicName,string doctor,int Id)
        {
            var timeSlot = db.TimeSlots.Find(Id);
            Session["timeSlotId"] = Id.ToString();
            string start = timeSlot.Date.ToShortDateString() +" "+ timeSlot.StartTime.ToShortTimeString();
            string end = timeSlot.Date.ToShortDateString() +" "+ timeSlot.EndTime.ToShortTimeString();
            Appointment b = new Appointment()
            {
                StartTime = start,
                EndTime = end,
                Email = User.Identity.Name,
                Clinic = ClinicName,
                DRemail = doctor
            };
      
            return View(b);
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,StartTime,EndTime,PhoneNumber,Name,Surname,Time,Clinic,DRemail")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var Email = User.Identity.Name;

                    // Prepare email message
                    var email = new MailMessage();
                    email.From = new MailAddress("clinimanagement@outlook.com");
                    email.To.Add(Email);
                    email.Subject = "Appointment Submitted";

                    email.Body = "Dear " + appointment.Name + " " + appointment.Surname + "\n\nPlease note that your request for an appointment was submitted successfully." +
                        "\n\n---------------------------------------------------\n" +
                        "Thank you. Your health is our concern." +
                        "\n---------------------------------------------------" +
                        "\n\nKind Regards,\nClinic";

                    
                        // Use the SMTP settings from web.config
                    var smtpClient = new SmtpClient();

                        // The SmtpClient will automatically use the settings from web.config
                    smtpClient.Send(email);

                       
                       
                    
                    var callenderEvent = new CalendarEvent()
                    {
                        subject = appointment.Surname + " Appointment",
                        description = "Clinic Appointment",
                        themeColor = "red",
                        start = DateTime.Parse(appointment.StartTime),
                        end = DateTime.Parse(appointment.EndTime),
                        IsFullDay = false,
                        DrEmail = appointment.DRemail,
                        Email = appointment.Email,
                        status = "Active"
                    };

                    string timeSlotId = Session["timeSlotId"] as string;
                    int Id = int.Parse(timeSlotId);
                    var timeSlot = db.TimeSlots.Find(Id);
                    timeSlot.IsAvailable = false;
                    timeSlot.BookedBy = appointment.Email;
                    appointment.Status = "Awaiting Approval";
                    db.calendarEvents.Add(callenderEvent);
                    db.Entry(timeSlot).State = EntityState.Modified;
                    db.Appointments.Add(appointment);
                    db.SaveChanges();
                    Session["timeSlotId"] = null;
                    TempData["Notification"] = "Appointment request successfully submitted, Please check your Emails.";
                    return RedirectToAction("MyAppointments");
                }
                catch(Exception ex)
                {
                    TempData["Notification_Failure"] = "Your request could not be completed, Please try again later.";
                    return View(appointment);
                }
            }

            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,PhoneNumber,Name,Surname,Date,Time,Clinic")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Appointment appointment = db.Appointments.Find(id);
            db.Appointments.Remove(appointment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
