using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Gradutionproject.Models
{
    public class Admin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdminId { get; set; }
        public string Username { get; set; }

        [Required(ErrorMessage = "EmailAddress Required")]
        [EmailAddress(ErrorMessage = "Enter Correct Email")]
        public string Email { get; set; }
        public string AdminPassword { get; set; }

        public ICollection<Course>? Courses { get; set; }
        public ICollection<Lecture>? Lectures { get; set; }
        public ICollection<Section>? Sections { get; set; }
    }
}
