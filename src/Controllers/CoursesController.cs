using Microsoft.AspNetCore.Mvc;
using eTeacherProject.Interfaces;
using eTeacherProject.Models.DTOs;
using System.Threading.Tasks;
using eTeacherProject.Models.Entities;

namespace eTeacherProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _service;

    public CoursesController(ICourseService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var courses = await _service.GetAllCoursesAsync();
        return Ok(courses);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CourseDto dto)
    {
        var course = new Course { Title = dto.Title, Description = dto.Description, Lecturer = dto.Lecturer };
        var id = await _service.AddCourseAsync(course);
        return CreatedAtAction(nameof(GetById), new { id }, course);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var course = await _service.GetCourseAsync(id);
        return course is null ? NotFound() : Ok(course);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CourseDto dto)
    {
        var existing = await _service.GetCourseAsync(id);
        if (existing is null) return NotFound();

        existing.Title = dto.Title;
        existing.Description = dto.Description;
        existing.Lecturer = dto.Lecturer;

        var ok = await _service.UpdateCourseAsync(id, existing);
        return ok ? NoContent() : BadRequest();
    }
}
