using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Question.QuestionDto
{
    public class AddQuestionDto
    {
        public int QuizId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int Points { get; set; }
    }
}
