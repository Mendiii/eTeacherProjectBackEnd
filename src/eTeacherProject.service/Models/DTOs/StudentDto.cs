using System.ComponentModel.DataAnnotations;

namespace eTeacherProject.Models.DTOs;

public class StudentDto
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Telephone is required")]
    public string Telephone { get; set; }

    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; }
}