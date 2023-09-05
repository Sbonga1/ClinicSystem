using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Clinic.Models
{
    public class Doctor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public string MedicalSpecialty { get; set; }
        public string Avalibiliy { get; set; }
        public double consultationFee { get; set; }
        public string picture { get; set; }
        [DisplayName("Clinic Name:")]
        public string clinic { get; set; }
    }
}