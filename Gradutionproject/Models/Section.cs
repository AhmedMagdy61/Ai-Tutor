using System.Text.Json.Serialization;

namespace Gradutionproject.Models
{
	public class Section
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public string FileName { get; set; }
		public int LectureId { get; set; }
		public Lecture Lecture { get; set; }
        public int AdminId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Admin Admin { get; set; }
    }
}
