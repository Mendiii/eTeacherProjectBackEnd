using eTeacherProject.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using eTeacherProject.Models.Entities;
using eTeacherProject.Models.DTOs;
using System;
using eTeacherProject.Models.Exceptions;

namespace eTeacherProject.Services;

public class EnrolmentService : IEnrolmentService
{
    private readonly IGenericRepository<Enrolment> _enrolmentsRepo;
    private readonly IGenericRepository<Student> _studentRepo;

    public EnrolmentService(IGenericRepository<Enrolment> enrolmentsRepo, IGenericRepository<Student> studentRepo)
    {
        _enrolmentsRepo = enrolmentsRepo;
        _studentRepo = studentRepo;
    }

    public Task<IEnumerable<Enrolment>> GetAllEnrolmentsAsync() => _enrolmentsRepo.GetAllAsync();

    public Task<Enrolment?> GetEnrolmentAsync(int id) => _enrolmentsRepo.GetByIdAsync(id);

    public async Task<int> AddEnrolmentAsync(Enrolment enrolment)
    {
        var id = Random.Shared.Next(1, 100000);
        enrolment.Id = id;
        await _enrolmentsRepo.AddAsync(enrolment);
        return id;
    }

    public async Task AssignStudentAsync(int enrolmentId, int studentId)
    {
        var enrolment = await _enrolmentsRepo.GetByIdAsync(enrolmentId);
        if (enrolment is null)
        {
            throw new GeneralErrorException($"Enrolment with id {enrolmentId} was not found.");
        }

        var student = await _studentRepo.GetByIdAsync(studentId);
        if (student is null)
        {
            throw new GeneralErrorException($"Student with id {studentId} was not found.");
        }

        if (enrolment.StudentsId.Contains(studentId))
        {
            throw new GeneralErrorException(
                $"Student already assigned to enrolment {enrolmentId}.");
        }

        enrolment.StudentsId.Add(studentId);
        await _enrolmentsRepo.UpdateAsync(enrolment);
    }

    public async Task<bool> UpdateEnrolmentAsync(int id, Enrolment enrolment)
    {
        if (id != enrolment.Id) return false;
        await _enrolmentsRepo.UpdateAsync(enrolment);
        return true;
    }
}

