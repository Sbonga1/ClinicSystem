using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Clinic.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PayPal.Api;

namespace Clinic.Controllers
{
    public class DoctorAppointmentsController : Controller
    {
       
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DoctorAppointments
        public ActionResult Index()
        {
            return View(db.DoctorAppointments.ToList());
        }




        // GET: DoctorAppointments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorAppointment doctorAppointment = db.DoctorAppointments.Find(id);
            if (doctorAppointment == null)
            {
                return HttpNotFound();
            }
            return View(doctorAppointment);
        }

        // GET: DoctorAppointments/Create
        public ActionResult Create(string email,string ClinicName)
        {
            ViewBag.Name = new SelectList(db.Doctors.Where(x=>x.clinic == ClinicName), "Email", "Email");
            DoctorAppointment b = new DoctorAppointment()
            {
                patientEmail = email ,
             
            };
            return View(b);
        }

        // POST: DoctorAppointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,patientEmail,DoctorEmail")] DoctorAppointment doctorAppointment)
        {
            if (ModelState.IsValid)
            {

                string name = (from x in db.Appointments
                               where x.Email == doctorAppointment.patientEmail && x.Status == "Awaiting Approval"
                               select x.Name).FirstOrDefault();

                string surname = (from x in db.Appointments
                               where x.Email == doctorAppointment.patientEmail && x.Status == "Awaiting Approval"
                               select x.Surname).FirstOrDefault();

                // Prepare email message
                var email = new MailMessage();
                email.From = new MailAddress("sbonga.dev@outlook.com");
                email.To.Add(doctorAppointment.patientEmail);
                email.Subject = "Appointment Approved";
                email.Body = "Dear " + name + " " + surname + "\n\n Please note that Your appointment has been approved." +
             "\n\n---------------------------------------------------\n" +
             "Thank you. Your health is our concern." +
             "\n---------------------------------------------------" +
             "\n\nKind Regards,\nClinic";

                // Configure SMTP client
                var smtpClient = new SmtpClient("smtp.office365.com", 587);
                smtpClient.Credentials = new NetworkCredential("sbonga.dev@outlook.com", "Sbonga@01");
                smtpClient.EnableSsl = true;

                //try
                //{
                // Send email
                smtpClient.Send(email);
                //    ViewBag.Message = "Email sent successfully.";
                ////}
                //catch (Exception ex)
                //{
                // Handle exception
                //ViewBag.Message = "Error sending email: " + ex.Message;
                //}





                var Appointment = db.Appointments.Where(x=>x.Email==doctorAppointment.patientEmail&& x.Status == "Awaiting Approval").FirstOrDefault();
                Appointment.DRemail = doctorAppointment.DoctorEmail;
                Appointment.Status = "Approved";
                db.Entry(Appointment).State = EntityState.Modified;
                db.DoctorAppointments.Add(doctorAppointment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(doctorAppointment);
        }

        // GET: DoctorAppointments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorAppointment doctorAppointment = db.DoctorAppointments.Find(id);
            if (doctorAppointment == null)
            {
                return HttpNotFound();
            }
            return View(doctorAppointment);
        }

        // POST: DoctorAppointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,patientEmail,DoctorEmail")] DoctorAppointment doctorAppointment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doctorAppointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctorAppointment);
        }

        // GET: DoctorAppointments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorAppointment doctorAppointment = db.DoctorAppointments.Find(id);
            if (doctorAppointment == null)
            {
                return HttpNotFound();
            }
            return View(doctorAppointment);
        }

        // POST: DoctorAppointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DoctorAppointment doctorAppointment = db.DoctorAppointments.Find(id);
            db.DoctorAppointments.Remove(doctorAppointment);
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
