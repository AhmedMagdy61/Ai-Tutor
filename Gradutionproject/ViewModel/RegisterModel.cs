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
        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Enter a valid Egyptian phone number starting with 01")]
        [MaxLength(11, ErrorMessage = "Phone number must be 11 digits")]
        public string PhoneParent { get; set; }
	}
}
