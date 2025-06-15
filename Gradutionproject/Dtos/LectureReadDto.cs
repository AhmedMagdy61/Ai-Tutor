namespace Gradutionproject.Dtos
{
    public class LectureReadDto
    {
        public int LectureId { get; set; }
        public string Title { get; set; }
        public string LecturePDF { get; set; }
        public int CourseId { get; set; }
        public string TitleCourse { get; set; }
        public int AdminId { get; set; }
    }
}
