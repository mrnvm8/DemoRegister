namespace DemoRegister.Models
{
    public class DailyAttendance
    {
        public DateTime Date { get; set; }
        public TimeSpan? TimeIn { get; set; }
        public TimeSpan? TimeOut { get; set; }
        public AttendanceStatus Status { get; set; }
    }
}
