using System.ComponentModel.DataAnnotations;

namespace Gradutionproject.ViewModel
{
	public class RegisterModel
	{
        [Required(ErrorMessage = "Username Required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "EmailAddress Required")]
        [EmailAddress(ErrorMessage = "Enter Correct Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password Required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "EmailAddress Required")]
        [EmailAddress(ErrorMessage = "Enter Correct Email")]
        public string EmailParent { get; set; }
        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "The phone number should contain 11 number")]
        public string PhoneParent { get; set; }
	}
}
