using FluentValidation;
using eTeacherProject.Models.Entities;

namespace eTeacherProject.Validators;

public class StudentValidator : AbstractValidator<Student>
{
    public StudentValidator()
    {
        RuleFor(s => s.Name).NotNull().NotEmpty().MinimumLength(2);
        RuleFor(s => s.Telephone).NotEmpty().MaximumLength(30);
        RuleFor(s => s.Address).NotEmpty().MaximumLength(200);
    }
}
