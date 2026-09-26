using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Classes.ClassDto
{
    public class UpdateClassDto
    {
        public int ClassId { get; set; }

        public string? ClassName { get; set; }

        public string? Branch { get; set; } 

        public string? AcademicYear { get; set; } 

        public int? Capacity { get; set; }
    }
}
