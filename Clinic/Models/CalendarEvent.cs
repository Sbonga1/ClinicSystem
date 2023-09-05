using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Clinic.Models
{
    public class CalendarEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string subject { get; set; }
        public string description { get; set; }
        public string themeColor { get; set; }
        
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public bool IsFullDay { get; set; }
        public string DrEmail { get; set; }
        public string Email { get; set; }
        public string status { get; set; }

    }
}