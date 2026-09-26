using BusinessLayer.Installment.InstallmentDto;
using DataLayer.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Installment
{
    public class Installment
    {
        private readonly SiSDBDbContext _context;

        public Installment(SiSDBDbContext context)
        {
            _context = context;
        }

        public List<GetInstallmentWithStudentDetailsDto> GetAllInstallmentsWithDetails()
        {
            return _context.Installments
                .Include(i => i.Student)
                    .ThenInclude(s => s.User)
                .Select(i => new GetInstallmentWithStudentDetailsDto
                {
                    InstallmentId = i.InstallmentId,
                    StudentId = i.StudentId,
                    Amount = i.Amount,
                    DueDate = i.DueDate,
                    IsPaid = i.IsPaid,
                    FirstName = i.Student != null && i.Student.User != null ? i.Student.User.FirstName : string.Empty,
                    LastName = i.Student != null && i.Student.User != null ? i.Student.User.LastName : string.Empty,
                    Email = i.Student != null && i.Student.User != null ? i.Student.User.Email : string.Empty,
                    ClassId = i.Student != null ? i.Student.ClassId : 0,
                    IsActive = i.Student != null && i.Student.User != null ? i.Student.User.IsActive : false
                })
                .ToList();
        }

        public GetInstallmentWithStudentDetailsDto? GetInstallmentByIdWithDetails(int installmentId)
        {
            if (installmentId <= 0) return null;

            return _context.Installments
                .Where(i => i.InstallmentId == installmentId)
                .Include(i => i.Student)
                    .ThenInclude(s => s.User)
                .Select(i => new GetInstallmentWithStudentDetailsDto
                {
                    InstallmentId = i.InstallmentId,
                    StudentId = i.StudentId,
                    Amount = i.Amount,
                    DueDate = i.DueDate,
                    IsPaid = i.IsPaid,
                    FirstName = i.Student != null && i.Student.User != null ? i.Student.User.FirstName : string.Empty,
                    LastName = i.Student != null && i.Student.User != null ? i.Student.User.LastName : string.Empty,
                    Email = i.Student != null && i.Student.User != null ? i.Student.User.Email : string.Empty,
                    ClassId = i.Student != null ? i.Student.ClassId : 0,
                    IsActive = i.Student != null && i.Student.User != null ? i.Student.User.IsActive : false
                })
                .FirstOrDefault();
        }

        public GetInstallmentWithStudentDetailsDto? GetInstallmentByStudentIdWithDetails(int studentId)
        {
            if (studentId <= 0) return null;

            return _context.Installments
                .Where(i => i.StudentId == studentId)
                .Include(i => i.Student)
                    .ThenInclude(s => s.User)
                .Select(i => new GetInstallmentWithStudentDetailsDto
                {
                    InstallmentId = i.InstallmentId,
                    StudentId = i.StudentId,
                    Amount = i.Amount,
                    DueDate = i.DueDate,
                    IsPaid = i.IsPaid,
                    FirstName = i.Student != null && i.Student.User != null ? i.Student.User.FirstName : string.Empty,
                    LastName = i.Student != null && i.Student.User != null ? i.Student.User.LastName : string.Empty,
                    Email = i.Student != null && i.Student.User != null ? i.Student.User.Email : string.Empty,
                    ClassId = i.Student != null ? i.Student.ClassId : 0,
                    IsActive = i.Student != null && i.Student.User != null ? i.Student.User.IsActive : false
                })
                .FirstOrDefault(); 
        }

    }
}