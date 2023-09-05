using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models
{
    public class PatientFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatietnId { get; set; }
        public string IdNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        [DataType(DataType.Date)]
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Clinic { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string bloodType { get; set; }
        public string signature { get; set; }
        public string drSignatur { get; set; }
        public string drEmail { get; set; }
        public string status { get; set; }
    }
}