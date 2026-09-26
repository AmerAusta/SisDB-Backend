using BusinessLayer.StudentGrade.StudentGradeDto;
using DataLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.StudentGrade
{
    public class StudentGrade
    {
        private readonly SiSDBDbContext _context;

        public StudentGrade(SiSDBDbContext context)
        {
            _context = context;
        }

        public GetStudentGradeDto? GetGradeById(int gradeId)
        {
            if (gradeId <= 0) return null;

            var grade = _context.StudentGrades.Find(gradeId);
            if (grade == null) return null;

            return new GetStudentGradeDto
            {
                GradeId = grade.GradeId,
                StudentId = grade.StudentId,
                QuizId = grade.QuizId,
                Score = grade.Score,
                AttemptNumber = grade.AttemptNumber,
                DateAttempted = grade.DateAttempted
            };
        }

        public List<GetStudentGradeDto> GetGradesByStudentId(int studentId)
        {
            if (studentId <= 0) return new List<GetStudentGradeDto>();

            return _context.StudentGrades
                .Where(g => g.StudentId == studentId)
                .Select(g => new GetStudentGradeDto
                {
                    GradeId = g.GradeId,
                    StudentId = g.StudentId,
                    QuizId = g.QuizId,
                    Score = g.Score,
                    AttemptNumber = g.AttemptNumber,
                    DateAttempted = g.DateAttempted
                })
                .ToList();
        }

        public List<GetStudentGradeDto> GetGradesByQuizId(int quizId)
        {
            if (quizId <= 0) return new List<GetStudentGradeDto>();

            return _context.StudentGrades
                .Where(g => g.QuizId == quizId)
                .Select(g => new GetStudentGradeDto
                {
                    GradeId = g.GradeId,
                    StudentId = g.StudentId,
                    QuizId = g.QuizId,
                    Score = g.Score,
                    AttemptNumber = g.AttemptNumber,
                    DateAttempted = g.DateAttempted
                })
                .ToList();
        }

        public List<GetStudentGradeWithQuizDetailsDto> GetGradesWithQuizDetailsByStudentId(int studentId)
        {
            if (studentId <= 0) return new List<GetStudentGradeWithQuizDetailsDto>();

            return _context.StudentGrades
                .Where(g => g.StudentId == studentId)
                .Join(_context.Quizzes,
                      grade => grade.QuizId,
                      quiz => quiz.QuizId,
                      (grade, quiz) => new GetStudentGradeWithQuizDetailsDto
                      {
                          GradeId = grade.GradeId,
                          StudentId = grade.StudentId,
                          Score = grade.Score,
                          AttemptNumber = grade.AttemptNumber,
                          DateAttempted = grade.DateAttempted,
                          QuizId = quiz.QuizId,
                          LessonId = quiz.LessonId,
                          Title = quiz.Title,
                          DurationMinutes = quiz.DurationMinutes,
                          PassingScore = quiz.PassingScore,
                          IsPassed= grade.Score > quiz.PassingScore 

                      })
                .ToList();
        }

        public List<GetStudentGradeDto> GetAllGrades()
        {
            return _context.StudentGrades
                .Select(g => new GetStudentGradeDto
                {
                    GradeId = g.GradeId,
                    StudentId = g.StudentId,
                    QuizId = g.QuizId,
                    Score = g.Score,
                    AttemptNumber = g.AttemptNumber,
                    DateAttempted = g.DateAttempted
                })
                .ToList();
        }

        public int AddNewGrade(AddStudentGradeDto addGradeDto)
        {
            if (addGradeDto == null) return 0;

            var student = _context.Students.Find(addGradeDto.StudentId);
            if (student == null) return -1; 

            var quiz = _context.Quizzes.Find(addGradeDto.QuizId);
            if (quiz == null) return -2; 

            int lastAttemptNumber = _context.StudentGrades
                .Where(g => g.StudentId == addGradeDto.StudentId && g.QuizId == addGradeDto.QuizId)
                .Max(g => (int?)g.AttemptNumber) ?? 0;

            int newAttemptNumber = lastAttemptNumber + 1;

            var grade = new DataLayer.Models.Entities.StudentGrade
            {
                StudentId = addGradeDto.StudentId,
                QuizId = addGradeDto.QuizId,
                Score = addGradeDto.Score,
                AttemptNumber = newAttemptNumber,
                DateAttempted = DateTime.Now
            };

            _context.StudentGrades.Add(grade);
            int rowsAffected = _context.SaveChanges();

            return rowsAffected > 0 ? grade.GradeId : 0;
        }

        public bool UpdateGrade(UpdateStudentGradeDto updateGradeDto)
        {
            if (updateGradeDto == null || updateGradeDto.GradeId <= 0) return false;

            var grade = _context.StudentGrades.Find(updateGradeDto.GradeId);
            if (grade == null) return false;

            if (updateGradeDto.StudentId.HasValue && updateGradeDto.StudentId.Value > 0)
            {
                var student = _context.Students.Find(updateGradeDto.StudentId.Value);
                if (student == null) return false;
                grade.StudentId = updateGradeDto.StudentId.Value;
            }

            if (updateGradeDto.QuizId.HasValue && updateGradeDto.QuizId.Value > 0)
            {
                var quiz = _context.Quizzes.Find(updateGradeDto.QuizId.Value);
                if (quiz == null) return false;
                grade.QuizId = updateGradeDto.QuizId.Value;
            }

            if (updateGradeDto.Score.HasValue && updateGradeDto.Score.Value >= 0)
            {
                grade.Score = updateGradeDto.Score.Value;
            }

            

            return _context.SaveChanges() > 0;
        }

        public bool DeleteGrade(int gradeId)
        {
            if (gradeId <= 0) return false;

            var grade = _context.StudentGrades.Find(gradeId);
            if (grade == null) return false;

            _context.StudentGrades.Remove(grade);
            return _context.SaveChanges() > 0;
        }
    }
}