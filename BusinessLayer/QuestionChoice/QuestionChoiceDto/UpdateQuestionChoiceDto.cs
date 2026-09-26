using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.QuestionChoice.QuestionChoiceDto
{
    public class UpdateQuestionChoiceDto
    {
        public int ChoiceId { get; set; }
        public int? QuestionId { get; set; }
        public string? ChoiceText { get; set; }
        public bool? IsCorrect { get; set; }
    }
}
