using System.ComponentModel.DataAnnotations;

namespace Gradutionproject.ViewModel
{
    public class PasswordConfirmationRequest
    {
        [Required]
        public string Password { get; set; }
    }
}
