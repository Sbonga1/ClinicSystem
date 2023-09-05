using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clinic.Models;

namespace Clinic.Controllers
{
    public class DoctorsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Doctors
        public ActionResult Index()
        {
            return View(db.Doctors.ToList());
        }

        public ActionResult ClinicDr(string ClinicName)
        {
            var doctors = db.Doctors
                            .Where(x => x.clinic == ClinicName)
                            .ToList();

            var timeSlotsDictionary = db.TimeSlots
                                        .Where(x => x.IsAvailable == true)
                                        .GroupBy(x => x.DoctorEmail)
                                        .ToDictionary(x => x.Key, x => x.ToList());

            var combinedData = doctors
                .Select(d => new DoctorTimeslotsViewModel
                {
                    Doctor = d,
                    TimeSlot = new TimeSlot // Initialize TimeSlot
                    {
                        AvailableTimeSlots = timeSlotsDictionary.ContainsKey(d.Email) ? timeSlotsDictionary[d.Email] : new List<TimeSlot>()
                    }
                })
                .ToList();

            return View(combinedData);
        }

        public ActionResult MyProfile()
        {
            string currentuser = User.Identity.Name;
            var profile = (from x in db.Doctors
                           where x.Email == currentuser
                           select x).ToList();
            return View(profile);
        }
        

        // GET: Doctors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // GET: Doctors/Create
        public ActionResult Create(string ClinicName)
        {
            string CurrentUser = User.Identity.Name;
            Doctor b = new Doctor()
            {
                clinic = ClinicName,
                Email = CurrentUser
            };
            return View(b);
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,Name,Surname,PhoneNumber,MedicalSpecialty,Avalibiliy,consultationFee,picture,clinic")] Doctor doctor, HttpPostedFileBase pictureFile)
        {
            if (ModelState.IsValid)
            {
                // Save the picture file on the server
                string pictureFileName = Guid.NewGuid().ToString() + Path.GetExtension(pictureFile.FileName);
                string picturePath = Path.Combine(Server.MapPath("~/"), pictureFileName);
                pictureFile.SaveAs(picturePath); 

                // Set the picture path in the record
                doctor.picture = pictureFileName;
                doctor.Avalibiliy = "Available";

                db.Doctors.Add(doctor);
                db.SaveChanges();
                return RedirectToAction("MyProfile");
            }

            return View(doctor);
        }

        // GET: Doctors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,Name,Surname,PhoneNumber,MedicalSpecialty,Avalibiliy,picture")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doctor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctor);
        }

        // GET: Doctors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Doctor doctor = db.Doctors.Find(id);
            db.Doctors.Remove(doctor);
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
