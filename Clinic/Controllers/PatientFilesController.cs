using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;
using Clinic.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using static System.Net.WebRequestMethods;

namespace Clinic.Controllers
{
    public class PatientFilesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PatientFiles
        public ActionResult Index()
        {
          
            return View(db.PatientFiles.ToList());
        }

        public ActionResult CurrentFile(string patientEmail)
        {
            var PatientFiles = db.PatientFiles.Where(x => x.Email == patientEmail).ToList();
            return View(PatientFiles);
        }


        //Dr Must use calender to get here
        public ActionResult DrPatientFiles()
        {
            //var medicalRecord = db.MedicalRecords.Where(x=>x.)
            string currentUser = User.Identity.Name;
            var PatientFiles = db.PatientFiles.Where(x => x.drEmail == currentUser).ToList();
            return View(PatientFiles);
        }


            public ActionResult Myfile()
        {
            string currentUser = User.Identity.Name;
            return View(db.PatientFiles.Where(x => x.Email == currentUser).ToList());
        }
            
        // GET: PatientFiles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientFile patientFile = db.PatientFiles.Find(id);
            if (patientFile == null)
            {
                return HttpNotFound();
            }
            return View(patientFile);
        }


        // GET: PatientFiles/Create
        public ActionResult Create(string email, string name,string surname, string phone,string clinic)
        {
           
            PatientFile b = new PatientFile()
            {
               
                Email = email,
                Name = name,
                Surname = surname,
                PhoneNumber = phone,
                Clinic = clinic
            };
            ViewBag.Id = new SelectList(db.ClinicBranches.ToList(), "Id", "Name");
            return View();
        }

        // POST: PatientFiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PatietnId,IdNumber,Name,Surname,DateOfBirth,Gender,Email,Clinic,PhoneNumber,Address,bloodType,signature,drSignatur")] PatientFile patientFile)
        {
            if (ModelState.IsValid)
            {
                try
                {


                    MemoryStream memoryStream = new MemoryStream();
                    Document document = new Document(PageSize.A5, 0, 0, 0, 0);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();



                    // Add a title
                    Font titleFont = new Font(Font.FontFamily.HELVETICA, 24, Font.BOLD);
                    Paragraph title = new Paragraph("Patient File Record", titleFont);
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

                    PdfPCell signatureTitleCell2 = new PdfPCell();
                    signatureTitleCell2.AddElement(new Paragraph("Patient Signature"));
                    signatureTitleCell2.HorizontalAlignment = Element.ALIGN_CENTER;

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

                    table1.AddCell("Patient ID Number : \t" + patientFile.IdNumber);
                    table1.AddCell("Patient Name : \t" + patientFile.Name);
                    table1.AddCell("Surname : \t" + patientFile.Surname);

                    table2.AddCell("Phone Number : \t" + patientFile.PhoneNumber);




                    table1.AddCell("Address : \t" + patientFile.Address);


                    table1.AddCell("Date of Birth: \t" + patientFile.DateOfBirth);
                    table1.AddCell("Blood Type: \t" + patientFile.bloodType);
                    table1.AddCell("Clinic: \t" + patientFile.Clinic);
                    table1.AddCell("Gender: \t" + patientFile.Gender);

                    table1.AddCell("\n\n");
                    table1.AddCell(signatureTitleCell);





                    // Convert the signature image to a byte array
                    var cleanerBase64 = patientFile.drSignatur.Substring(22);
                    byte[] signatureBytes = Convert.FromBase64String(cleanerBase64);

                    // Create an image from the byte array
                    iTextSharp.text.Image signatureImage = iTextSharp.text.Image.GetInstance(signatureBytes);

                    // Scale the image to fit within the cell
                    signatureImage.ScaleToFit(100, 100);

                    // Add the image to the cell
                    PdfPCell signatureCell = new PdfPCell(signatureImage);
                    signatureCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    table1.AddCell(signatureCell);
                    table1.AddCell("\n\n");
                    table1.AddCell(signatureTitleCell2);

                    // Convert the signature image to a byte array
                    var CustcleanerBase64 = patientFile.signature.Substring(22);
                    byte[] CustsignatureBytes = Convert.FromBase64String(CustcleanerBase64);

                    // Create an image from the byte array
                    iTextSharp.text.Image CustsignatureImage = iTextSharp.text.Image.GetInstance(CustsignatureBytes);

                    // Scale the image to fit within the cell
                    CustsignatureImage.ScaleToFit(100, 100);

                    // Add the image to the cell
                    PdfPCell CustsignatureCell = new PdfPCell(CustsignatureImage);
                    CustsignatureCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    table1.AddCell(CustsignatureCell);

                    table1.AddCell("\n");





                    table1.AddCell(cell);
                    document.Add(table1);

                    document.Add(table3);
                    document.Close();

                    byte[] bytes = memoryStream.ToArray();
                    memoryStream.Close();


                    var attachments = new List<Attachment>();
                    attachments.Add(new Attachment(new MemoryStream(bytes), "Patient File", "application/pdf"));
                    var email = new MailMessage();
                    email.From = new MailAddress("sbonga.dev@outlook.com");
                    email.To.Add(patientFile.Email);
                    email.Subject = "Patient File Created";
                    email.Body = "Dear " + patientFile.Name + " " + patientFile.Surname + "\n\nPlease see the attached PDF for your clinic file." +
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
                    //}
                    string currentUser = User.Identity.Name;
                    patientFile.status = "Created";
                    patientFile.drEmail = currentUser;
                    db.PatientFiles.Add(patientFile);
                    db.SaveChanges();
                    // Specify the file path and name
                    string filePath = Server.MapPath("~/") + patientFile.Name + patientFile.PatietnId + ".pdf";

                    // Write the PDF bytes to the file
                    System.IO.File.WriteAllBytes(filePath, bytes);
                    TempData["FileNotification"] = "File created Successfully, pdf sent to client.";
                    return RedirectToAction("DrPatientFiles");

                }
                catch
                {
                    TempData["FileNotificationFailure"] = "Patient fie could not be created, Please try again later.";
                    return View(patientFile);
                }
            }

            
            return View(patientFile);
        }

        // GET: PatientFiles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientFile patientFile = db.PatientFiles.Find(id);
            if (patientFile == null)
            {
                return HttpNotFound();
            }
            
            return View(patientFile);
        }

        // POST: PatientFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PatietnId,Id,Name,Surname,DateOfBirth,Gender,Email,PhoneNumber,Address,bloodType,signature")] PatientFile patientFile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patientFile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
           
            return View(patientFile);
        }

        // GET: PatientFiles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientFile patientFile = db.PatientFiles.Find(id);
            if (patientFile == null)
            {
                return HttpNotFound();
            }
            return View(patientFile);
        }

        // POST: PatientFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PatientFile patientFile = db.PatientFiles.Find(id);
            db.PatientFiles.Remove(patientFile);
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
