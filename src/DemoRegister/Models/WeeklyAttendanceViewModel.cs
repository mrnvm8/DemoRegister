namespace DemoRegister.Models;

public class WeeklyAttendanceViewModel
{
    public int EmployeeId { get; set; }        // The ID of the employee whose attendance is being edited
    public DateTime WeekStartDate { get; set; } // The start date of the week being edited
    public List<DailyAttendance> WeeklyAttendance { get; set; } // The list of daily attendance records for the week

    // Constructor to initialize the WeeklyAttendance list
    public WeeklyAttendanceViewModel()
    {
        WeeklyAttendance = new List<DailyAttendance>();
    }
}
