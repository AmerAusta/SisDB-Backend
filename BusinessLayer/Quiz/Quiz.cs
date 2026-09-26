using BusinessLayer.Quiz.QuizDto;
using DataLayer.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Quiz
{
    public class Quiz
    {
        private readonly SiSDBDbContext _context;

        public Quiz(SiSDBDbContext context)
        {
            _context = context;
        }

        public List<GetQuizDto> GetQuizzesByLessonId(int lessonId)
        {
            if (lessonId <= 0) return new List<GetQuizDto>();

            return _context.Quizzes
                .Where(q => q.LessonId == lessonId)
                .Select(q => new GetQuizDto
                {
                    QuizId = q.QuizId,
                    LessonId = q.LessonId,
                    Title = q.Title,
                    DurationMinutes = q.DurationMinutes,
                    PassingScore = q.PassingScore
                })
                .ToList();
        }

        public List<GetQuizDto> GetQuizzesByTeacherId(int teacherId)
        {
            if (teacherId <= 0) return new List<GetQuizDto>();

            return _context.Quizzes
                .Join(_context.Lessons,
                      quiz => quiz.LessonId,
                      lesson => lesson.LessonId,
                      (quiz, lesson) => new { quiz, lesson })
                .Join(_context.Assignments,
                      x => x.lesson.AssignmentId,
                      assignment => assignment.AssignmentId,
                      (x, assignment) => new { x.quiz, assignment })
                .Where(x => x.assignment.TeacherId == teacherId)
                .Select(x => new GetQuizDto
                {
                    QuizId = x.quiz.QuizId,
                    LessonId = x.quiz.LessonId,
                    Title = x.quiz.Title,
                    DurationMinutes = x.quiz.DurationMinutes,
                    PassingScore = x.quiz.PassingScore
                })
                .ToList();
        }

        public GetQuizDto? GetQuizById(int quizId)
        {
            if (quizId <= 0) return null;

            return _context.Quizzes
                .Where(q => q.QuizId == quizId)
                .Select(q => new GetQuizDto
                {
                    QuizId = q.QuizId,
                    LessonId = q.LessonId,
                    Title = q.Title,
                    DurationMinutes = q.DurationMinutes,
                    PassingScore = q.PassingScore
                })
                .FirstOrDefault();
        }

        public int AddNewQuiz(AddQuizDto addQuizDto)
        {
            if (addQuizDto == null) return 0;


            var lesson = _context.Lessons.Find(addQuizDto.LessonId);
            if (lesson == null) return -1;

            var quiz = new DataLayer.Models.Entities.Quiz
            {
                LessonId = addQuizDto.LessonId,
                Title = addQuizDto.Title,
                DurationMinutes = addQuizDto.DurationMinutes,
                PassingScore = addQuizDto.PassingScore
            };

            _context.Quizzes.Add(quiz);
            int rowsAffected = _context.SaveChanges();

            return rowsAffected > 0 ? quiz.QuizId : 0;
        }

        public bool UpdateQuiz(UpdateQuizDto updateQuizDto)
        {
            if (updateQuizDto == null) return false;

            var quiz = _context.Quizzes.Find(updateQuizDto.QuizId);
            if (quiz == null) return false;

            if (updateQuizDto.LessonId.HasValue && updateQuizDto.LessonId.Value > 0)
            {
                var lesson = _context.Lessons.Find(updateQuizDto.LessonId.Value);
                if (lesson == null) return false;
                quiz.LessonId = updateQuizDto.LessonId.Value;
            }

            quiz.Title = !string.IsNullOrWhiteSpace(updateQuizDto.Title) ? updateQuizDto.Title : quiz.Title;
            quiz.DurationMinutes = updateQuizDto.DurationMinutes ?? quiz.DurationMinutes;
            quiz.PassingScore = updateQuizDto.PassingScore ?? quiz.PassingScore;

            return _context.SaveChanges() > 0;
        }

        public bool DeleteQuizWithDependencies(int quizId)
        {
            if (quizId <= 0) return false;

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var quiz = _context.Quizzes
                    .Include(q => q.Questions)
                        .ThenInclude(q => q.QuestionChoices)
                    .FirstOrDefault(q => q.QuizId == quizId);

                if (quiz == null)
                    return false;

                var studentGrades = _context.StudentGrades
                    .Where(sg => sg.QuizId == quizId)
                    .ToList();

                if (studentGrades.Any())
                {
                    _context.StudentGrades.RemoveRange(studentGrades);
                }

                foreach (var question in quiz.Questions)
                {
                    if (question.QuestionChoices != null && question.QuestionChoices.Any())
                    {
                        _context.QuestionChoices.RemoveRange(question.QuestionChoices);
                    }
                }

                if (quiz.Questions.Any())
                {
                    _context.Questions.RemoveRange(quiz.Questions);
                }

                _context.Quizzes.Remove(quiz);

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
