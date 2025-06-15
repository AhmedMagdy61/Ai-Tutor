using Microsoft.AspNetCore.Identity;

namespace Gradutionproject.Models
{
	

	public class ApplicationUser : IdentityUser
	{

        public ICollection<Quiz> Quizzes { get; set; }
        public string EmailParent { get; set; }
		public string PhoneParent { get; set; }

	}
}
