using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Quiz.QuizDto
{
    public class AddQuizDto
    {
        public int LessonId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public int PassingScore { get; set; }
    }
}
