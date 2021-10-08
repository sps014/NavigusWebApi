namespace NavigusWebApp.Shared.Models
{
    public class AnswerModel
    {
        public string CourseId { get; set; }
        public int QuestionIndex { get; set; }
        public int[] Answers { get; set; }
    }
}
