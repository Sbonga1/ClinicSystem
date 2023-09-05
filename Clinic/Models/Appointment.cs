using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace Clinic.Models
{
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Email { get; set; }
       
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }


        //public DateTime Time { get; set; }
        [DisplayName("Start")]
        public string StartTime { get; set; }
        [DisplayName("End")]
        public string EndTime { get; set; }
        public string Clinic { get; set; }
        public string DRemail { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Status2 { get; set; }
        public string SelectedRating { get; set; }

    }
}