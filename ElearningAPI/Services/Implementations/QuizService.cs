using AutoMapper;
using ElearningAPI.Models;
using ElearningAPI.Models.DTOs;
using ElearningAPI.Repository.Interfaces;
using ElearningAPI.Services.Interfaces;
using System.Text.Json;

namespace ElearningAPI.Services.Implementations
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IMapper _mapper;
        private readonly ICourseRepository _courseRepository;

        public QuizService(IQuizRepository quizRepository, IMapper mapper, ICourseRepository courseRepository)
        {
            _quizRepository = quizRepository;
            _mapper = mapper;
            _courseRepository = courseRepository;
        }

        public async Task<QuizDto?> GetQuizByIdAsync(int id)
        {
            var quiz = await _quizRepository.GetByIdAsync(id);
            return quiz != null ? _mapper.Map<QuizDto>(quiz) : null;
        }

        public async Task<QuizDto> CreateQuizAsync(CreateQuizDto createDto)
        {
            var quiz = _mapper.Map<Quiz>(createDto);
            var createdQuiz = await _quizRepository.CreateAsync(quiz);
            return _mapper.Map<QuizDto>(createdQuiz);
        }

        public async Task<QuizDto?> UpdateQuizAsync(int id, CreateQuizDto updateDto)
        {
            var quiz = await _quizRepository.GetByIdAsync(id);
            if (quiz == null) return null;

            _mapper.Map(updateDto, quiz);
            var updatedQuiz = await _quizRepository.UpdateAsync(quiz);
            return _mapper.Map<QuizDto>(updatedQuiz);
        }

        public async Task<bool> DeleteQuizAsync(int id)
        {
            return await _quizRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<QuestionDto>> GetQuizQuestionsAsync(int quizId)
        {
            var questions = await _quizRepository.GetQuizQuestionsAsync(quizId);
            return _mapper.Map<List<QuestionDto>>(questions);
        }

        public async Task<QuizResultDto> SubmitQuizAttemptAsync(int quizId, string userId, SubmitQuizDto submitDto)
        {
            // Ellenőrizd, hogy a felhasználó jogosult-e a kvíz kitöltésére
            if (!await CanUserTakeQuizAsync(quizId, userId))
            {
                throw new UnauthorizedAccessException("User cannot take this quiz");
            }

            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null) throw new Exception("Quiz not found");

            var nextAttemptNumber = await _quizRepository.GetNextAttemptNumberAsync(userId, quizId);
            
            if (nextAttemptNumber > quiz.MaxAttempts)
            {
                throw new Exception("Maximum attempts exceeded");
            }

            // Kvíz kiértékelése
            var result = await EvaluateQuizAsync(quiz, submitDto);
            result.AttemptNumber = nextAttemptNumber;

            // QuizAttempt mentése
            var quizAttempt = new QuizAttempt
            {
                UserId = userId,
                QuizId = quizId,
                Score = result.Score,
                TotalPoints = result.TotalPoints,
                Percentage = result.Percentage,
                IsPassed = result.IsPassed,
                AttemptNumber = nextAttemptNumber,
                StartedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow,
                AnswersJson = JsonSerializer.Serialize(submitDto.Answers)
            };

            await _quizRepository.CreateQuizAttemptAsync(quizAttempt);

            return result;
        }

        private async Task<QuizResultDto> EvaluateQuizAsync(Quiz quiz, SubmitQuizDto submitDto)
        {
            var result = new QuizResultDto
            {
                QuizId = quiz.Id,
                TotalPoints = 0,
                Score = 0,
                QuestionResults = new List<QuestionResultDto>()
            };

            foreach (var question in quiz.Questions)
            {
                var questionResult = await EvaluateQuestionAsync(question, submitDto);
                result.QuestionResults.Add(questionResult);
                result.TotalPoints += questionResult.TotalPoints;
                result.Score += questionResult.PointsEarned;
            }

            result.Percentage = result.TotalPoints > 0 ? (double)result.Score / result.TotalPoints * 100 : 0;
            result.IsPassed = result.Percentage >= quiz.PassPercentage;

            return result;
        }

        private Task<QuestionResultDto> EvaluateQuestionAsync(Question question, SubmitQuizDto submitDto)
        {
            var userAnswer = submitDto.Answers.FirstOrDefault(a => a.QuestionId == question.Id);
            var result = new QuestionResultDto
            {
                QuestionId = question.Id,
                TotalPoints = question.Points
            };

            if (userAnswer == null)
            {
                result.PointsEarned = 0;
                result.IsCorrect = false;
                return Task.FromResult(result);
            }

            switch (question.Type)
            {
                case QuestionType.MultipleChoice:
                    result.IsCorrect = EvaluateMultipleChoice(question, userAnswer);
                    break;
                case QuestionType.TrueFalse:
                    result.IsCorrect = EvaluateTrueFalse(question, userAnswer);
                    break;
                case QuestionType.ShortAnswer:
                    result.IsCorrect = EvaluateShortAnswer(question, userAnswer);
                    break;
                case QuestionType.Matching:
                    result.IsCorrect = EvaluateMatching(question, userAnswer);
                    break;
                case QuestionType.Sorting:
                    result.IsCorrect = EvaluateSorting(question, userAnswer);
                    break;
                default:
                    result.IsCorrect = false;
                    break;
            }

            result.PointsEarned = result.IsCorrect ? question.Points : 0;
            result.CorrectAnswer = question.CorrectAnswer;
            
            // User answer összefűzése
            if (!string.IsNullOrEmpty(userAnswer.TextAnswer))
                result.UserAnswer = userAnswer.TextAnswer;
            else if (userAnswer.SelectedOptionIds != null && userAnswer.SelectedOptionIds.Any())
                result.UserAnswer = $"Selected options: {string.Join(", ", userAnswer.SelectedOptionIds)}";
            else if (userAnswer.MatchingPairs != null && userAnswer.MatchingPairs.Any())
                result.UserAnswer = $"Matching pairs: {string.Join("; ", userAnswer.MatchingPairs.Select(p => $"{p.Key}->{p.Value}"))}";
            else if (userAnswer.SortedItems != null && userAnswer.SortedItems.Any())
                result.UserAnswer = $"Sorted items: {string.Join(" -> ", userAnswer.SortedItems)}";

            return Task.FromResult(result);
        }

        private bool EvaluateMultipleChoice(Question question, QuestionAttemptDto userAnswer)
        {
            if (userAnswer.SelectedOptionIds == null || !userAnswer.SelectedOptionIds.Any())
                return false;

            var correctOptionIds = question.AnswerOptions
                .Where(ao => ao.IsCorrect)
                .Select(ao => ao.Id)
                .OrderBy(id => id)
                .ToList();

            var userSelectedIds = userAnswer.SelectedOptionIds
                .OrderBy(id => id)
                .ToList();

            return correctOptionIds.SequenceEqual(userSelectedIds);
        }

        private bool EvaluateTrueFalse(Question question, QuestionAttemptDto userAnswer)
        {
            var userAnswerText = userAnswer.TextAnswer?.ToLower().Trim();
            var correctAnswer = question.CorrectAnswer?.ToLower().Trim();
            
            return userAnswerText == correctAnswer;
        }

        private bool EvaluateShortAnswer(Question question, QuestionAttemptDto userAnswer)
        {
            var userAnswerText = userAnswer.TextAnswer?.ToLower().Trim();
            var correctAnswer = question.CorrectAnswer?.ToLower().Trim();
            
            return userAnswerText == correctAnswer;
        }

        private bool EvaluateMatching(Question question, QuestionAttemptDto userAnswer)
        {
            if (userAnswer.MatchingPairs == null || string.IsNullOrEmpty(question.MatchingPairsJson))
                return false;

            try
            {
                var correctPairs = JsonSerializer.Deserialize<Dictionary<string, string>>(question.MatchingPairsJson);
                
                if (correctPairs == null || correctPairs.Count != userAnswer.MatchingPairs.Count)
                    return false;
                
                foreach (var pair in userAnswer.MatchingPairs)
                {
                    if (!correctPairs.TryGetValue(pair.Key, out var correctValue) || correctValue != pair.Value)
                        return false;
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool EvaluateSorting(Question question, QuestionAttemptDto userAnswer)
        {
            if (userAnswer.SortedItems == null || string.IsNullOrEmpty(question.SortingItemsJson))
                return false;

            try
            {
                var correctOrder = JsonSerializer.Deserialize<List<string>>(question.SortingItemsJson);
                return userAnswer.SortedItems.SequenceEqual(correctOrder ?? new List<string>());
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<QuizAttemptDto>> GetUserQuizAttemptsAsync(int quizId, string userId)
        {
            var attempts = await _quizRepository.GetUserQuizAttemptsAsync(userId, quizId);
            return _mapper.Map<List<QuizAttemptDto>>(attempts);
        }

        public async Task<bool> CanUserTakeQuizAsync(int quizId, string userId)
        {
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null) return false;

            // Ha a kvíz egy leckéhez tartozik, ellenőrizd, hogy a felhasználó hozzáfér-e a leckéhez
            if (quiz.LessonId.HasValue)
            {
                // Itt implementáld a hozzáférés ellenőrzést
                return true;
            }

            // Ha a kvíz egy kurzus vizsga, ellenőrizd, hogy a felhasználó fel van-e iratkozva
            if (quiz.CourseId.HasValue)
            {
                return await _courseRepository.IsUserEnrolledAsync(userId, quiz.CourseId.Value);
            }

            return false;
        }
    }
}