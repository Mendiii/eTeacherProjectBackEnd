using FluentValidation;
using eTeacherProject.Models.Entities;

namespace eTeacherProject.Validators;

public class CourseValidator : AbstractValidator<Course>
{
    public CourseValidator()
    {
        RuleFor(c => c.Title).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Description).NotEmpty().MaximumLength(500);
        RuleFor(c => c.Lecturer).NotEmpty().MaximumLength(100);
    }
}
