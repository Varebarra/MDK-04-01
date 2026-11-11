using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomBooking.Cofiguration
{
    public class AppSettings
    {
        public string AppTitle { get; set; } = "Classroom Booking - рабочая конытшкрауия";
        public string WorkDayStart { get; set; } = "09:00";
        public string WorkDayEnd { get; set; } = "18:00";
        public int MinBookingMinutes { get; set; } = 60;
        public bool AllowUnavailableRooms { get; set; } = false;
        public string StatusMessage { get; set; } = "рабочий режим";
    }
}
