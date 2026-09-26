using BusinessLayer.Certificate.CertificateDto;
using DataLayer.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Certificate
{
    public class Certificate
    {
        private readonly SiSDBDbContext _context; 

        public Certificate(SiSDBDbContext context)
        {
            _context = context;
        }

        public List<GetCertificateDto> GetAllCertificates()
        {
            return _context.Certificates
                .Select(c => new GetCertificateDto
                {
                    CertificateId = c.CertificateId,
                    StudentId = c.StudentId,
                    ClassId = c.ClassId,
                    Title = c.Title,
                    IssueDate = c.IssueDate,
                    Grade = c.Grade
                })
                .ToList();
        }


        public GetCertificateDto? GetCertificateById(int certificateId)
        {
            if (certificateId <= 0) return null;

            return _context.Certificates
                .Where(c => c.CertificateId == certificateId)
                .Select(c => new GetCertificateDto
                {
                    CertificateId = c.CertificateId,
                    StudentId = c.StudentId,
                    ClassId = c.ClassId,
                    Title = c.Title,
                    IssueDate = c.IssueDate,
                    Grade = c.Grade
                })
                .FirstOrDefault();
        }

        public List<GetCertificateDto> GetCertificatesByStudentId(int studentId)
        {
            if (studentId <= 0) return new List<GetCertificateDto>();

            return _context.Certificates
                .Where(c => c.StudentId == studentId)
                .Select(c => new GetCertificateDto
                {
                    CertificateId = c.CertificateId,
                    StudentId = c.StudentId,
                    ClassId = c.ClassId,
                    Title = c.Title,
                    IssueDate = c.IssueDate,
                    Grade = c.Grade
                })
                .ToList();
        }

        public List<GetCertificateWithFullDetailsDto> GetCertificatesWithFullDetailsByStudentId(int studentId)
        {
            if (studentId <= 0) return new List<GetCertificateWithFullDetailsDto>();

            return _context.Certificates
                .Where(c => c.StudentId == studentId)
                .Join(_context.Students, cert => cert.StudentId, s => s.StudentId, (cert, s) => new { cert, s })
                .Join(_context.Users, temp => temp.s.UserId, u => u.UserId, (temp, u) => new { temp.cert, temp.s, u })
                .Join(_context.Classes, temp => temp.s.ClassId, cl => cl.ClassId, (temp, cl) => new GetCertificateWithFullDetailsDto
                {
                    CertificateId = temp.cert.CertificateId,
                    Title = temp.cert.Title,
                    IssueDate = temp.cert.IssueDate,
                    Grade = temp.cert.Grade,

                    StudentId = temp.s.StudentId,
                    UserId = temp.u.UserId,
                    FirstName = temp.u.FirstName,
                    SecondName = temp.u.SecondName,
                    LastName = temp.u.LastName,
                    Email = temp.u.Email,
                    PhoneNumber = temp.u.PhoneNumber,
                    EnrollmentDate = temp.s.EnrollmentDate,

                    ClassId = cl.ClassId,
                    ClassName = cl.ClassName,
                    Branch = cl.Branch
                })
                .ToList();
        }

        public List<GetCertificateWithFullDetailsDto> GetAllCertificatesWithFullDetails()
        {
            return _context.Certificates
                .Join(_context.Students, cert => cert.StudentId, s => s.StudentId, (cert, s) => new { cert, s })
                .Join(_context.Users, temp => temp.s.UserId, u => u.UserId, (temp, u) => new { temp.cert, temp.s, u })
                .Join(_context.Classes, temp => temp.s.ClassId, cl => cl.ClassId, (temp, cl) => new GetCertificateWithFullDetailsDto
                {
                    CertificateId = temp.cert.CertificateId,
                    Title = temp.cert.Title,
                    IssueDate = temp.cert.IssueDate,
                    Grade = temp.cert.Grade,

                    StudentId = temp.s.StudentId,
                    UserId = temp.u.UserId,
                    FirstName = temp.u.FirstName,
                    SecondName = temp.u.SecondName,
                    LastName = temp.u.LastName,
                    Email = temp.u.Email,
                    PhoneNumber = temp.u.PhoneNumber,
                    EnrollmentDate = temp.s.EnrollmentDate,

                    ClassId = cl.ClassId,
                    ClassName = cl.ClassName,
                    Branch = cl.Branch
                })
                .ToList();
        }

        public int AddNewCertificate(AddCertificateDto addDto)
        {
            if (addDto == null) return 0;

            bool studentExists = _context.Students.Any(s => s.StudentId == addDto.StudentId);
            if (!studentExists) return -1;

            bool classExists = _context.Classes.Any(c => c.ClassId == addDto.ClassId);
            if (!classExists) return -2;

            bool certificateExists = _context.Certificates.Any(c =>
                c.StudentId == addDto.StudentId &&
                c.ClassId == addDto.ClassId &&
                c.Title.ToLower() == addDto.Title.ToLower());

            if (certificateExists) return -3;


            if (!string.IsNullOrEmpty(addDto.Grade) && addDto.Grade.Length > 10)
            {
                return -4; 
            }

            
            if (!int.TryParse(addDto.Grade, out int gradeValue) || gradeValue < 0 || gradeValue > 100)
            {
                return -5; 
            }

            var certificate = new DataLayer.Models.Entities.Certificate
            {
                StudentId = addDto.StudentId,
                ClassId = addDto.ClassId,
                Title = addDto.Title,
                IssueDate = addDto.IssueDate,
                Grade = addDto.Grade
            };

            _context.Certificates.Add(certificate);
            bool isSaved = _context.SaveChanges() > 0;

            return isSaved ? certificate.CertificateId : 0;
        }

        public bool UpdateCertificate(UpdateCertificateDto updateDto)
        {
            if (updateDto == null || updateDto.CertificateId <= 0) return false;

            var certificate = _context.Certificates.Find(updateDto.CertificateId);
            if (certificate == null) return false;


            if (updateDto.StudentId.HasValue && updateDto.StudentId.Value > 0)
            {
                bool studentExists = _context.Students.Any(s => s.StudentId == updateDto.StudentId.Value);
                if (!studentExists) return false;
                certificate.StudentId = updateDto.StudentId.Value;
            }


            if (updateDto.ClassId.HasValue && updateDto.ClassId.Value > 0)
            {
                bool classExists = _context.Classes.Any(c => c.ClassId == updateDto.ClassId.Value);
                if (!classExists) return false;
                certificate.ClassId = updateDto.ClassId.Value;
            }


            if (!string.IsNullOrWhiteSpace(updateDto.Title))
            {
                certificate.Title = updateDto.Title;
            }


            if (!string.IsNullOrWhiteSpace(updateDto.Grade))
            {
                certificate.Grade = updateDto.Grade;
            }

            _context.Certificates.Update(certificate);
            return _context.SaveChanges() > 0;
        }


        public bool DeleteCertificate(int certificateId)
        {
            if (certificateId <= 0) return false;

            var certificate = _context.Certificates.Find(certificateId);
            if (certificate == null) return false;

            _context.Certificates.Remove(certificate);
            return _context.SaveChanges() > 0;
        }
    }
}