namespace Gradutionproject.Dtos
{
    public class QuizDto
    {
        public int QuizId { get; set; }
        public float Score { get; set; }
        public DateTime QuizDate { get; set; }=DateTime.Now;
        public string CourseName { get; set; }
    }
}
