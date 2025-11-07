using eTeacherProject.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eTeacherProject.Interfaces;

public interface IStudentService
{
    Task<IEnumerable<Student>> GetAllStudentsAsync();
    Task<Student?> GetStudentAsync(int id);
    Task<int> AddStudentAsync(Student student);
    Task<bool> UpdateStudentAsync(int id, Student student);
}
