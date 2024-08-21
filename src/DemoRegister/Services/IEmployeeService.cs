using DemoRegister.Models;

namespace DemoRegister.Services;

public interface IEmployeeService
{
    List<Employee> GetAllEmployeeAsync();
    Employee GetEmployeeAsync(int employeeId);
    void SaveWeeklyAttendance(int employeeId, List<DailyAttendance> weeklyAttendance);
    List<DailyAttendance> GetWeeklyAttendance(int employeeId);
    List<List<DailyAttendance>> GroupAttendanceByWeek(List<DailyAttendance> attendances);
}
