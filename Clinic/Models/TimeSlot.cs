using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Clinic.Models
{
    public class TimeSlot
    {
        public int Id { get; set; }
       [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }
        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public string BookedBy { get; set; }
        public string DoctorEmail { get; set; }
        public List<TimeSlot> AvailableTimeSlots { get; set; }
    }
}