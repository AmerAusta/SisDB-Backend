using BusinessLayer.Assignments.AssignmentDto;
using DataLayer.Data;
using DataLayer.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Assignments
{
    public class Assignment
    {
        private readonly SiSDBDbContext _context;

        public Assignment(SiSDBDbContext context)
        {
            _context = context;
        }

        public GetAssignmentDto? GetAssignmentById(int assignmentId)
        {
            if (assignmentId <= 0) return null;

            return _context.Assignments
              .Include(a => a.Teacher)
              .ThenInclude(t => t.User) 
              .Include(a => a.Subject)
              .Include(a => a.Class)
              .Where(a => a.AssignmentId == assignmentId)
              .Select(a => new GetAssignmentDto
            {
               AssignmentId = a.AssignmentId,
               TeacherId = a.TeacherId,
               TeacherName = a.Teacher != null && a.Teacher.User != null ? $"{a.Teacher.User.FirstName} {a.Teacher.User.LastName}" : null, // <--- الوصول للأسماء عبر a.Teacher.User
               SubjectId = a.SubjectId,
               SubjectName = a.Subject != null ? a.Subject.SubjectName : null,
               ClassId = a.ClassId,
               ClassName = a.Class != null ? a.Class.ClassName : null
            })
            .FirstOrDefault();
        }

        public List<GetAssignmentDto> GetAllAssignments()
        {
            return _context.Assignments
           .Include(a => a.Teacher)
           .ThenInclude(t => t.User) // <--- ضروري لجلب بيانات المستخدم التابع للمدرس
           .Include(a => a.Subject)
           .Include(a => a.Class)
           .Select(a => new GetAssignmentDto
           {
               AssignmentId = a.AssignmentId,
               TeacherId = a.TeacherId,
               TeacherName = a.Teacher != null && a.Teacher.User != null ? $"{a.Teacher.User.FirstName} {a.Teacher.User.LastName}" : null, // <--- التعديل هنا للوصول إلى اسم المستخدم
               SubjectId = a.SubjectId,
               SubjectName = a.Subject != null ? a.Subject.SubjectName : null,
               ClassId = a.ClassId,
               ClassName = a.Class != null ? a.Class.ClassName : null
           })
             .ToList();
        }

        public List<GetAssignmentDto> GetAssignmentsByTeacherId(int teacherId)
        {
            if (teacherId <= 0) return new List<GetAssignmentDto>();

            return _context.Assignments
           .Include(a => a.Teacher)
           .ThenInclude(t => t.User) 
           .Include(a => a.Subject)
           .Include(a => a.Class)
           .Select(a => new GetAssignmentDto
           {
               AssignmentId = a.AssignmentId,
               TeacherId = a.TeacherId,
               TeacherName = a.Teacher != null && a.Teacher.User != null ? $"{a.Teacher.User.FirstName} {a.Teacher.User.LastName}" : null, // <--- التعديل هنا للوصول إلى اسم المستخدم
               SubjectId = a.SubjectId,
               SubjectName = a.Subject != null ? a.Subject.SubjectName : null,
               ClassId = a.ClassId,
               ClassName = a.Class != null ? a.Class.ClassName : null
           })
             .ToList();
        }

        public List<GetAssignmentDto> GetAssignmentsByClassId(int classId)
        {
            if (classId <= 0) return new List<GetAssignmentDto>();

            return _context.Assignments
            .Include(a => a.Teacher)
            .ThenInclude(t => t.User) 
            .Include(a => a.Subject)
            .Include(a => a.Class)
            .Select(a => new GetAssignmentDto
             {
               AssignmentId = a.AssignmentId,
               TeacherId = a.TeacherId,
               TeacherName = a.Teacher != null && a.Teacher.User != null ? $"{a.Teacher.User.FirstName} {a.Teacher.User.LastName}" : null, // <--- التعديل هنا للوصول إلى اسم المستخدم
               SubjectId = a.SubjectId,
               SubjectName = a.Subject != null ? a.Subject.SubjectName : null,
               ClassId = a.ClassId,
               ClassName = a.Class != null ? a.Class.ClassName : null
             })
              .ToList();
        }

        public bool IsAssignmentExists(int teacherId, int subjectId, int classId, int excludeAssignmentId = 0)
        {
            return _context.Assignments.Any(a =>
                a.TeacherId == teacherId &&
                a.SubjectId == subjectId &&
                a.ClassId == classId &&
                a.AssignmentId != excludeAssignmentId);
        }

        public int AddNewAssignment(AddAssignmentDto addAssignmentDto)
        {
            if (addAssignmentDto == null) return 0;
            if (addAssignmentDto.TeacherId <= 0 || addAssignmentDto.SubjectId <= 0 || addAssignmentDto.ClassId <= 0) return 0;


            bool teacherExists = _context.Users.Any(u => u.UserId == addAssignmentDto.TeacherId);
            bool subjectExists = _context.Subjects.Any(s => s.SubjectId == addAssignmentDto.SubjectId);
            bool classExists = _context.Classes.Any(c => c.ClassId == addAssignmentDto.ClassId);

            if (!teacherExists || !subjectExists || !classExists) return 0;

            var assignment = new DataLayer.Models.Entities.Assignment
            {
                TeacherId = addAssignmentDto.TeacherId,
                SubjectId = addAssignmentDto.SubjectId,
                ClassId = addAssignmentDto.ClassId
            };

            _context.Assignments.Add(assignment);
            int rowsAffected = _context.SaveChanges();

            return rowsAffected > 0 ? assignment.AssignmentId : 0;
        }

        public bool UpdateAssignment(UpdateAssignmentDto updateAssignmentDto)
        {
            if (updateAssignmentDto == null || updateAssignmentDto.AssignmentId <= 0) return false;

            var assignment = _context.Assignments.Find(updateAssignmentDto.AssignmentId);
            if (assignment == null) return false;

            int targetTeacherId = updateAssignmentDto.TeacherId ?? assignment.TeacherId;
            int targetSubjectId = updateAssignmentDto.SubjectId ?? assignment.SubjectId;
            int targetClassId = updateAssignmentDto.ClassId ?? assignment.ClassId;

            if (IsAssignmentExists(targetTeacherId, targetSubjectId, targetClassId, updateAssignmentDto.AssignmentId))
            {
                return false;
            }

            bool teacherExists = _context.Users.Any(u => u.UserId == targetTeacherId);
            bool subjectExists = _context.Subjects.Any(s => s.SubjectId == targetSubjectId);
            bool classExists = _context.Classes.Any(c => c.ClassId == targetClassId);

            if (!teacherExists || !subjectExists || !classExists) return false;

 
            assignment.TeacherId = targetTeacherId;
            assignment.SubjectId = targetSubjectId;
            assignment.ClassId = targetClassId;

            return _context.SaveChanges() > 0;
        }

        public bool DeleteAssignment(int assignmentId)
        {
            if (assignmentId <= 0) return false;

            var assignment = _context.Assignments.Find(assignmentId);
            if (assignment == null) return false;

            _context.Assignments.Remove(assignment);
            return _context.SaveChanges() > 0;
        }

        public bool IsAssigmenthaveThisSubject(int subjectId)
        {
            if (subjectId <= 0)
                return false;

            return _context.Assignments.Any(a => a.SubjectId == subjectId);
        }
    }
}