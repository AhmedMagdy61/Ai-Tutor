using System.ComponentModel.DataAnnotations;

namespace Gradutionproject.Dtos
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "EmailAddress Required")]
        [EmailAddress(ErrorMessage = "Enter Correct Email")]
        public string Email { get; set; }
    }

}
