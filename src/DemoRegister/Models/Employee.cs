namespace DemoRegister.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<DailyAttendance> Attendances { get; set; } = new List<DailyAttendance>();
    }
}
