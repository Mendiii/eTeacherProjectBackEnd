using eTeacherProject.Interfaces;
using eTeacherProject.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace eTeacherProject.Services;

public class CourseService : ICourseService
{
    private readonly IGenericRepository<Course> _repo;

    public CourseService(IGenericRepository<Course> repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<Course>> GetAllCoursesAsync() => _repo.GetAllAsync();

    public Task<Course?> GetCourseAsync(int id) => _repo.GetByIdAsync(id);

    public async Task<int> AddCourseAsync(Course course)
    {
        await _repo.AddAsync(course);
        return course.Id;
    }

    public async Task<bool> UpdateCourseAsync(int id, Course course)
    {
        if (id != course.Id) return false;
        await _repo.UpdateAsync(course);
        return true;
    }
    public async Task<bool> DeleteCourseAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
