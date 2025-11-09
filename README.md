# eTeacherProject API Backend Service

This project provides a **RESTful API backend** for managing an e‑learning platform.  
It is built with **ASP.NET Core** and follows a clean architecture pattern using **controllers, services, and repositories**.  
The API exposes endpoints for managing **Students, Courses, and Enrolments**, supporting full CRUD operations and additional domain‑specific actions.

---

## Key Features

### Students API
- Create, read, update, and delete student records
- Each student has `Name`, `Telephone`, and `Address`
- Integrated with enrolments to assign students to courses

### Courses API
- Manage courses with `Title`, `Description`, and `Lecturer`
- Full CRUD support
- Designed to be linked with enrolments

### Enrolments API
- Create and manage enrolments with `Title`, `StartAt`, and `CourseId`
- Assign students to enrolments with validation (prevents duplicates, checks existence)
- Full CRUD support for enrolments
- Returns clear error messages when enrolments or students are not found


<img width="1122" height="800" alt="image" src="https://github.com/user-attachments/assets/f5d602e5-f792-48cc-b866-56aa4f907256" />


---

## Architecture

- **Controllers**: Handle HTTP requests and return appropriate responses (`Ok`, `NotFound`, `BadRequest`, `NoContent`).
- **Services**: Contain business logic and interact with repositories. Examples:
  - `StudentService`
  - `CourseService`
  - `EnrolmentService`
- **Repositories**: Abstract data access via a generic interface `IGenericRepository<T>`. Provides methods:
  - `GetAllAsync()`
  - `GetByIdAsync(id)`
  - `AddAsync(entity)`
  - `UpdateAsync(entity)`
  - `DeleteAsync(id)`

---

## Error Handling

- Controllers return meaningful HTTP status codes:
  - `200 OK` → successful reads
  - `201 Created` → new resources
  - `204 No Content` → successful updates/deletes
  - `404 Not Found` → resource doesn’t exist
  - `400 Bad Request` → invalid updates
- Domain‑specific exceptions (e.g. `GeneralErrorException` in enrolments) ensure clear feedback when business rules are violated.

---

## Database

- By default, the service runs using an **in‑memory database**.
- Optionally, you can configure it to run with a **SQL database**:
  - Set `"UseSqlRepository": true` in `appsettings.json`.
  - Add and configure the appropriate `ConnectionStrings` section in `appsettings.json`.

---

## Installation & Running the Service

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- Optional: A SQL database (need be configured in `appsettings.json`)
- Git

### Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/Mendiii/eTeacherProjectBackEnd.git
   dotnet restore
   dotnet build
   dotnet run


