using DepartmentService.Models;
using Microsoft.AspNetCore.Mvc;

namespace DepartmentService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private static readonly List<Department> departments = new()
    {
        new Department { Id = 1, Name = "IT" },
        new Department { Id = 2, Name = "HR" }
    };

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(departments);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var department = departments.FirstOrDefault(d => d.Id == id);
        return department == null ? NotFound() : Ok(department);
    }

    [HttpPost]
    public IActionResult Create(Department department)
    {
        department.Id = departments.Max(d => d.Id) + 1;
        departments.Add(department);
        return Ok(department);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var department = departments.FirstOrDefault(d => d.Id == id);

        if (department == null)
            return NotFound();

        departments.Remove(department);
        return NoContent();
    }
}