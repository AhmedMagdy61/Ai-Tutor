using System.ComponentModel.DataAnnotations;

namespace Gradutionproject.Dtos
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "EmailAddress Required")]
        [EmailAddress(ErrorMessage = "Enter Correct Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "New Password Required")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "New Password Required")]
        public string ConfirmPassword { get; set; }

    }
}
