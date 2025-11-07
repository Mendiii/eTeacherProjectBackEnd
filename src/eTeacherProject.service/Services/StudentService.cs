using eTeacherProject.Interfaces;
using eTeacherProject.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eTeacherProject.Services;

public class StudentService : IStudentService
{
    private readonly IGenericRepository<Student> _repo;

    public StudentService(IGenericRepository<Student> repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<Student>> GetAllStudentsAsync() => _repo.GetAllAsync();

    public Task<Student?> GetStudentAsync(int id) => _repo.GetByIdAsync(id);

    public async Task<int> AddStudentAsync(Student student)
    {
        await _repo.AddAsync(student);
        return student.Id;
    }

    public async Task<bool> UpdateStudentAsync(int id, Student student)
    {
        if (id != student.Id) return false;
        await _repo.UpdateAsync(student);
        return true;
    }
}
