using System.ComponentModel.DataAnnotations;

namespace Gradutionproject.Dtos
{
    public class VerifyCodeDto
    {
        [Required(ErrorMessage = "EmailAddress Required")]
        [EmailAddress(ErrorMessage = "Enter Correct Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Code Required")]
        public string Code { get; set; }
    }
}
