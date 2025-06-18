namespace Gradutionproject.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public float Score { get; set; }
        public DateTime QuizDate { get; set; } = DateTime.Now;

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int NumberOfQuestion { get; set; }
        public string UserName { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
