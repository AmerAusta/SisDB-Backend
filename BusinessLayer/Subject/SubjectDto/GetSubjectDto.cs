using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Subject.SubjectDto
{
    public class GetSubjectDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } 
        public string? Description { get; set; }
    }
}
