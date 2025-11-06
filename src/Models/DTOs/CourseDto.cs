using System.ComponentModel.DataAnnotations;

namespace eTeacherProject.Models.DTOs
{
    public class CourseDto
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Lecturer is required")]
        public string Lecturer { get; set; }
    }
}
