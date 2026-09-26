using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Certificate.CertificateDto
{
    public class GetCertificateDto
    {
        public int CertificateId { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateOnly IssueDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public string Grade { get; set; } = string.Empty;
    }
}
