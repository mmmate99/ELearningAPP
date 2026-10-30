namespace ElearningAPI.Models
{
    public class AnswerOption
    {
        public int Id { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int Order { get; set; }
        
        // Foreign keys
        public int QuestionId { get; set; }
        
        // Navigációs tulajdonságok
        public virtual Question Question { get; set; } = null!;
    }
}