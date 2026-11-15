using EmployeeService.Models;
using EmployeeService.Messages;
using EmployeeService.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private static readonly List<Employee> employees = new()
    {
        new Employee { Id = 1, Name = "Ahmed", Email = "ahmed@test.com", DepartmentId = 1 },
        new Employee { Id = 2, Name = "Omar", Email = "omar@test.com", DepartmentId = 2 }
    };

    private readonly RabbitMQPublisher _rabbitMQPublisher;

    public EmployeesController(RabbitMQPublisher rabbitMQPublisher)
    {
        _rabbitMQPublisher = rabbitMQPublisher;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(employees);

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var employee = employees.FirstOrDefault(e => e.Id == id);
        return employee == null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Employee employee)
    {
        employee.Id = employees.Max(e => e.Id) + 1;
        employees.Add(employee);

        var employeeEvent = new EmployeeCreatedEvent
        {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.Email,
            DepartmentId = employee.DepartmentId
        };

        await _rabbitMQPublisher.PublishEmployeeCreatedAsync(employeeEvent);

        return Ok(employee);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Employee updatedEmployee)
    {
        var employee = employees.FirstOrDefault(e => e.Id == id);
        if (employee == null) return NotFound();

        employee.Name = updatedEmployee.Name;
        employee.Email = updatedEmployee.Email;
        employee.DepartmentId = updatedEmployee.DepartmentId;

        return Ok(employee);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var employee = employees.FirstOrDefault(e => e.Id == id);
        if (employee == null) return NotFound();

        employees.Remove(employee);
        return NoContent();
    }
}