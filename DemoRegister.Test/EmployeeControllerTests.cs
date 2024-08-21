using DemoRegister.Controllers;
using DemoRegister.Models;
using DemoRegister.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace DemoRegister.Test;

public class EmployeeControllerTests
{
    private readonly EmployeeController _sut;
    private readonly IEmployeeService _employeeService = Substitute.For<IEmployeeService>();

    public EmployeeControllerTests()
    {
        _sut = new EmployeeController(_employeeService);
    }

    #region Testing Empty Employee List
    [Fact]
    public void GetAllEmployee_ShouldReturnEmptyList_WhenNoUserExist()
    {
        //Arrange
        _employeeService.GetAllEmployeeAsync().Returns(new List<Employee>());

        //Act
        var result = _sut.Index();

        //Assert
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeAssignableTo<List<Employee>>()
            .Which.Should().BeEmpty();
    }
    #endregion

    #region Testing Employee List
    [Fact]
    public void GetAllEmployee_ShouldReturnEmployeesList_WhenNoUserExist()
    {
        //Arrange
        var employee = new Employee
        {
            Id = 1,
            Name = "Unit Test"
        };

        var employees = new List<Employee> { employee };
        _employeeService.GetAllEmployeeAsync().Returns(employees);

        //Act 
        var result = _sut.Index();

        //Assert
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeAssignableTo<List<Employee>>()
            .Which.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(employees.First());
    }
    #endregion

    #region Testing Get Employee by Id (is Null)
    [Fact]
    public void EnterAttendance_ShouldReturnNotFound_WhenEmployeeIsNull()
    {
        //Arrange
        int employeeId = 1;
        _employeeService.GetEmployeeAsync(employeeId).Returns((Employee)null);

        //Act
        var result = _sut.EnterAttendance(employeeId);

        //Assert
        result.Should().BeOfType<NotFoundResult>();
    }
    #endregion

    #region Testing Get Employee by Id (is Null)
    [Fact]
    public void EnterAttendance_ShouldReturnEmployee_WhenEmployeeIsNotNull()
    {
        // Arrange
        int employeeId = 1;
        var employee = new Employee { Id = employeeId, Name = "Lindokuhle" };
        _employeeService.GetEmployeeAsync(employeeId).Returns(employee);

        // Act
        var result = _sut.EnterAttendance(employeeId);

        //Assert
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeAssignableTo<AttendanceViewModel>()
            .Which.Employee.Should().Be(employee);

        var model = ((ViewResult)result).Model as AttendanceViewModel;
        model.WeeklyAttendance.Should().HaveCount(5);
        model.WeeklyAttendance.All(x => x.Status != null).Should().BeTrue();

    }
    #endregion

    #region Testing when save Attandance
    [Fact]
    public void SaveAttendance_ShouldReturnBadRequest_WhenEmployeeIdIsZero()
    {
        //Act 
        var result = _sut.SaveAttendance(0, new List<DailyAttendance>());

        //Assert
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("Invalid data.");
    }

    [Fact]
    public void SaveAttendance_ShouldReturnBadRequest_WhenWeeklyAttendanceIsNull()
    {
        // Act
        var result = _sut.SaveAttendance(1, null);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
              .Which.Value.Should().Be("Invalid data.");
    }

    [Fact]
    public void SaveAttendance_ShouldReturnBadRequest_WhenWeeklyAttendanceIsEmpty()
    {
        // Act
        var result = _sut.SaveAttendance(1, new List<DailyAttendance>());

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
              .Which.Value.Should().Be("Invalid data.");
    }

    [Fact]
    public void SaveAttendance_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        _employeeService.GetEmployeeAsync(Arg.Any<int>()).Returns((Employee)null);

        // Act
        var result = _sut.SaveAttendance(1, new List<DailyAttendance> { new DailyAttendance() });

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void SaveAttendance_ShouldRedirectToIndex_WhenDataIsValid()
    {
        // Arrange
        var employee = new Employee();
        _employeeService.GetEmployeeAsync(Arg.Any<int>()).Returns(employee);

        // Act
        var result = _sut.SaveAttendance(1, new List<DailyAttendance> { new DailyAttendance() });

        // Assert
        _employeeService.Received(1).SaveWeeklyAttendance(1, Arg.Any<List<DailyAttendance>>());
        result.Should().BeOfType<RedirectToActionResult>()
              .Which.ActionName.Should().Be("Index");
    }
    #endregion

