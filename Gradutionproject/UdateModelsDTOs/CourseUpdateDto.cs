namespace Gradutionproject.UdateModelsDTOs
{
    public class CourseUpdateDto
    {
        public string? Title { get; set; }
        public int? AdminId { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
