using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Clinic.Models
{
    public class Prescription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("Patient Email")]
        public string Email { get; set; }
        [DisplayName("Patient ID Number")]
        public string IdNumber { get; set; }
         [DisplayName("Patient Name")]
        public string Name { get; set; }
        public string Surname { get; set; }
        [DisplayName("Doctor Email")]
        public string drEmail { get; set; }
        [DisplayName("Doctor")]
        public string drName { get; set; }
        public string Initials { get; set; }
        public string Date { get; set; }
        [DisplayName("Prescribed Medicine")]
        public string Medicine { get; set; }
        public string Instructions { get; set; }
        public string Signature { get; set; }
    }
}