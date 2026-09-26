using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Payment.PaymentDto
{
    public class GetPaymentWithDetailsDto
    {
        public int PaymentId { get; set; }
        public int StudentId { get; set; }
        public int InstallmentId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public int SecretaryId { get; set; }


        public string StudentFirstName { get; set; } = string.Empty;
        public string StudentLastName { get; set; } = string.Empty;

        public string SecretaryFirstName { get; set; } = string.Empty;
        public string SecretaryLastName { get; set; } = string.Empty;


        public decimal RemainingInstallmentAmount { get; set; }
    }
}
