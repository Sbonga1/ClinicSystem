using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Clinic.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Web.Helpers;

namespace Clinic.Controllers
{
    public class PrescriptionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult MyPrescriptions()
        {
            string currentUser = User.Identity.Name;
            return View(db.Prescriptions.Where(x => x.Email == currentUser).ToList());
        }
        public ActionResult DrPrescriptions()
        {
            string currentUser = User.Identity.Name;
            return View(db.Prescriptions.Where(x=>x.drEmail == currentUser).ToList());
        }
        // GET: Prescriptions
        public ActionResult Index()
        {
            return View(db.Prescriptions.ToList());
        }

        // GET: Prescriptions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prescription prescription = db.Prescriptions.Find(id);
            if (prescription == null)
            {
                return HttpNotFound();
            }
            return View(prescription);
        }

        // GET: Prescriptions/Create
        public ActionResult Create(string patientEmail, string idNumber)
        {
            string currentUser = User.Identity.Name;
            var file = db.PatientFiles.Where(x => x.Email == patientEmail).FirstOrDefault();
            var dr = db.Doctors.Where(x => x.Email == currentUser).FirstOrDefault();
            string doctorName = dr.Name;
            char initial = doctorName[0];
            string doctor = initial + " " + dr.Surname;
            Prescription b = new Prescription()
            {
                Email = patientEmail,
                IdNumber = idNumber,
                Name = file.Name,
                Surname = file.Surname,
                drEmail = currentUser,
                drName = doctor,
                Date = DateTime.Now.ToString()
            };

            return View(b);
        }

        // POST: Prescriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,IdNumber,Name,Surname,drEmail,drName,Date,Medicine,Instructions,Signature")] Prescription prescription)
        {
            if (ModelState.IsValid)
            {
                
                MemoryStream memoryStream = new MemoryStream();
                Document document = new Document(PageSize.A5, 0, 0, 0, 0);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Add a title
                Font titleFont = new Font(Font.FontFamily.HELVETICA, 24, Font.BOLD);
                Paragraph title = new Paragraph("Medical Prescription", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                // Create the heading paragraph with the headig font
                PdfPTable table1 = new PdfPTable(1);
                PdfPTable table2 = new PdfPTable(5);
                PdfPTable table3 = new PdfPTable(1);

                iTextSharp.text.pdf.draw.VerticalPositionMark seperator = new iTextSharp.text.pdf.draw.LineSeparator();
                seperator.Offset = -6f;
                // Remove table cell
                table1.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                table3.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                table1.WidthPercentage = 80;
                table1.SetWidths(new float[] { 100 });
                table2.WidthPercentage = 80;
                table3.SetWidths(new float[] { 100 });
                table3.WidthPercentage = 80;

                PdfPCell subtitleCell = new PdfPCell();
                subtitleCell.AddElement(new Paragraph("Patient Details"));
                subtitleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                

                PdfPCell MedicineCell = new PdfPCell();
                subtitleCell.AddElement(new Paragraph("Prescribed Medicine"));
                subtitleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell DrsubtitleCell = new PdfPCell();
                subtitleCell.AddElement(new Paragraph("Doctor Details"));
                subtitleCell.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell signatureTitleCell = new PdfPCell();
                signatureTitleCell.AddElement(new Paragraph("Doctor Signature"));
                signatureTitleCell.HorizontalAlignment = Element.ALIGN_CENTER;



                PdfPCell cell = new PdfPCell(new Phrase(""));
                cell.Colspan = 3;
                table1.AddCell("\n");
                table1.AddCell(cell);
                table1.AddCell("\n\n");
                table1.AddCell(
                    "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t" +
                    "Clinic \n" +
                    "Email :sbonga.prg@outlook.com" + "\n" +
                    "\n" + "\n");
                table1.AddCell(subtitleCell);

                table1.AddCell("Patient Name : \t" + prescription.Name);

                table1.AddCell("Surname : \t" + prescription.Surname);

                table2.AddCell("Patient ID Number : \t" +prescription.IdNumber);
                table2.AddCell("Patient ID Number : \t" + prescription.IdNumber);

                table1.AddCell("\n\n");
                table1.AddCell(DrsubtitleCell);
                string currentUser = User.Identity.Name;
                var Doctor = db.Doctors.Where(x => x.Email == currentUser).FirstOrDefault();
                
             
                table1.AddCell("Doctor : \t" + prescription.drName);
                table1.AddCell("Email : \t" + currentUser);
                var clinic = db.ClinicBranches.Where(x => x.Name == Doctor.clinic).FirstOrDefault();
                table1.AddCell("Address :\t" + clinic.Address);

                table1.AddCell("\n\n");
                table1.AddCell(MedicineCell);
                table1.AddCell("Medicine: \t" + prescription.Medicine);
                table1.AddCell("Instructions: \t" + prescription.Instructions);
                table1.AddCell("Issued: \t" + prescription.Date);

                table1.AddCell("\n\n");
                table1.AddCell(signatureTitleCell);
                // Convert the signature image to a byte array
                var cleanerBase64 = prescription.Signature.Substring(22);
                byte[] signatureBytes = Convert.FromBase64String(cleanerBase64);

                // Create an image from the byte array
                iTextSharp.text.Image signatureImage = iTextSharp.text.Image.GetInstance(signatureBytes);

                // Scale the image to fit within the cell
                signatureImage.ScaleToFit(100, 100);

                // Add the image to the cell
                PdfPCell signatureCell = new PdfPCell(signatureImage);
                signatureCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                table1.AddCell(signatureCell);






                table1.AddCell(cell);
                document.Add(table1);

                document.Add(table3);
                document.Close();

                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();
                // Specify the file path and name
                string filePath = Server.MapPath("~/") + prescription.Email+ prescription.Id + ".pdf";

                // Write the PDF bytes to the file
                System.IO.File.WriteAllBytes(filePath, bytes);

                var attachments = new List<Attachment>();
                attachments.Add(new Attachment(new MemoryStream(bytes), "Patient File", "application/pdf"));
                var email = new MailMessage();
                email.From = new MailAddress("sbonga.dev@outlook.com");
                email.To.Add(prescription.Email);
                email.Subject = "Medical Record Created";
                email.Body = "Dear " + prescription.Name + " " + prescription.Surname + "\n\nPlease see the attached PDF for your clinic file." +
             "\n\n---------------------------------------------------\n" +
             "Thank you. Your health is our concern." +
             "\n---------------------------------------------------" +
             "\n\nKind Regards,\nClinic";
                // Attach the files to the email
                foreach (var attachment in attachments)
                {
                    email.Attachments.Add(attachment);
                }
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
                //}\


                var appointment = db.Appointments.Where(x => x.Email == prescription.Email && x.Status == "Approved").FirstOrDefault();
                appointment.Status = "Awaiting Payment";
                var calenderEvent = db.calendarEvents.Where(x => x.Email == prescription.Email && x.status == "Active").FirstOrDefault();
                db.calendarEvents.Remove(calenderEvent);
                prescription.drEmail = currentUser;
                db.Entry(appointment).State = EntityState.Modified;
                db.Prescriptions.Add(prescription);
                db.SaveChanges();
                return RedirectToAction("DrPrescriptions");
            }

            return View(prescription);
        }

        // GET: Prescriptions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prescription prescription = db.Prescriptions.Find(id);
            if (prescription == null)
            {
                return HttpNotFound();
            }
            return View(prescription);
        }

        // POST: Prescriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,IdNumber,Name,Surname,drEmail,drName,Initials,Date,Signature")] Prescription prescription)
        {
            if (ModelState.IsValid)
            {
                db.Entry(prescription).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(prescription);
        }

        // GET: Prescriptions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prescription prescription = db.Prescriptions.Find(id);
            if (prescription == null)
            {
                return HttpNotFound();
            }
            return View(prescription);
        }

        // POST: Prescriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Prescription prescription = db.Prescriptions.Find(id);
            db.Prescriptions.Remove(prescription);
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
