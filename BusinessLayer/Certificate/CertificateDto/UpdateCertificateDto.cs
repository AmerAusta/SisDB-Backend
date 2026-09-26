using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Certificate.CertificateDto
{
    public class UpdateCertificateDto
    {
        public int CertificateId { get; set; }
        public int? StudentId { get; set; }
        public int? ClassId { get; set; }
        public string? Title { get; set; }
        public string? Grade { get; set; }
    }
}
