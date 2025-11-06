using System.Threading.Tasks;
using System.Collections.Generic;
using eTeacherProject.Models.Entities;
using eTeacherProject.Models.DTOs;

namespace eTeacherProject.Interfaces;

public interface IEnrolmentService
{
    Task<IEnumerable<Enrolment>> GetAllEnrolmentsAsync();
    Task<Enrolment?> GetEnrolmentAsync(int id);
    Task<int> AddEnrolmentAsync(Enrolment enrolment);
    Task AssignStudentAsync(int enrolmentId, int studentId);
    Task<bool> UpdateEnrolmentAsync(int id, Enrolment enrolment);
}
