using System.ComponentModel.DataAnnotations;

namespace Gradutionproject.Dtos
{
    public class CreateQuizDTO
    {
        [Required(ErrorMessage = "Score required.")]
        public float Score { get; set; }
        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Course ID is required.")]
        public int CourseId { get; set; }
        [Required(ErrorMessage = "Number of question is required.")]
        public int NumberOfQuestion { get; set; }
    }
}
