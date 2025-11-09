using eTeacherProject.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eTeacherProject.Interfaces;

public interface ICourseService
{
    Task<IEnumerable<Course>> GetAllCoursesAsync();
    Task<Course?> GetCourseAsync(int id);
    Task<int> AddCourseAsync(Course course);
    Task<bool> UpdateCourseAsync(int id, Course course);
    Task<bool> DeleteCourseAsync(int id);
}
