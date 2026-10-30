using System.Collections.Generic;

namespace ElearningAPI.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public int Points { get; set; }
        public int Order { get; set; }
        
        // Különböző kérdéstípusokhoz
        public string? CorrectAnswer { get; set; }
        public string? MatchingPairsJson { get; set; }
        public string? SortingItemsJson { get; set; }
        
        // Foreign keys
        public int QuizId { get; set; }
        
        // Navigációs tulajdonságok
        public virtual Quiz Quiz { get; set; } = null!;
        public virtual ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
    }

    public enum QuestionType
    {
        MultipleChoice,
        TrueFalse,
        ShortAnswer,
        Matching,
        Sorting
    }
}