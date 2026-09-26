using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Installment.InstallmentDto
{
    public class GetInstallmentWithStudentDetailsDto
    {
        public int InstallmentId { get; set; }
        public int StudentId { get; set; }
        public decimal Amount { get; set; }
        public DateOnly DueDate { get; set; }
        public bool IsPaid { get; set; }

        
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public bool IsActive { get; set; }
    }
}
