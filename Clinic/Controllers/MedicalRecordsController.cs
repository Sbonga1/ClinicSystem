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

namespace Clinic.Controllers
{
    public class MedicalRecordsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MedicalRecords
        public ActionResult Index()
        {
            return View(db.MedicalRecords.ToList());
        }
        public ActionResult MyMedicalRecord()
        {
            string currentUser = User.Identity.Name;
            return View(db.MedicalRecords.Where(x => x.Email == currentUser).ToList());
        }

        public ActionResult DrMedicalRecords()
        {
            string currentUser = User.Identity.Name;
            return View(db.MedicalRecords.Where(x => x.drEmail == currentUser).ToList());
        }


            // GET: MedicalRecords/Details/5
            public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicalRecord medicalRecord = db.MedicalRecords.Find(id);
            if (medicalRecord == null)
            {
                return HttpNotFound();
            }
            return View(medicalRecord);
        }

        // GET: MedicalRecords/Create
        public ActionResult Create(string email, string idNumber)
        {
            MedicalRecord b = new MedicalRecord()
            {
                DateCrated = DateTime.Now.Date.ToShortDateString(),
                Email = email,
                IdNumber = idNumber
            };
            return View(b);
        }

        // POST: MedicalRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdNumber,Email,bloodPressure,heartRate,Temperature,Diagnosis,DateCrated,Signature")] MedicalRecord medicalRecord)
        {
            if (ModelState.IsValid)
            {
                string Name = (from x in db.PatientFiles
                               where x.Email == medicalRecord.Email
                               select x.Name).FirstOrDefault();
                string surname = (from x in db.PatientFiles
                               where x.Email == medicalRecord.Email
                               select x.Surname).FirstOrDefault();

                MemoryStream memoryStream = new MemoryStream();
                Document document = new Document(PageSize.A5, 0, 0, 0, 0);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();



                // Add a title
                Font titleFont = new Font(Font.FontFamily.HELVETICA, 24, Font.BOLD);
                Paragraph title = new Paragraph("Medical Record", titleFont);
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
                table1.AddCell("Patient ID Number : \t" + medicalRecord.IdNumber);

                table1.AddCell("Patient Name : \t" + Name);

                table1.AddCell("Surname : \t" + surname);

                table2.AddCell("Patient Email : \t" + medicalRecord.Email);

                


                table1.AddCell("Blood Pressure : \t" + medicalRecord.bloodPressure);


                table1.AddCell("Heart Rate: \t" + medicalRecord.heartRate);
                table1.AddCell("Temperature: \t" + medicalRecord.Temperature);
                table1.AddCell("Diagnosis: \t" + medicalRecord.Diagnosis);
                table1.AddCell("Date: \t" + medicalRecord.DateCrated);

                table1.AddCell("\n\n");

                table1.AddCell(signatureTitleCell);





                // Convert the signature image to a byte array
                var cleanerBase64 = medicalRecord.Signature.Substring(22);
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
                string filePath = Server.MapPath("~/") + medicalRecord.Email + ".pdf";

                // Write the PDF bytes to the file
                System.IO.File.WriteAllBytes(filePath, bytes);

                var attachments = new List<Attachment>();
                attachments.Add(new Attachment(new MemoryStream(bytes), "Patient File", "application/pdf"));
                var email = new MailMessage();
                email.From = new MailAddress("sbonga.dev@outlook.com");
                email.To.Add(medicalRecord.Email);
                email.Subject = "Medical Record Created";
                email.Body = "Dear " + Name + " " + surname + "\n\nPlease see the attached PDF for your clinic file." +
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
                
                medicalRecord.status = "Created";
                string currentUser = User.Identity.Name;
                medicalRecord.drEmail = currentUser;
                var patientFile = db.PatientFiles.Where(x => x.status == "Created" && x.drEmail == currentUser).FirstOrDefault();
                patientFile.status = "Settled";
                db.Entry(patientFile).State = EntityState.Modified;
                db.MedicalRecords.Add(medicalRecord);
                db.SaveChanges();
                return RedirectToAction("DrMedicalRecords");
            }

            return View(medicalRecord);
        }

        // GET: MedicalRecords/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicalRecord medicalRecord = db.MedicalRecords.Find(id);
            if (medicalRecord == null)
            {
                return HttpNotFound();
            }
            return View(medicalRecord);
        }

        // POST: MedicalRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,bloodPressure,heartRate,Temperature,Diagnosis,DateCrated,Signature")] MedicalRecord medicalRecord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(medicalRecord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(medicalRecord);
        }

        // GET: MedicalRecords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicalRecord medicalRecord = db.MedicalRecords.Find(id);
            if (medicalRecord == null)
            {
                return HttpNotFound();
            }
            return View(medicalRecord);
        }

        // POST: MedicalRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MedicalRecord medicalRecord = db.MedicalRecords.Find(id);
            db.MedicalRecords.Remove(medicalRecord);
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
