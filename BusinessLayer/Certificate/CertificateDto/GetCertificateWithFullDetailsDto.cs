using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Certificate.CertificateDto
{
    public class GetCertificateWithFullDetailsDto
    {
        public int CertificateId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateOnly IssueDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public string Grade { get; set; } = string.Empty;


        public int StudentId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }


        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
    }
}
