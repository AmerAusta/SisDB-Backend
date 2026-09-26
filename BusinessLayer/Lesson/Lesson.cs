using BusinessLayer.Lesson.LessonDto;
using BusinessLayer.Services;
using DataLayer.Data;
using DataLayer.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BusinessLayer.Lesson
{
    public class Lesson
    {
        private readonly SiSDBDbContext _context;

        public Lesson(SiSDBDbContext context)
        {
            _context = context;
        }

        public GetLessonDto? GetLessonById(int lessonId, bool isStudent = false)
        {
            if (lessonId <= 0) return null;

            var query = _context.Lessons.Where(l => l.LessonId == lessonId);

            if (isStudent)
            {
                query = query.Where(l => l.IsPublished);
            }

            return query.Select(l => new GetLessonDto
            {
                LessonId = l.LessonId,
                AssignmentId = l.AssignmentId,
                Title = l.Title,
                VideoUrl = l.VideoUrl,
                PdfUrl = l.PdfUrl,
                IsPublished = l.IsPublished
            }).FirstOrDefault();
        }

        public List<GetLessonDto> GetLessonsByAssignmentId(int assignmentId, bool isStudent = false)
        {
            if (assignmentId <= 0) return new List<GetLessonDto>();

            var query = _context.Lessons.Where(l => l.AssignmentId == assignmentId);

            if (isStudent)
            {
                query = query.Where(l => l.IsPublished);
            }

            return query.Select(l => new GetLessonDto
            {
                LessonId = l.LessonId,
                AssignmentId = l.AssignmentId,
                Title = l.Title,
                VideoUrl = l.VideoUrl,
                PdfUrl = l.PdfUrl,
                IsPublished = l.IsPublished
            }).ToList();
        }

        public List<GetLessonDto> GetLessonsByTeacherId(int teacherId)
        {
            if (teacherId <= 0) return new List<GetLessonDto>();

            var query = _context.Lessons
                .Join(_context.Assignments,
                      lesson => lesson.AssignmentId,
                      assignment => assignment.AssignmentId,
                      (lesson, assignment) => new { lesson, assignment })
                .Where(x => x.assignment.TeacherId == teacherId)
                .Select(x => x.lesson);

            return query.Select(l => new GetLessonDto
            {
                LessonId = l.LessonId,
                AssignmentId = l.AssignmentId,
                Title = l.Title,
                VideoUrl = l.VideoUrl,
                PdfUrl = l.PdfUrl,
                IsPublished = l.IsPublished
            }).ToList();
        }

        public bool AssignmentExists(int AssignmentId)
        {
            return _context.Assignments.Any(s => s.AssignmentId == AssignmentId);
        }

        public int AddNewLesson(AddLessonDto addLessonDto)
        {
            if (addLessonDto == null || string.IsNullOrWhiteSpace(addLessonDto.Title) || addLessonDto.AssignmentId <= 0)
                return 0;

            bool assignmentExists = _context.Assignments.Any(a => a.AssignmentId == addLessonDto.AssignmentId);
            if (!assignmentExists) return 0;

            var lesson = new DataLayer.Models.Entities.Lesson
            {
                AssignmentId = addLessonDto.AssignmentId,
                Title = addLessonDto.Title.Trim(),
                VideoUrl = addLessonDto.VideoUrl,
                PdfUrl = addLessonDto.PdfUrl,
                IsPublished = addLessonDto.IsPublished
            };

            _context.Lessons.Add(lesson);
            int rowsAffected = _context.SaveChanges();

            return rowsAffected > 0 ? lesson.LessonId : 0;
        }

        public bool UpdateLesson(UpdateLessonDto updateLessonDto)
        {
            if (updateLessonDto == null || updateLessonDto.LessonId <= 0) return false;

            var lesson = _context.Lessons.Find(updateLessonDto.LessonId);
            if (lesson == null) return false;

            int targetAssignmentId = updateLessonDto.AssignmentId ?? lesson.AssignmentId;
            string targetTitle = !string.IsNullOrWhiteSpace(updateLessonDto.Title) ? updateLessonDto.Title.Trim() : lesson.Title;
            string? targetVideoUrl = updateLessonDto.VideoUrl ?? lesson.VideoUrl;
            string? targetPdfUrl = updateLessonDto.PdfUrl ?? lesson.PdfUrl;
            bool targetIsPublished = updateLessonDto.IsPublished ?? lesson.IsPublished;

            if (targetAssignmentId != lesson.AssignmentId)
            {
                bool assignmentExists = _context.Assignments.Any(a => a.AssignmentId == targetAssignmentId);
                if (!assignmentExists) return false;
            }

            lesson.AssignmentId = targetAssignmentId;
            lesson.Title = targetTitle;
            lesson.VideoUrl = targetVideoUrl;
            lesson.PdfUrl = targetPdfUrl;
            lesson.IsPublished = targetIsPublished;

            if (!_context.ChangeTracker.HasChanges())
            {
                return true;
            }

            return _context.SaveChanges() > 0;
        }

        public bool DeleteLesson(int lessonId, FileService fileService)
        {
            if (lessonId <= 0) return false;

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var lesson = _context.Lessons.Find(lessonId);
                if (lesson == null)
                    return false;

                var comments = _context.Comments
                    .Where(c => c.LessonId == lessonId)
                    .ToList();
                if (comments.Any())
                {
                    _context.Comments.RemoveRange(comments);
                }

                var attendanceRecords = _context.Attendances
                    .Where(a => a.LessonId == lessonId)
                    .ToList();
                if (attendanceRecords.Any())
                {
                    _context.Attendances.RemoveRange(attendanceRecords);
                }

                var quizzes = _context.Quizzes
                    .Where(q => q.LessonId == lessonId)
                    .Include(q => q.Questions)
                        .ThenInclude(q => q.QuestionChoices)
                    .ToList();

                foreach (var quiz in quizzes)
                {
                    var studentGrades = _context.StudentGrades
                        .Where(sg => sg.QuizId == quiz.QuizId)
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
                }


                if (quizzes.Any())
                {
                    _context.Quizzes.RemoveRange(quizzes);
                }


                if (!string.IsNullOrWhiteSpace(lesson.VideoUrl))
                    fileService.DeleteFile(lesson.VideoUrl);

                if (!string.IsNullOrWhiteSpace(lesson.PdfUrl))
                    fileService.DeleteFile(lesson.PdfUrl);


                _context.Lessons.Remove(lesson);


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