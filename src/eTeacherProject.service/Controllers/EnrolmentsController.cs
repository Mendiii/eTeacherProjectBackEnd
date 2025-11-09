using Microsoft.AspNetCore.Mvc;
using eTeacherProject.Interfaces;
using eTeacherProject.Models;
using System.Threading.Tasks;
using eTeacherProject.Models.Entities;
using eTeacherProject.Models.DTOs;
using System;
using eTeacherProject.Models.Exceptions;

namespace eTeacherProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrolmentsController : ControllerBase
{
    private readonly IEnrolmentService _service;

    public EnrolmentsController(IEnrolmentService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var enrolments = await _service.GetAllEnrolmentsAsync();
        return Ok(enrolments);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var enrolment = await _service.GetEnrolmentAsync(id);
        return enrolment is null ? NotFound() : Ok(enrolment);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EnrolmentDto enrolmentRequest)
    {
        var enrolment = new Enrolment { Title = enrolmentRequest.Title, StartAt = enrolmentRequest.StartingDate, CourseId = enrolmentRequest.CourseId };
        var id = await _service.AddEnrolmentAsync(enrolment);
        return CreatedAtAction(nameof(GetById), new { id }, enrolment);
    }

    [HttpPost("{id:int}/assign-student")]
    public async Task<IActionResult> AssignStudent(int id, [FromBody] int studentId)
    {
        try
        {
            await _service.AssignStudentAsync(id, studentId);
            return Ok();
        }
        catch (GeneralErrorException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EnrolmentDto dto)
    {
        var existing = await _service.GetEnrolmentAsync(id);
        if (existing is null) return NotFound();

        existing.Title = dto.Title;
        existing.StartAt = dto.StartingDate;
        existing.CourseId = dto.CourseId;

        var ok = await _service.UpdateEnrolmentAsync(id, existing);
        return ok ? NoContent() : BadRequest();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteEnrolmentAsync(id);
        return ok ? NoContent() : NotFound();
    }
}