    #region Testing View Attandance
    [Fact]
    public void ViewAttendance_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        int employeeId = 1;

        // Return null when GetEmployeeAsync is called with the given employeeId
        _employeeService.GetEmployeeAsync(employeeId).Returns((Employee)null);

        // Act
        var result = _sut.ViewAttendance(employeeId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void ViewAttendance_ShouldReturnViewResult_WithCorrectModel_WhenEmployeeExists()
    {
        // Arrange
        int employeeId = 1;
        var employee = new Employee { Id = employeeId, Name = "Lindokuhle" };
        var weeklyAttendance = new List<DailyAttendance>
        {
            new DailyAttendance { Date = DateTime.Today, TimeIn = TimeSpan.FromHours(9), TimeOut = TimeSpan.FromHours(17) },
            new DailyAttendance { Date = DateTime.Now.AddDays(1), TimeIn = TimeSpan.FromHours(9), TimeOut = TimeSpan.FromHours(17)},
        };

        _employeeService.GetEmployeeAsync(employeeId).Returns(employee);
        _employeeService.GetWeeklyAttendance(employeeId).Returns(weeklyAttendance);

        // Act
        var result = _sut.ViewAttendance(employeeId);

        // Assert
        //var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        //var model = viewResult.Model.Should().BeOfType<AttendanceViewModel>().Subject;

        //model.Employee.Should().Be(employee);
        //model.WeeklyAttendance.Should().BeEquivalentTo(weeklyAttendance);
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeAssignableTo<AttendanceViewModel>()
            .Which.Employee.Should().Be(employee);

        var model = ((ViewResult)result).Model as AttendanceViewModel;
        model.WeeklyAttendance.Should().BeEquivalentTo(weeklyAttendance);
    }
    #endregion

    #region ViewGroupedAttendance
    [Fact]
    public void ViewGroupedAttendance_Should_ReturnNotFound_WhenEmployeeDoesNotExist()
    {
        //Arrange
        int employeeId = 1;
        _employeeService.GetEmployeeAsync(employeeId).Returns((Employee)null);

        //Act
        var result = _sut.ViewGroupedAttendance(employeeId);

        //Assert
        result.Should().BeOfType<NotFoundResult>();
       
    }

    [Fact]
    public void ViewGroupedAttendance_Should_ReturnViewWithModel_WhenEmployeeExists()
    {
        // Arrange
        int employeeId = 1;
        var employee = new Employee { Id = employeeId, Attendances = GetMockAttendances() };
        var groupedAttendance = new List<List<DailyAttendance>>
        {
            new List<DailyAttendance> // Week 1
            {
                new DailyAttendance { Date = new DateTime(2024, 8, 5), Status = new AttendanceStatus() },
                new DailyAttendance { Date = new DateTime(2024, 8, 6), Status = new AttendanceStatus() }
            },
            new List<DailyAttendance> // Week 2
            {
                new DailyAttendance { Date = new DateTime(2024, 8, 12), Status = new AttendanceStatus() },
                new DailyAttendance { Date = new DateTime(2024, 8, 13), Status = new AttendanceStatus() }
            }
        };

        _employeeService.GetEmployeeAsync(employeeId).Returns(employee);
        _employeeService.GroupAttendanceByWeek(employee.Attendances).Returns(groupedAttendance);

        // Act
        var result = _sut.ViewGroupedAttendance(employeeId);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().BeOfType<GroupedAttendanceViewModel>();

        var model = viewResult.Model as GroupedAttendanceViewModel;
        model.Should().NotBeNull();
        model.Employee.Should().Be(employee);
        model.GroupedAttendance.Should().BeSameAs(groupedAttendance);
    }

    private List<DailyAttendance> GetMockAttendances()
    {
        return new List<DailyAttendance>
        {
            new DailyAttendance { Date = new DateTime(2024, 8, 5), Status = new AttendanceStatus() },
            new DailyAttendance { Date = new DateTime(2024, 8, 6), Status = new AttendanceStatus() },
            new DailyAttendance { Date = new DateTime(2024, 8, 12), Status = new AttendanceStatus() },
            new DailyAttendance { Date = new DateTime(2024, 8, 13), Status = new AttendanceStatus() }
        };
    }
    #endregion

    #region Edit Weekly Attendance
    [Fact]
    public void WeeklyAttendance_ShouldInitializeNewWeek_WhenNoRecordsExist()
    {
        // Arrange
        int employeeId = 1;
        DateTime weekStartDate = new DateTime(2024, 8, 5); // Monday, August 5th, 2024
        var employee = new Employee
        {
            Id = employeeId,
            Attendances = new List<DailyAttendance>()
        };

        _employeeService.GetEmployeeAsync(employeeId).Returns(employee);

        // Act
        var result =  _sut.EditWeeklyAttendance(employeeId, weekStartDate);

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();

        var model = viewResult.Model as WeeklyAttendanceViewModel;
        model.Should().NotBeNull();
        model.WeeklyAttendance.Should().HaveCount(5);
        model.WeeklyAttendance.All(a => a.Date >= weekStartDate && a.Date <= weekStartDate.AddDays(4)).Should().BeTrue();
    }

    [Fact]
    public void EditWeeklyAttendance_ShouldReturnViewWithExistingRecords_WhenRecordsExistForGivenWeek()
    {
        // Arrange
        var weekStartDate = new DateTime(2024, 8, 19); // Example: week starting on August 19, 2024
        var existingAttendance = new List<DailyAttendance>
        {
            new DailyAttendance { Date = weekStartDate, TimeIn = new TimeSpan(8, 0, 0), TimeOut = new TimeSpan(17, 0, 0) }, // Monday
            new DailyAttendance { Date = weekStartDate.AddDays(1), TimeIn = new TimeSpan(8, 0, 0), TimeOut = new TimeSpan(17, 0, 0) }, // Tuesday
            new DailyAttendance { Date = weekStartDate.AddDays(2), TimeIn = new TimeSpan(8, 0, 0), TimeOut = new TimeSpan(17, 0, 0) }, // Wednesday
            new DailyAttendance { Date = weekStartDate.AddDays(3), TimeIn = new TimeSpan(8, 0, 0), TimeOut = new TimeSpan(17, 0, 0) }, // Thursday
            new DailyAttendance { Date = weekStartDate.AddDays(4), TimeIn = new TimeSpan(8, 0, 0), TimeOut = new TimeSpan(17, 0, 0) }  // Friday
        };
        var employeeId = 1;
        var employee = new Employee { Id = employeeId, Attendances = existingAttendance };
        _employeeService.GetEmployeeAsync(employeeId).Returns(employee);

        // Act
        var result = _sut.EditWeeklyAttendance(employeeId, weekStartDate);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeAssignableTo<WeeklyAttendanceViewModel>().Subject;

        model.EmployeeId.Should().Be(employeeId);
        model.WeekStartDate.Should().Be(weekStartDate);
        model.WeeklyAttendance.Should().BeEquivalentTo(existingAttendance, options => options.WithStrictOrdering());
    }
    #endregion

    #region Save Weekly Attendance
    [Fact]
    public void SaveWeeklyAttendance_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        var employeeId = 1;
        _employeeService.GetEmployeeAsync(employeeId).Returns((Employee)null);

        // Act
        var result = _sut.SaveWeeklyAttendance(employeeId, DateTime.Today, new List<DailyAttendance>());

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void SaveWeeklyAttendance_ShouldRemoveExistingRecords_ForGivenWeek()
    {
        // Arrange
        var employeeId = 1;
        var weekStartDate = new DateTime(2024, 8, 19); // Example: week starting on August 19, 2024
        var existingAttendance = new List<DailyAttendance>
        {
            new DailyAttendance { Date = weekStartDate, TimeIn = new TimeSpan(9, 0, 0), TimeOut = new TimeSpan(17, 0, 0) }, // Monday
            new DailyAttendance { Date = weekStartDate.AddDays(1), TimeIn = new TimeSpan(9, 0, 0), TimeOut = new TimeSpan(17, 0, 0) }, // Tuesday
        };
        var employee = new Employee { Id = employeeId, Attendances = existingAttendance };
        _employeeService.GetEmployeeAsync(employeeId).Returns(employee);

        var newWeeklyAttendance = new List<DailyAttendance>
        {
            new DailyAttendance { Date = weekStartDate, TimeIn = new TimeSpan(8, 30, 0), TimeOut = new TimeSpan(17, 30, 0) }, // Updated Monday
            new DailyAttendance { Date = weekStartDate.AddDays(2), TimeIn = new TimeSpan(8, 30, 0), TimeOut = new TimeSpan(17, 30, 0) }, // New Wednesday
        };

        // Act
        _sut.SaveWeeklyAttendance(employeeId, weekStartDate, newWeeklyAttendance);

        // Assert
        employee.Attendances.Should().NotContain(a => a.Date == weekStartDate.AddDays(1)); // Ensure Tuesday's record is removed
        employee.Attendances.Should().Contain(newWeeklyAttendance); // Ensure new records are added
    }

    [Fact]
    public void SaveWeeklyAttendance_ShouldRedirectToViewGroupedAttendance_WhenSuccessful()
    {
        // Arrange
        var employeeId = 1;
        var employee = new Employee { Id = employeeId, Attendances = new List<DailyAttendance>() };
        _employeeService.GetEmployeeAsync(employeeId).Returns(employee);

        var weekStartDate = DateTime.Today;
        var weeklyAttendance = new List<DailyAttendance>
        {
            new DailyAttendance { Date = weekStartDate, TimeIn = new TimeSpan(9, 0, 0), TimeOut = new TimeSpan(17, 0, 0) },
        };

        // Act
        var result = _sut.SaveWeeklyAttendance(employeeId, weekStartDate, weeklyAttendance);

        // Assert
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("ViewGroupedAttendance");
        redirectResult.RouteValues["employeeId"].Should().Be(1);
    }
    #endregion

    #region Delete Weekly Attendance
    [Fact]
    public void DeleteWeeklyAttendance_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        int employeeId = 1;
        _employeeService.GetEmployeeAsync(employeeId).Returns((Employee)null);

        // Act
        var result = _sut.DeleteWeeklyAttendance(employeeId, DateTime.Today);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void DeleteWeeklyAttendance_ShouldRemoveAttendanceRecords_ForGivenWeek()
    {
        // Arrange
        int employeeId = 1;
        var weekStartDate = new DateTime(2024, 8, 19); // Example: week starting on August 19, 2024
        var existingAttendance = new List<DailyAttendance>
        {
            new DailyAttendance { Date = weekStartDate, TimeIn = new TimeSpan(9, 0, 0), TimeOut = new TimeSpan(17, 0, 0) }, // Monday
            new DailyAttendance { Date = weekStartDate.AddDays(1), TimeIn = new TimeSpan(9, 0, 0), TimeOut = new TimeSpan(17, 0, 0) }, // Tuesday
            new DailyAttendance { Date = weekStartDate.AddDays(5), TimeIn = new TimeSpan(9, 0, 0), TimeOut = new TimeSpan(17, 0, 0) }  // Following Monday
        };
        var employee = new Employee { Id = employeeId, Attendances = existingAttendance };
        _employeeService.GetEmployeeAsync(employeeId).Returns(employee);

        // Act
        _sut.DeleteWeeklyAttendance(1, weekStartDate);

        // Assert
        employee.Attendances.Should().NotContain(a => a.Date >= weekStartDate && a.Date <= weekStartDate.AddDays(4)); // Ensure records within the week are removed
        employee.Attendances.Should().Contain(a => a.Date == weekStartDate.AddDays(5)); // Ensure records outside the week are not removed
    }

    [Fact]
    public void DeleteWeeklyAttendance_ShouldRedirectToViewGroupedAttendance_WhenSuccessful()
    {
        // Arrange
        int employeeId = 1;
        var employee = new Employee { Id = employeeId, Attendances = new List<DailyAttendance>() };
        _employeeService.GetEmployeeAsync(employeeId).Returns(employee);

        var weekStartDate = DateTime.Today;

        // Act
        var result = _sut.DeleteWeeklyAttendance(employeeId, weekStartDate);

        // Assert
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("ViewGroupedAttendance");
        redirectResult.RouteValues["employeeId"].Should().Be(employeeId);
    }
    #endregion
}
