using BusinessLayer.Student.StudentDto;
using BusinessLayer.User;
using DataLayer.Data;
using DataLayer.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Student
{
    public class Student: User.User
    {
        private readonly SiSDBDbContext _context;

        public Student(SiSDBDbContext context) : base(context)
        {
            _context = context;
        }

        public List<GetStudentDto> GetAllStudents()
        {
            var students = _context.Students
                .Include(s => s.User)
                .Where(s => s.User != null)
                .Select(student => new GetStudentDto
                {
                    UserId = student.User.UserId,
                    FirstName = student.User.FirstName,
                    SecondName = student.User.SecondName,
                    LastName = student.User.LastName,
                    Email = student.User.Email,
                    PhoneNumber = student.User.PhoneNumber,
                    RoleId = student.User.RoleId,
                    IsActive = student.User.IsActive,
                    CreatedAt = student.User.CreatedAt,
                    StudentId = student.StudentId,
                    ClassId = student.ClassId,
                    ParentId = student.ParentId,
                    TotalContractAmount = student.TotalContractAmount,
                    EnrollmentDate = student.EnrollmentDate,
                })
                .ToList();

            return students;
        }

        public IEnumerable<GetStudentDto> GetStudentsByParentId(int parentId)
        {
            if (parentId == 0) return Enumerable.Empty<GetStudentDto>();

            var students = _context.Students
                .Include(s => s.User)
                .Where(s => s.ParentId == parentId && s.User != null)
                .Select(student => new GetStudentDto
                {
                    UserId = student.User.UserId,
                    FirstName = student.User.FirstName,
                    SecondName = student.User.SecondName,
                    LastName = student.User.LastName,
                    Email = student.User.Email,
                    PhoneNumber = student.User.PhoneNumber,
                    RoleId = student.User.RoleId,
                    IsActive = student.User.IsActive,
                    CreatedAt = student.User.CreatedAt,
                    StudentId = student.StudentId,
                    ClassId = student.ClassId,
                    ParentId = student.ParentId,
                    TotalContractAmount = student.TotalContractAmount,
                    EnrollmentDate = student.EnrollmentDate,
                })
                .ToList();

            return students;
        }

        public GetStudentDto? GetStudentById(int studentId)
        {
            if (studentId == 0) return null;

            var student = _context.Students
                .Include(s => s.User)
                .FirstOrDefault(s => s.StudentId == studentId);

            if (student == null || student.User == null)
                return null;
            else
                return new GetStudentDto
                {
                    UserId = student.User.UserId,
                    FirstName = student.User.FirstName,
                    SecondName = student.User.SecondName,
                    LastName = student.User.LastName,
                    Email = student.User.Email,
                    PhoneNumber = student.User.PhoneNumber,
                    RoleId = student.User.RoleId,
                    IsActive = student.User.IsActive,
                    CreatedAt = student.User.CreatedAt,
                    StudentId = student.StudentId,
                    ClassId = student.ClassId,
                    ParentId = student.ParentId,
                    TotalContractAmount = student.TotalContractAmount,
                    EnrollmentDate = student.EnrollmentDate,
                };

        }

        public GetStudentDto? GetStudentByUserId(int userId)
        {
            if (userId == 0) return null;

            var student = _context.Students
                .Include(s => s.User)
                .FirstOrDefault(s => s.UserId == userId);

            if (student == null || student.User == null)
                return null;
            else
                return new GetStudentDto
                {
                    UserId = student.User.UserId,
                    FirstName = student.User.FirstName,
                    SecondName = student.User.SecondName,
                    LastName = student.User.LastName,
                    Email = student.User.Email,
                    PhoneNumber = student.User.PhoneNumber,
                    RoleId = student.User.RoleId,
                    IsActive = student.User.IsActive,
                    CreatedAt = student.User.CreatedAt,
                    StudentId = student.StudentId,
                    ClassId = student.ClassId,
                    ParentId = student.ParentId,
                    TotalContractAmount = student.TotalContractAmount,
                    EnrollmentDate = student.EnrollmentDate,
                };
        }

        public List<GetStudentDto> GetStudentsByName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return new List<GetStudentDto>();

            string searchName = fullName.Trim();

            return _context.Students.Include(s => s.User)
                .Where(s => (s.User.FirstName + " " + (s.User.SecondName != null ? s.User.SecondName + " " : "") + s.User.LastName)
                    .Contains(searchName))
                .Select(s => new GetStudentDto
                {
                    UserId = s.User.UserId,
                    FirstName = s.User.FirstName,
                    SecondName = s.User.SecondName,
                    LastName = s.User.LastName,
                    Email = s.User.Email,
                    PhoneNumber = s.User.PhoneNumber,
                    RoleId = s.User.RoleId,
                    IsActive = s.User.IsActive,
                    CreatedAt = s.User.CreatedAt,
                    StudentId = s.StudentId,
                    ClassId = s.ClassId,
                    ParentId = s.ParentId,
                    TotalContractAmount = s.TotalContractAmount,
                    EnrollmentDate = s.EnrollmentDate,
                })
                .ToList();
        }

        public List<GetStudentDto> GetStudentsByClassId(int classId)
        {
            if (classId <= 0) return new List<GetStudentDto>();

            return _context.Students
                .Include(s => s.User)
                .Where(s => s.ClassId == classId)
                .Select(student => new GetStudentDto
                {
                    UserId = student.User.UserId,
                    FirstName = student.User.FirstName,
                    SecondName = student.User.SecondName,
                    LastName = student.User.LastName,
                    Email = student.User.Email,
                    PhoneNumber = student.User.PhoneNumber,
                    RoleId = student.User.RoleId,
                    IsActive = student.User.IsActive,
                    CreatedAt = student.User.CreatedAt,
                    StudentId = student.StudentId,
                    ClassId = student.ClassId,
                    ParentId = student.ParentId,
                    TotalContractAmount = student.TotalContractAmount,
                    EnrollmentDate = student.EnrollmentDate,
                })
                .ToList();
        }

        public List<int> GetStudentIdsByClassId(int classId)
        {
            if (classId <= 0) return new List<int>();

            return _context.Students
                .Where(s => s.ClassId == classId)
                .Select(s => s.StudentId)
                .ToList();
        }

        public int AddNewStudent(AddStudentDto AddStudent)
        {
            if (AddStudent == null) return 0;

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var targetClass = _context.Classes.Find(AddStudent.ClassId);

                if (targetClass == null)
                {
                    transaction.Rollback();
                    return -1;
                }

                if (targetClass.CurrentCapacity >= targetClass.Capacity)
                {
                    transaction.Rollback();
                    return -2;
                }

                if (AddStudent.ParentId.HasValue && AddStudent.ParentId.Value > 0)
                {
                    var parent = _context.Users.Find(AddStudent.ParentId.Value);
                    if (parent == null || parent.RoleId != 5)
                    {
                        transaction.Rollback();
                        return -3;
                    }
                }

                int userId = AddNewUser(AddStudent);

                if (userId == 0)
                {
                    transaction.Rollback();
                    return 0;
                }

                var student = new DataLayer.Models.Entities.Student
                {
                    UserId = userId,
                    ClassId = AddStudent.ClassId,
                    TotalContractAmount = AddStudent.TotalContractAmount,
                    EnrollmentDate = DateTime.Now,
                    ParentId = (AddStudent.ParentId == null || AddStudent.ParentId == 0) ? null : AddStudent.ParentId
                };

                _context.Students.Add(student);
                targetClass.CurrentCapacity += 1;

                _context.SaveChanges();

                if (AddStudent.TotalContractAmount > 0)
                {
                    var installment = new DataLayer.Models.Entities.Installment
                    {
                        StudentId = student.StudentId, 
                        Amount = AddStudent.TotalContractAmount,
                        DueDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
                        IsPaid = false
                    };

                    _context.Installments.Add(installment);
                }


                int rowsAffected = _context.SaveChanges();

                if (rowsAffected > 0 || student.StudentId > 0)
                {
                    transaction.Commit();
                    return student.StudentId; 
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

        public int UpdateStudent(UpdateStudentDto UpdateStudent)
        {
            if (UpdateStudent == null) return 0;

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var student = _context.Students.Find(UpdateStudent.StudentId);

                if (student == null) return 0;

                if (UpdateStudent.ParentId.HasValue && UpdateStudent.ParentId.Value > 0)
                {
                    var parent = _context.Users.Find(UpdateStudent.ParentId.Value);
                    if (parent == null || parent.RoleId != 5)
                    {
                        transaction.Rollback();
                        return -3; 
                    }
                }

                bool IsUpdate = UpdateStudentorTeacher(UpdateStudent);

                if (!IsUpdate)
                {
                    transaction.Rollback();
                    return 0;
                }

                if (UpdateStudent.ClassId.HasValue && UpdateStudent.ClassId.Value != student.ClassId)
                {
                    int oldClassId = student.ClassId;
                    int newClassId = UpdateStudent.ClassId.Value;

                    var oldClass = _context.Classes.Find(oldClassId);
                    var newClass = _context.Classes.Find(newClassId);

                    if (newClass == null)
                    {
                        transaction.Rollback();
                        return -1; 
                    }

                    if (newClass.CurrentCapacity >= newClass.Capacity)
                    {
                        transaction.Rollback();
                        return -2; 
                    }

                    if (oldClass != null && oldClass.CurrentCapacity > 0)
                    {
                        oldClass.CurrentCapacity -= 1;
                    }

                    newClass.CurrentCapacity += 1;

                    student.ClassId = newClassId;
                }


                if (UpdateStudent.ParentId.HasValue)
                {
                    student.ParentId = UpdateStudent.ParentId.Value == 0 ? null : UpdateStudent.ParentId.Value;
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
                Console.WriteLine(ex.Message);
                transaction.Rollback();
                return 0;
            }
        }

        public bool DeleteStudent(int StudentId)
        {
            if (StudentId == 0) return false;

            try
            {
                var Student = _context.Students.Find(StudentId);

                if (Student == null) return false;

                bool IsDeleted = DeleteUser(Student.UserId);

                return IsDeleted;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


    }
}
