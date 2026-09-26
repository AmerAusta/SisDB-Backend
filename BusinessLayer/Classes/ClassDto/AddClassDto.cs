using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Classes.ClassDto
{
    public class AddClassDto
    {
        public string ClassName { get; set; } = null!;

        public string Branch { get; set; } = null!;

        public string AcademicYear { get; set; } = null!;

        public int Capacity { get; set; }
    }
}
