using BusinessLayer.QuestionChoice.QuestionChoiceDto;
using DataLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.QuestionChoice
{
    public class QuestionChoice
    {
        private readonly SiSDBDbContext _context;

        public QuestionChoice(SiSDBDbContext context)
        {
            _context = context;
        }

        public GetQuestionChoiceDto? GetChoiceById(int choiceId)
        {
            if (choiceId <= 0) return null;

            var choice = _context.QuestionChoices.Find(choiceId);
            if (choice == null) return null;

            return new GetQuestionChoiceDto
            {
                ChoiceId = choice.ChoiceId,
                QuestionId = choice.QuestionId,
                ChoiceText = choice.ChoiceText,
                IsCorrect = choice.IsCorrect
            };
        }

        public List<GetQuestionChoiceDto> GetChoicesByQuestionId(int questionId)
        {
            if (questionId <= 0) return new List<GetQuestionChoiceDto>();

            return _context.QuestionChoices
                .Where(c => c.QuestionId == questionId)
                .Select(c => new GetQuestionChoiceDto
                {
                    ChoiceId = c.ChoiceId,
                    QuestionId = c.QuestionId,
                    ChoiceText = c.ChoiceText,
                    IsCorrect = c.IsCorrect
                })
                .ToList();
        }

        public List<GetQuestionChoiceDto> GetAllChoices()
        {
            return _context.QuestionChoices
                .Select(c => new GetQuestionChoiceDto
                {
                    ChoiceId = c.ChoiceId,
                    QuestionId = c.QuestionId,
                    ChoiceText = c.ChoiceText,
                    IsCorrect = c.IsCorrect
                })
                .ToList();
        }

        public int AddNewChoice(AddQuestionChoiceDto addChoiceDto)
        {
            if (addChoiceDto == null) return 0;

            var question = _context.Questions.Find(addChoiceDto.QuestionId);
            if (question == null) return -1;

            var choice = new DataLayer.Models.Entities.QuestionChoice
            {
                QuestionId = addChoiceDto.QuestionId,
                ChoiceText = addChoiceDto.ChoiceText,
                IsCorrect = addChoiceDto.IsCorrect
            };

            _context.QuestionChoices.Add(choice);
            int rowsAffected = _context.SaveChanges();

            return rowsAffected > 0 ? choice.ChoiceId : 0;
        }

        public bool UpdateChoice(UpdateQuestionChoiceDto updateChoiceDto)
        {
            if (updateChoiceDto == null || updateChoiceDto.ChoiceId <= 0) return false;

            var choice = _context.QuestionChoices.Find(updateChoiceDto.ChoiceId);
            if (choice == null) return false;

            if (updateChoiceDto.QuestionId.HasValue && updateChoiceDto.QuestionId.Value > 0)
            {
                var question = _context.Questions.Find(updateChoiceDto.QuestionId.Value);
                if (question == null) return false;
                choice.QuestionId = updateChoiceDto.QuestionId.Value;
            }

            if (!string.IsNullOrWhiteSpace(updateChoiceDto.ChoiceText))
            {
                choice.ChoiceText = updateChoiceDto.ChoiceText;
            }

            if (updateChoiceDto.IsCorrect.HasValue)
            {
                choice.IsCorrect = updateChoiceDto.IsCorrect.Value;
            }

            return _context.SaveChanges() > 0;
        }

        public bool DeleteChoice(int choiceId)
        {
            if (choiceId <= 0) return false;

            var choice = _context.QuestionChoices.Find(choiceId);
            if (choice == null) return false;

            _context.QuestionChoices.Remove(choice);
            return _context.SaveChanges() > 0;
        }
    }
}