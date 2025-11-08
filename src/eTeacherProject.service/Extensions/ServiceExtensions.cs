using eTeacherProject.Interfaces;
using eTeacherProject.Repositories;
using eTeacherProject.Services;
using eTeacherProject.Models.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace eTeacherProject.Extensions;

public static class ServiceExtensions
{

    private static async Task EnsureDatabaseExistsAsync(string connectionString, string databaseName)
    {
        // Connect to master DB to check/create
        var builder = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = "master"
        };

        using var conn = new SqlConnection(builder.ConnectionString);
        await conn.OpenAsync();

        var sql = $@"
        IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{databaseName}')
        BEGIN
            CREATE DATABASE [{databaseName}];
        END";

        using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task EnsureTableExistsAsync(string connectionString, string tableName, string createSql)
    {
        using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        var checkSql = $"IF OBJECT_ID('{tableName}', 'U') IS NULL {createSql}";
        using var cmd = new SqlCommand(checkSql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public static void AddAppServices(this IServiceCollection services, IConfiguration config, bool useSqlRepository)
    {
        if (useSqlRepository)
        {
            var connString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
            var builder = new SqlConnectionStringBuilder(connString);
            var dbName = builder.InitialCatalog;

            // Ensure database exists
            EnsureDatabaseExistsAsync(connString, dbName).GetAwaiter().GetResult();

            EnsureTableExistsAsync(connString, "Courses",
           @"CREATE TABLE Courses (
                Id INT PRIMARY KEY IDENTITY,
                Title NVARCHAR(200),
                Description NVARCHAR(MAX),
                Lecturer NVARCHAR(100)
            )").GetAwaiter().GetResult();

            EnsureTableExistsAsync(connString, "Students",
                @"CREATE TABLE Students (
                Id INT PRIMARY KEY IDENTITY,
                Name NVARCHAR(100),
                Telephone NVARCHAR(50),
                Address NVARCHAR(200)
            )").GetAwaiter().GetResult();

            EnsureTableExistsAsync(connString, "Enrolments",
                @"CREATE TABLE Enrolments (
                Id INT PRIMARY KEY IDENTITY,
                Title NVARCHAR(200),
                CourseId INT
            )").GetAwaiter().GetResult();


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
