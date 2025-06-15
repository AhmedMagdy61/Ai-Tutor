using System.Reflection;
using System.Text.Json.Serialization;

namespace Gradutionproject.Models
{
    public class Course
    {

		public int Id { get; set; }

		public string Title { get; set; }

        public string ImageName { get; set; }
        public int AdminId { get; set; }
        // [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonIgnore]
        public Admin Admin { get; set; }

        public ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();
        public ICollection<Quiz> Quizzes { get; set; }
    }
}
