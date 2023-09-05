using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace Clinic.Models
{
    public class MedicalRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string IdNumber { get; set; }
        [DisplayName("Patient Email")]
        public string Email { get; set; }
        [DisplayName("Blood Pressure")]
        public string bloodPressure { get; set; }
        [DisplayName("Heart Rate")]
        public string heartRate { get; set; }
        public string Temperature { get; set; }
        public string Diagnosis { get; set; }
        [DisplayName("Date Created")]
        public string DateCrated { get; set; }
        public string Signature { get; set; }
        public string drEmail { get; set; }
        public string status { get; set; }
    }
}