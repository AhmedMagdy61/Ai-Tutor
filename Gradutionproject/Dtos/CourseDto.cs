using System.ComponentModel.DataAnnotations;

namespace Gradutionproject.Dtos
{
    public class CourseDto
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "AdminId is required.")]
        public int AdminId { get; set; }
        [Required(ErrorMessage = "photo is required.")]
        public IFormFile Photo { get; set; }
    }
}
