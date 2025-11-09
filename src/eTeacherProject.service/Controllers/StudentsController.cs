using Microsoft.AspNetCore.Mvc;
using eTeacherProject.Interfaces;
using eTeacherProject.Models.DTOs;
using eTeacherProject.Models.Entities;
using System.Threading.Tasks;

namespace eTeacherProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;
    private readonly IEnrolmentService _enrolmentService;

    public StudentsController(IStudentService service, IEnrolmentService enrolmentService)
    {
        _service = service;
        _enrolmentService = enrolmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var students = await _service.GetAllStudentsAsync();
        return Ok(students);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StudentDto dto)
    {
        var student = new Student { Name = dto.Name, Telephone = dto.Telephone, Address = dto.Address };
        var id = await _service.AddStudentAsync(student);
        return CreatedAtAction(nameof(GetById), new { id }, student);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var student = await _service.GetStudentAsync(id);
        return student is null ? NotFound() : Ok(student);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] StudentDto dto)
    {
        var existing = await _service.GetStudentAsync(id);
        if (existing is null) return NotFound();

        existing.Name = dto.Name;
        existing.Telephone = dto.Telephone;
        existing.Address = dto.Address;

        var ok = await _service.UpdateStudentAsync(id, existing);
        return ok ? NoContent() : BadRequest();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteStudentAsync(id);
        return ok ? NoContent() : NotFound();
    }
}
