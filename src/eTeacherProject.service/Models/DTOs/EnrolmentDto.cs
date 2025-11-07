using System;
using System.ComponentModel.DataAnnotations;

namespace eTeacherProject.Models.DTOs
{
    public class EnrolmentDto
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Starting date is required")]
        public DateTime StartingDate { get; set; }

        [Required(ErrorMessage = "CourseId is required")]
        public int CourseId { get; set; }
    }
}
