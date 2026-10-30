using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ElearningAPI.Models.DTOs
{
    public class QuizDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int PassPercentage { get; set; }
        public int TimeLimit { get; set; }
        public bool IsPublished { get; set; }
        public int MaxAttempts { get; set; }
        public int? LessonId { get; set; }
        public int? CourseId { get; set; }
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }

    public class QuestionDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public int Points { get; set; }
        public int Order { get; set; }
        public List<AnswerOptionDto> AnswerOptions { get; set; } = new List<AnswerOptionDto>();
        public string? CorrectAnswer { get; set; }
        public string? MatchingPairsJson { get; set; }
        public string? SortingItemsJson { get; set; }
    }

    public class AnswerOptionDto
    {
        public int Id { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int Order { get; set; }
    }

    public class CreateQuizDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        
        [Range(0, 100)]
        public int PassPercentage { get; set; } = 70;
        
        [Range(0, 300)]
        public int TimeLimit { get; set; } = 0;
        
        public bool IsPublished { get; set; } = false;
        
        [Range(1, 10)]
        public int MaxAttempts { get; set; } = 3;
        
        public int? LessonId { get; set; }
        public int? CourseId { get; set; }
    }

    // DTO for submitting quiz answers (used when student takes quiz)
    public class SubmitQuizDto
    {
        public int QuizId { get; set; }
        public List<QuestionAttemptDto> Answers { get; set; } = new List<QuestionAttemptDto>();
    }

    // Individual question answer
    public class QuestionAttemptDto
    {
        public int QuestionId { get; set; }
        public List<int>? SelectedOptionIds { get; set; }
        public string? TextAnswer { get; set; }
        public Dictionary<string, string>? MatchingPairs { get; set; }
        public List<string>? SortedItems { get; set; }
    }

    // DTO for quiz attempt history (what user has completed)
    public class QuizAttemptDto
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int AttemptNumber { get; set; }
        public int Score { get; set; }
        public int TotalPoints { get; set; }
        public double Percentage { get; set; }
        public bool IsPassed { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class QuizResultDto
    {
        public int QuizId { get; set; }
        public int Score { get; set; }
        public int TotalPoints { get; set; }
        public double Percentage { get; set; }
        public bool IsPassed { get; set; }
        public int AttemptNumber { get; set; }
        public List<QuestionResultDto> QuestionResults { get; set; } = new List<QuestionResultDto>();
    }

    public class QuestionResultDto
    {
        public int QuestionId { get; set; }
        public bool IsCorrect { get; set; }
        public int PointsEarned { get; set; }
        public int TotalPoints { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? UserAnswer { get; set; }
    }
}