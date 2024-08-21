namespace DemoRegister.Models;

public class GroupedAttendanceViewModel
{
    public Employee Employee { get; set; }
    public List<List<DailyAttendance>> GroupedAttendance { get; set; }
}
