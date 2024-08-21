using DemoRegister.Models;
using DemoRegister.Services;
using Microsoft.AspNetCore.Mvc;

namespace DemoRegister.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public IActionResult Index()
        {
            var employees = _employeeService.GetAllEmployeeAsync();
            return View(employees);
        }

        public IActionResult EnterAttendance(int id)
        {
            var employee = _employeeService.GetEmployeeAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            var weeklyAttendance = /*_employeeService.GetWeeklyAttendance(id);*/ new List<DailyAttendance>();

            // Initialize with 5 empty records if not available
            while (weeklyAttendance.Count < 5)
            {
                weeklyAttendance.Add(new DailyAttendance { Date = DateTime.Now, Status = new AttendanceStatus() });
            }

            // Initialize the WeeklyAttendance list with 5 DailyAttendance objects,
            // each having a Status object initialized.
            var model = new AttendanceViewModel
            {
                Employee = employee,
                //WeeklyAttendance = Enumerable.Range(0, 5).Select(_ => new DailyAttendance
                //{
                //    Date = DateTime.Now,
                //    Status = new AttendanceStatus("") // Initialize the Status property
                //}).ToList()
                WeeklyAttendance = weeklyAttendance
            };

            return View(model);
        }

        // POST: Employee/SaveAttendance
        [HttpPost]
        public IActionResult SaveAttendance(int employeeId, List<DailyAttendance> weeklyAttendance)
        {
            if (employeeId == 0 || weeklyAttendance == null || !weeklyAttendance.Any())
            {
                // Handle the error (e.g., redirect to an error page or show a validation message)
                return BadRequest("Invalid data.");
            }

            var employee = _employeeService.GetEmployeeAsync(employeeId);
            if (employee == null)
            {
                return NotFound();
            }

            _employeeService.SaveWeeklyAttendance(employeeId, weeklyAttendance);
            return RedirectToAction("Index");
        }

        // GET: Employee/ViewAttendance/1
        public IActionResult ViewAttendance(int id)
        {
            var employee = _employeeService.GetEmployeeAsync(id);
            if(employee is null)
            {
                return NotFound();
            }
            var weeklyAttendance = _employeeService.GetWeeklyAttendance(id);

            var model = new AttendanceViewModel
            {
                Employee = employee,
                WeeklyAttendance = weeklyAttendance
            };

            return View(model);
        }
        [HttpGet]
        public IActionResult ViewGroupedAttendance(int employeeId)
        {
            var employee = _employeeService.GetEmployeeAsync(employeeId);
            if (employee == null)
            {
                return NotFound();
            }

            var groupedAttendance = _employeeService.GroupAttendanceByWeek(employee.Attendances);

            var model = new GroupedAttendanceViewModel
            {
                Employee = employee,
                GroupedAttendance = groupedAttendance
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult EditWeeklyAttendance(int employeeId, DateTime weekStartDate)
        {
            var employee = _employeeService.GetEmployeeAsync(employeeId);
            if (employee == null)
            {
                return NotFound();
            }

            // Get attendance records for the specified week
            var weeklyAttendance = employee.Attendances
                .Where(a => a.Date >= weekStartDate && a.Date <= weekStartDate.AddDays(4))
                .OrderBy(a => a.Date)
                .ToList();

            if (weeklyAttendance == null || weeklyAttendance.Count == 0)
            {
                // Initialize a new week if none exists
                for (int i = 0; i < 5; i++)
                {
                    weeklyAttendance.Add(new DailyAttendance { Date = weekStartDate.AddDays(i) });
                }
            }

            var model = new WeeklyAttendanceViewModel
            {
                EmployeeId = employeeId,
                WeekStartDate = weekStartDate,
                WeeklyAttendance = weeklyAttendance
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult SaveWeeklyAttendance(int employeeId, DateTime weekStartDate, List<DailyAttendance> weeklyAttendance)
        {
            var employee = _employeeService.GetEmployeeAsync(employeeId);
            if (employee == null)
            {
                return NotFound();
            }

            // Remove existing records for this week
            employee.Attendances.RemoveAll(a => a.Date >= weekStartDate && a.Date <= weekStartDate.AddDays(4));

            // Add the updated weekly attendance
            employee.Attendances.AddRange(weeklyAttendance);

            return RedirectToAction("ViewGroupedAttendance", new { employeeId });
        }

        public IActionResult DeleteWeeklyAttendance(int employeeId, DateTime weekStartDate)
        {
            var employee = _employeeService.GetEmployeeAsync(employeeId);
            if (employee == null)
            {
                return NotFound();
            }

            // Find and remove attendance records for the specified week
            employee.Attendances.RemoveAll(a => a.Date >= weekStartDate && a.Date <= weekStartDate.AddDays(4));

            return RedirectToAction("ViewGroupedAttendance", new { employeeId });
        }

    }
}
