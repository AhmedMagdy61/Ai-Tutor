using System.Text.Json.Serialization;

namespace Gradutionproject.Models
{
	public class Lecture
	{
		public int Id { get; set; }
		public string Title { get; set; }

		public int CourseId { get; set; }
		public Course Course { get; set; }

		public string FileName { get; set; }
        public int AdminId { get; set; }
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonIgnore]
        public Admin Admin { get; set; }

        public ICollection<Section> Sections { get; set; } = new List<Section>();


	}
}
