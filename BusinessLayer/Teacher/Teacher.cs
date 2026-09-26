using BusinessLayer.Student;
using BusinessLayer.Teacher.TeacherDto;
using BusinessLayer.User;
using DataLayer.Data;
using DataLayer.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Teacher
{
    public class Teacher : User.User
    {
        private readonly SiSDBDbContext _context;

        public Teacher(SiSDBDbContext context) : base(context)
        {
            _context = context;
        }

        public List<GetTeacherDto> GetAllTeachers()
        {
            var teachers = _context.Teachers
                .Include(t => t.User)
                .Where(t => t.User != null) 
                .Select(teacher => new GetTeacherDto
                {
                    UserId = teacher.User.UserId,
                    FirstName = teacher.User.FirstName,
                    SecondName = teacher.User.SecondName,
                    LastName = teacher.User.LastName,
                    Email = teacher.User.Email,
                    PhoneNumber = teacher.User.PhoneNumber,
                    RoleId = teacher.User.RoleId,
                    IsActive = teacher.User.IsActive,
                    CreatedAt = teacher.User.CreatedAt,
                    TeacherId = teacher.TeacherId,
                    SubjectId = teacher.SubjectId,
                    Salary = teacher.Salary,
                    HireDate = teacher.HireDate
                })
                .ToList();

            return teachers;
        }

        public GetTeacherDto? GetTeacherById(int teacherId)
        {
            if (teacherId == 0) return null;

            var teacher = _context.Teachers
                .Include(t => t.User)
                .FirstOrDefault(t => t.TeacherId == teacherId);

            if (teacher == null || teacher.User == null)
                return null;
            else
                return new GetTeacherDto
                {
                    UserId = teacher.User.UserId,
                    FirstName = teacher.User.FirstName,
                    SecondName = teacher.User.SecondName,
                    LastName = teacher.User.LastName,
                    Email = teacher.User.Email,
                    PhoneNumber = teacher.User.PhoneNumber,
                    RoleId = teacher.User.RoleId,
                    IsActive = teacher.User.IsActive,
                    CreatedAt = teacher.User.CreatedAt,
                    TeacherId = teacher.TeacherId,
                    SubjectId = teacher.SubjectId,
                    Salary = teacher.Salary,
                    HireDate = teacher.HireDate
                };
        }

        public GetTeacherDto? GetTeacherByUserId(int userId)
        {
            if (userId == 0) return null;

            var teacher = _context.Teachers
                .Include(t => t.User)
                .FirstOrDefault(t => t.User.UserId == userId); 

            if (teacher == null || teacher.User == null)
                return null;
            else
                return new GetTeacherDto
                {
                    UserId = teacher.User.UserId,
                    FirstName = teacher.User.FirstName,
                    SecondName = teacher.User.SecondName,
                    LastName = teacher.User.LastName,
                    Email = teacher.User.Email,
                    PhoneNumber = teacher.User.PhoneNumber,
                    RoleId = teacher.User.RoleId,
                    IsActive = teacher.User.IsActive,
                    CreatedAt = teacher.User.CreatedAt,
                    TeacherId = teacher.TeacherId,
                    SubjectId = teacher.SubjectId,
                    Salary = teacher.Salary,
                    HireDate = teacher.HireDate
                };
        }

        public List<GetTeacherDto> GetTeachersByName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return new List<GetTeacherDto>();

            string searchName = fullName.Trim();

            return _context.Teachers.Include(t => t.User)
                .Where(t => (t.User.FirstName + " " + (t.User.SecondName != null ? t.User.SecondName + " " : "") + t.User.LastName)
                    .Contains(searchName))
                .Select(t => new GetTeacherDto
                {
                    UserId = t.User.UserId,
                    FirstName = t.User.FirstName,
                    SecondName = t.User.SecondName,
                    LastName = t.User.LastName,
                    Email = t.User.Email,
                    PhoneNumber = t.User.PhoneNumber,
                    RoleId = t.User.RoleId,
                    IsActive = t.User.IsActive,
                    CreatedAt = t.User.CreatedAt,
                    TeacherId = t.TeacherId,
                    SubjectId = t.SubjectId,
                    Salary = t.Salary,
                    HireDate = t.HireDate
                })
                .ToList();
        }

        public List<GetTeacherDto> GetTeachersBySubjectId(int subjectId)
        {
            if (subjectId <= 0)
                return new List<GetTeacherDto>();

            bool exists = _context.Subjects.Any(s => s.SubjectId == subjectId);
            if (!exists)
            {
                return new List<GetTeacherDto>();
            }

            return _context.Teachers
                .Include(t => t.User)
                .Where(t => t.SubjectId == subjectId)
                .Select(t => new GetTeacherDto
                {
                    UserId = t.User.UserId,
                    FirstName = t.User.FirstName,
                    SecondName = t.User.SecondName,
                    LastName = t.User.LastName,
                    Email = t.User.Email,
                    PhoneNumber = t.User.PhoneNumber,
                    RoleId = t.User.RoleId,
                    IsActive = t.User.IsActive,
                    CreatedAt = t.User.CreatedAt,
                    TeacherId = t.TeacherId,
                    SubjectId = t.SubjectId,
                    Salary = t.Salary,
                    HireDate = t.HireDate
                })
                .ToList();
        }

        public int AddNewTeacher(AddTeacherDto addTeacherDto)
        {
            if (addTeacherDto == null) return 0;

            using var transaction = _context.Database.BeginTransaction();

            try
            {

                var targetSubject = _context.Subjects.Find(addTeacherDto.SubjectId);
                if (targetSubject == null)
                {
                    transaction.Rollback();
                    return -1; 
                }

                int userId = AddNewUser(addTeacherDto);

                if (userId == 0)
                {
                    transaction.Rollback();
                    return 0;
                }

                var teacher = new DataLayer.Models.Entities.Teacher
                {
                    UserId = userId,
                    SubjectId = addTeacherDto.SubjectId,
                    Salary = addTeacherDto.Salary,
                    HireDate = addTeacherDto.HireDate == default ? DateTime.Now : addTeacherDto.HireDate
                };

                _context.Teachers.Add(teacher);

                int rowsAffected = _context.SaveChanges();

                if (rowsAffected > 0)
                {
                    transaction.Commit();
                    return teacher.TeacherId;
                }

                transaction.Rollback();
                return 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return 0;
            }
        }

        public int UpdateTeacher(UpdateTeacherDto updateTeacherDto)
        {
            if (updateTeacherDto == null) return 0;

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var teacher = _context.Teachers.Find(updateTeacherDto.TeacherId);

                if (teacher == null) return 0;

                bool IsUpdate = UpdateStudentorTeacher(updateTeacherDto);

                if (!IsUpdate)
                {
                    transaction.Rollback();
                    return 0;
                }


                if (updateTeacherDto.SubjectId.HasValue && updateTeacherDto.SubjectId.Value != teacher.SubjectId)
                {
                    var targetSubject = _context.Subjects.Find(updateTeacherDto.SubjectId.Value);
                    if (targetSubject == null)
                    {
                        transaction.Rollback();
                        return -1; 
                    }
                    teacher.SubjectId = updateTeacherDto.SubjectId.Value;
                }

                if (updateTeacherDto.Salary.HasValue)
                {
                    teacher.Salary = updateTeacherDto.Salary.Value;
                }

                if (updateTeacherDto.HireDate.HasValue)
                {
                    teacher.HireDate = updateTeacherDto.HireDate.Value;
                }

                int rowsAffected = _context.SaveChanges();

                if (rowsAffected > 0)
                {
                    transaction.Commit();
                    return 1;
                }

                transaction.Rollback();
                return 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return 0;
            }
        }

        public bool DeleteTeacher(int teacherId)
        {

            if (teacherId == 0) return false;

            try
            {
                var teacher = _context.Teachers.Find(teacherId);

                if (teacher == null) return false;

                bool IsDeleted = DeleteUser(teacher.UserId);

                return IsDeleted;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool IsTeacher(int teacherId)
        {
            if (teacherId <= 0) return false;

            const int teacherRoleId = 3;

            return _context.Teachers
                .Any(t => t.TeacherId == teacherId &&
                          t.User.IsActive &&
                          t.User.RoleId == teacherRoleId);
        }

        public bool IsTeacherhaveThisSubject(int subjectId)
        {
            if (subjectId <= 0)
                return false;

            return _context.Teachers.Any(t => t.SubjectId == subjectId);
        }
    }
}