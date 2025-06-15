using System.ComponentModel.DataAnnotations;

namespace Gradutionproject.ViewModel
{
	public class LoginModel
	{
        [Required(ErrorMessage = "EmailAddress Required")]
        [EmailAddress(ErrorMessage = "Enter Correct Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password Required")]
        public string Password { get; set; }
	}
}
