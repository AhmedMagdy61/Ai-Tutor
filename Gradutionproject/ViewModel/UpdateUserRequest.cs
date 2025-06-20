using System.ComponentModel.DataAnnotations;

namespace Gradutionproject.ViewModel
{
    public class UpdateUserRequest
    {
        public string? UserName { get; set; }
        [EmailAddress(ErrorMessage = "Enter Correct Email")]
        public string? EmailParent { get; set; }
        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Enter a valid Egyptian phone number starting with 01")]
        [MaxLength(11, ErrorMessage = "Phone number must be 11 digits")]
        public string? PhoneParent { get; set; }

        [Required(ErrorMessage = "Password is required for verification")]
        public string Password { get; set; }
    }
}
