using eTeacherProject.Interfaces;
using eTeacherProject.Repositories;
using eTeacherProject.Services;
using eTeacherProject.Models.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace eTeacherProject.Extensions;

public static class ServiceExtensions
{
    public static void AddAppServices(this IServiceCollection services, IConfiguration config, bool useSqlRepository)
    {
        // Choose repository implementation
        if (useSqlRepository)
        {
            var connString = config.GetConnectionString("DefaultConnection") ?? string.Empty;

            // For SQL repository, you typically register specialized repos per entity due to mapping needs:
            services.AddScoped<IGenericRepository<Course>>(sp =>
                new SqlRepository<Course>(connString, "Courses", reader => new Course {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    Lecturer = reader.GetString(reader.GetOrdinal("Lecturer"))
                }));

            services.AddScoped<IGenericRepository<Student>>(sp =>
                new SqlRepository<Student>(connString, "Students", reader => new Student {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Telephone = reader.GetString(reader.GetOrdinal("Telephone")),
                    Address = reader.GetString(reader.GetOrdinal("Address"))
                }));

            // Enrolments often need joins; for simplicity, store only core fields:
            services.AddScoped<IGenericRepository<Enrolment>>(sp =>
                new SqlRepository<Enrolment>(connString, "Enrolments", reader => new Enrolment {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                    // Course and Students hydration would require additional queries at service level
                }));
        }
        else
        {
            services.AddSingleton<IGenericRepository<Course>, InMemoryRepository<Course>>();
            services.AddSingleton<IGenericRepository<Student>, InMemoryRepository<Student>>();
            services.AddSingleton<IGenericRepository<Enrolment>, InMemoryRepository<Enrolment>>();
        }

        // Services
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IEnrolmentService, EnrolmentService>();

    }
}
