using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clinic.Models;

namespace Clinic.Controllers
{
    public class ClinicBranchesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ClinicBranches
        public ActionResult Index()
        {
            return View(db.ClinicBranches.ToList());
        }

        // GET: ClinicBranches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClinicBranch clinicBranch = db.ClinicBranches.Find(id);
            if (clinicBranch == null)
            {
                return HttpNotFound();
            }
            return View(clinicBranch);
        }

        // GET: ClinicBranches/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClinicBranches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Address,City,Province,ZipCode,Phone,Email,Picture")] ClinicBranch clinicBranch, HttpPostedFileBase pictureFile)
        {
            if (ModelState.IsValid)
            {
                
                    // Save the picture file on the server
                    string pictureFileName = Guid.NewGuid().ToString() + Path.GetExtension(pictureFile.FileName);
                    string picturePath = Path.Combine(Server.MapPath("~/"), pictureFileName);
                    pictureFile.SaveAs(picturePath);

                    // Set the picture path in the record
                    clinicBranch.Picture = pictureFileName;
                
                db.ClinicBranches.Add(clinicBranch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(clinicBranch);
        }

        // GET: ClinicBranches/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClinicBranch clinicBranch = db.ClinicBranches.Find(id);
            if (clinicBranch == null)
            {
                return HttpNotFound();
            }
            return View(clinicBranch);
        }

        // POST: ClinicBranches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Address,City,Province,ZipCode,Phone,Email,Picture")] ClinicBranch clinicBranch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clinicBranch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clinicBranch);
        }

        // GET: ClinicBranches/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClinicBranch clinicBranch = db.ClinicBranches.Find(id);
            if (clinicBranch == null)
            {
                return HttpNotFound();
            }
            return View(clinicBranch);
        }

        // POST: ClinicBranches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClinicBranch clinicBranch = db.ClinicBranches.Find(id);
            db.ClinicBranches.Remove(clinicBranch);
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
