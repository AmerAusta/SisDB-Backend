using BusinessLayer.Quiz.QuizDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.StudentGrade.StudentGradeDto
{
    public class GetStudentGradeWithQuizDetailsDto:GetQuizDto
    {
        public int GradeId { get; set; }
        public int StudentId { get; set; }
        public int Score { get; set; }
        public int AttemptNumber { get; set; }
        public DateTime DateAttempted { get; set; }
        public bool IsPassed { get; set; }
    }
}
