using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Question.QuestionDto
{
    public class UpdateQuestionDto
    {
        public int QuestionId { get; set; }
        public int? QuizId { get; set; }
        public string? QuestionText { get; set; }
        public int? Points { get; set; }
    }
}
