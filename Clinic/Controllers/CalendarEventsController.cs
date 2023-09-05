using Clinic.Models;
using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clinic.Controllers
{
    public class CalendarEventsController : Controller
    {
        // GET: CalendarEvents
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Calendar()
        {
            return View();
        }

        public JsonResult GetEvents()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //Get user role 
                //query events for dr and patient based on role
                string currentUser = User.Identity.Name;
                var events = db.calendarEvents.Where(x=>x.Email == currentUser&&x.status == "Active").ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        [HttpPost]
        public JsonResult SaveEvent(CalendarEvent e)
        {
            var status = false;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (e.Id >0)
                {
                    //Update the event
                    var v = db.calendarEvents.Where(a => a.Id == e.Id).FirstOrDefault();
                    if (v != null)
                    {
                        v.subject = e.subject;
                        v.start = e.start;
                        v.end = e.end;
                        v.description = e.description;
                        v.IsFullDay = e.IsFullDay;
                        v.themeColor = e.themeColor;
                    }
                }
                else
                {
                    db.calendarEvents.Add(e);
                }
                db.SaveChanges();
                status = true;
            }
            return new JsonResult { Data = new { status = status } };

        }
        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            var status = false;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var v = db.calendarEvents.Where(a => a.Id == eventID).FirstOrDefault();
                if (v != null)
                {
                    db.calendarEvents.Remove(v);
                    db.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }
    }
}