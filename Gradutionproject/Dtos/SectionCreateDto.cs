using System.ComponentModel.DataAnnotations;

namespace Gradutionproject.Dtos
{
    public class SectionCreateDto
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "PDFFile is required.")]
        public IFormFile SectionPDF { get; set; }
        [Required(ErrorMessage = "LectureId is required.")]
        public int LectureId { get; set; }
        [Required(ErrorMessage = "AdminId is required.")]
        public int AdminId { get; set; }
    }
}
