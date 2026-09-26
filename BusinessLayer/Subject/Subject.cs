using BusinessLayer.Subject.SubjectDto;
using DataLayer.Data;
using DataLayer.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Subject
{
    public class Subject
    {
        private readonly SiSDBDbContext _context;

        public Subject(SiSDBDbContext context)
        {
            _context = context;
        }

        public GetSubjectDto? GetSubjectById(int subjectId)
        {
            if (subjectId <= 0) return null;

            var subject = _context.Subjects.Find(subjectId);
            if (subject == null) return null;

            return new GetSubjectDto
            {
                SubjectId = subject.SubjectId,
                SubjectName = subject.SubjectName,
                Description = subject.Description
            };
        }

        public List<GetSubjectDto> GetSubjectsByName(string subjectName)
        {
            if (string.IsNullOrWhiteSpace(subjectName))
                return new List<GetSubjectDto>();

            string searchName = subjectName.Trim();

            return _context.Subjects
                .Where(s => s.SubjectName.Contains(searchName))
                .Select(s => new GetSubjectDto
                {
                    SubjectId = s.SubjectId,
                    SubjectName = s.SubjectName,
                    Description = s.Description
                })
                .ToList();
        }

        public List<GetSubjectDto> GetAllSubjects()
        {
            return _context.Subjects
                .Select(s => new GetSubjectDto
                {
                    SubjectId = s.SubjectId,
                    SubjectName = s.SubjectName,
                    Description = s.Description
                })
                .ToList();
        }

        public bool IsSubjectNameExists(string subjectName, int excludeSubjectId = 0)
        {

            return _context.Subjects.Any(s => s.SubjectName.ToLower() == subjectName.ToLower() && s.SubjectId != excludeSubjectId);
        }

        public bool SubjectExists(int subjectId)
        {
            return _context.Subjects.Any(s => s.SubjectId == subjectId);
        }

        public int AddNewSubject(AddSubjectDto addSubjectDto)
        {
            if (addSubjectDto == null) return 0;

            var subject = new DataLayer.Models.Entities.Subject
            {
                SubjectName = addSubjectDto.SubjectName,
                Description = addSubjectDto.Description
            };

            _context.Subjects.Add(subject);
            int rowsAffected = _context.SaveChanges();

            return rowsAffected > 0 ? subject.SubjectId : 0;
        }

        public bool UpdateSubject(UpdateSubjectDto updateSubjectDto)
        {
            if (updateSubjectDto == null || updateSubjectDto.SubjectId <= 0) return false;

            var subject = _context.Subjects.Find(updateSubjectDto.SubjectId);
            if (subject == null) return false;

            if (!string.IsNullOrWhiteSpace(updateSubjectDto.SubjectName))
            {
                subject.SubjectName = updateSubjectDto.SubjectName;
            }

            if (!string.IsNullOrWhiteSpace(updateSubjectDto.Description))
            {
                subject.Description = updateSubjectDto.Description;
            }


            return _context.SaveChanges() > 0;
        }

        public bool DeleteSubject(int subjectId)
        {
            if (subjectId <= 0) return false;

            var subject = _context.Subjects.Find(subjectId);
            if (subject == null) return false;

            _context.Subjects.Remove(subject);
            return _context.SaveChanges() > 0;
        }


    }
}