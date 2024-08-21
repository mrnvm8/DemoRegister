namespace DemoRegister.Models
{
    public class AttendanceViewModel
    {
        public Employee Employee { get; set; }
        public List<DailyAttendance> WeeklyAttendance { get; set; }
    }
}
