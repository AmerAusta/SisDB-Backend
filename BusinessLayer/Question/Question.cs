using BusinessLayer.Question.QuestionDto;
using DataLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Question
{
    public class Question
    {
        private readonly SiSDBDbContext _context;

        public Question(SiSDBDbContext context)
        {
            _context = context;
        }

        public GetQuestionDto? GetQuestionById(int questionId)
        {
            if (questionId <= 0) return null;

            var question = _context.Questions.Find(questionId);
            if (question == null) return null;

            return new GetQuestionDto
            {
                QuestionId = question.QuestionId,
                QuizId = question.QuizId,
                QuestionText = question.QuestionText,
                Points = question.Points
            };
        }

        public List<GetQuestionDto> GetQuestionsByQuizId(int quizId)
        {
            if (quizId <= 0) return new List<GetQuestionDto>();

            return _context.Questions
                .Where(q => q.QuizId == quizId)
                .Select(q => new GetQuestionDto
                {
                    QuestionId = q.QuestionId,
                    QuizId = q.QuizId,
                    QuestionText = q.QuestionText,
                    Points = q.Points
                })
                .ToList();
        }

        public List<GetQuestionDto> GetAllQuestions()
        {
            return _context.Questions
                .Select(q => new GetQuestionDto
                {
                    QuestionId = q.QuestionId,
                    QuizId = q.QuizId,
                    QuestionText = q.QuestionText,
                    Points = q.Points
                })
                .ToList();
        }

        public int AddNewQuestion(AddQuestionDto addQuestionDto)
        {
            if (addQuestionDto == null) return 0;

            var quiz = _context.Quizzes.Find(addQuestionDto.QuizId);
            if (quiz == null) return -1;

            var question = new DataLayer.Models.Entities.Question
            {
                QuizId = addQuestionDto.QuizId,
                QuestionText = addQuestionDto.QuestionText,
                Points = addQuestionDto.Points
            };

            _context.Questions.Add(question);
            int rowsAffected = _context.SaveChanges();

            return rowsAffected > 0 ? question.QuestionId : 0;
        }

        public bool UpdateQuestion(UpdateQuestionDto updateQuestionDto)
        {
            if (updateQuestionDto == null || updateQuestionDto.QuestionId <= 0) return false;

            var question = _context.Questions.Find(updateQuestionDto.QuestionId);
            if (question == null) return false;

       
            if (updateQuestionDto.QuizId.HasValue && updateQuestionDto.QuizId.Value > 0)
            {
                var quiz = _context.Quizzes.Find(updateQuestionDto.QuizId.Value);
                if (quiz == null) return false;
                question.QuizId = updateQuestionDto.QuizId.Value;
            }

            if (!string.IsNullOrWhiteSpace(updateQuestionDto.QuestionText))
            {
                question.QuestionText = updateQuestionDto.QuestionText;
            }

            if (updateQuestionDto.Points.HasValue && updateQuestionDto.Points.Value >= 0)
            {
                question.Points = updateQuestionDto.Points.Value;
            }

            return _context.SaveChanges() > 0;
        }

        public bool DeleteQuestionWithChoices(int questionId)
        {
            if (questionId <= 0) return false;

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var question = _context.Questions
                    .Include(q => q.QuestionChoices)
                    .FirstOrDefault(q => q.QuestionId == questionId);

                if (question == null)
                    return false;

                if (question.QuestionChoices != null && question.QuestionChoices.Any())
                {
                    _context.QuestionChoices.RemoveRange(question.QuestionChoices);
                }

                _context.Questions.Remove(question);

                _context.SaveChanges();

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}