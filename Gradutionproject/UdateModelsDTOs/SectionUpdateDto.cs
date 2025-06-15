namespace Gradutionproject.UdateModelsDTOs
{
    public class SectionUpdateDto
    {
        public string? Title { get; set; }
        public IFormFile? SectionPDF { get; set; }
        public int? LectureId { get; set; }
        public int? AdminId { get; set; }
    }
}
