namespace Clinic_Management_System.Models
{
    public class AppointmentCalendarDto
    {
        public int id { get; set; }
        public string title { get; set; } = null!;
        public string start { get; set; } = null!;
        public string end { get; set; } = null!;
        public bool isAttended { get; set; }
        public bool isCanceled { get; set; }
        public string receptionist { get; set; } = null!;
        public string description { get; set; } = null!;
        public string color { get; set; } = null!;
    }
}