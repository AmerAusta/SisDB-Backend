using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Payment.PaymentDto
{
    public class AddPaymentDto
    {
        public int StudentId { get; set; }
        public int InstallmentId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public int SecretaryId { get; set; }
    }
}
