using System.ComponentModel.DataAnnotations;

namespace Gradutionproject.ViewModel
{
    public class UpdateUserRequest
    {
        public string? UserName { get; set; }
        [EmailAddress(ErrorMessage = "Enter Correct Email")]
        public string? EmailParent { get; set; }
        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Phone number must be exactly 11 digits and start with 01")]
        public string? PhoneParent { get; set; }

        [Required(ErrorMessage = "Password is required for verification")]
        public string Password { get; set; }
    }
}
