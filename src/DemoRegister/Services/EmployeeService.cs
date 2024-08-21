using DemoRegister.Models;
using System.Globalization;

namespace DemoRegister.Services;
public class EmployeeService : IEmployeeService
{
    // In-memory store for employee data
    public readonly List<Employee> _employees;
    // In-memory store for attendance data
    private readonly Dictionary<int, List<DailyAttendance>> _attendanceData;
   

    public EmployeeService()
    {
        _employees = new List<Employee>
            {
                new Employee
                {
                    Id = 1,
                    Name = "Lindokuhle",

                },
                 new Employee
                {
                    Id = 2,
                    Name = "Lihle",

                },
                 new Employee
                {
                    Id = 3,
                    Name = "Siphelo",
                }
            };
        _attendanceData = new Dictionary<int, List<DailyAttendance>>();
    }

    public List<Employee> GetAllEmployeeAsync()
    {
        return _employees;
    }

    public Employee GetEmployeeAsync(int employeeeId)
    {
        return _employees.FirstOrDefault(x => x.Id == employeeeId);
    }

    public void SaveWeeklyAttendance(int employeeId, List<DailyAttendance> weeklyAttendance)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == employeeId);
        if (employee != null)
        {
            // Add new weekly attendance to the employee's list
            employee.Attendances.AddRange(weeklyAttendance);
        }
        //if (_attendanceData.ContainsKey(employeeId))
        //{
        //    _attendanceData[employeeId] = weeklyAttendance;
        //}
        //else
        //{
        //    _attendanceData.Add(employeeId, weeklyAttendance);
        //}
    }

    public List<DailyAttendance> GetWeeklyAttendance(int employeeId)
    {
        return _attendanceData.ContainsKey(employeeId) ? _attendanceData[employeeId] : new List<DailyAttendance>();
    }

    public List<List<DailyAttendance>> GroupAttendanceByWeek(List<DailyAttendance> attendances)
    {
        var groupedByWeek = attendances
                        .GroupBy(a => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                            a.Date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday))
                        .OrderBy(g => g.Key)
                        .Select(g => g.OrderBy(a => a.Date).ToList())
                        .ToList();

        return groupedByWeek;
    }

    private int GetWeekNumber(DateTime date)
    {
        // Shift the date to ensure Monday is the start of the week
        DateTime shiftedDate = date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);

        // Get the start of the week for the shifted date
        DateTime startOfYear = new DateTime(date.Year, 1, 1);
        int weekNumber = (int)Math.Floor((shiftedDate - startOfYear).TotalDays / 7.0) + 1;

        return weekNumber;
    }
}
