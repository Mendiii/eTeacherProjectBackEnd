using System;
using System.Collections.Generic;

namespace eTeacherProject.Models.Entities;

public class Enrolment
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int CourseId { get; set; }
    public DateTime StartAt { get; set; }
    public List<int> StudentsId { get; set; } = new();
}
