using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.StudentGrade.StudentGradeDto
{
    public class UpdateStudentGradeDto
    {
        public int GradeId { get; set; }
        public int? StudentId { get; set; }
        public int? QuizId { get; set; }
        public int? Score { get; set; }
       
    }
}