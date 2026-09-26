using BusinessLayer.Payment.PaymentDto;
using DataLayer.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Payment
{
    public class Payment
    {
        private readonly SiSDBDbContext _context;

        public Payment(SiSDBDbContext context)
        {
            _context = context;
        }

        public List<GetPaymentWithDetailsDto> GetAllPaymentsWithDetails()
        {
            return _context.Payments
                .Include(p => p.Student)
                    .ThenInclude(s => s.User)
                .Include(p => p.Installment)
                .Join(_context.Users,
                    p => p.SecretaryId,
                    u => u.UserId,
                    (p, secretary) => new { p, secretary })
                .Select(x => new GetPaymentWithDetailsDto
                {
                    PaymentId = x.p.PaymentId,
                    StudentId = x.p.StudentId,
                    InstallmentId = x.p.InstallmentId,
                    AmountPaid = x.p.AmountPaid,
                    PaymentDate = x.p.PaymentDate,
                    SecretaryId = x.p.SecretaryId,
                    StudentFirstName = x.p.Student != null && x.p.Student.User != null ? x.p.Student.User.FirstName : string.Empty,
                    StudentLastName = x.p.Student != null && x.p.Student.User != null ? x.p.Student.User.LastName : string.Empty,
                    SecretaryFirstName = x.secretary != null ? x.secretary.FirstName : string.Empty,
                    SecretaryLastName = x.secretary != null ? x.secretary.LastName : string.Empty,
                    RemainingInstallmentAmount = x.p.Installment != null ? x.p.Installment.Amount : 0
                })
                .ToList();
        }

        public GetPaymentWithDetailsDto? GetPaymentByIdWithDetails(int paymentId)
        {
            if (paymentId <= 0) return null;

            return _context.Payments
                .Where(p => p.PaymentId == paymentId)
                .Include(p => p.Student)
                    .ThenInclude(s => s.User)
                .Include(p => p.Installment)
                .Join(_context.Users,
                    p => p.SecretaryId,
                    u => u.UserId,
                    (p, secretary) => new { p, secretary })
                .Where(x => x.p.PaymentId == paymentId)
                .Select(x => new GetPaymentWithDetailsDto
                {
                    PaymentId = x.p.PaymentId,
                    StudentId = x.p.StudentId,
                    InstallmentId = x.p.InstallmentId,
                    AmountPaid = x.p.AmountPaid,
                    PaymentDate = x.p.PaymentDate,
                    SecretaryId = x.p.SecretaryId,
                    StudentFirstName = x.p.Student != null && x.p.Student.User != null ? x.p.Student.User.FirstName : string.Empty,
                    StudentLastName = x.p.Student != null && x.p.Student.User != null ? x.p.Student.User.LastName : string.Empty,
                    SecretaryFirstName = x.secretary != null ? x.secretary.FirstName : string.Empty,
                    SecretaryLastName = x.secretary != null ? x.secretary.LastName : string.Empty,
                    RemainingInstallmentAmount = x.p.Installment != null ? x.p.Installment.Amount : 0
                })
                .FirstOrDefault();
        }

        public List<GetPaymentWithDetailsDto> GetPaymentsByStudentIdWithDetails(int studentId)
        {
            if (studentId <= 0) return new List<GetPaymentWithDetailsDto>();

            return _context.Payments
                .Where(p => p.StudentId == studentId)
                .Include(p => p.Student)
                    .ThenInclude(s => s.User)
                .Include(p => p.Installment)
                .Join(_context.Users,
                    p => p.SecretaryId,
                    u => u.UserId,
                    (p, secretary) => new { p, secretary })
                .Where(x => x.p.StudentId == studentId)
                .Select(x => new GetPaymentWithDetailsDto
                {
                    PaymentId = x.p.PaymentId,
                    StudentId = x.p.StudentId,
                    InstallmentId = x.p.InstallmentId,
                    AmountPaid = x.p.AmountPaid,
                    PaymentDate = x.p.PaymentDate,
                    SecretaryId = x.p.SecretaryId,
                    StudentFirstName = x.p.Student != null && x.p.Student.User != null ? x.p.Student.User.FirstName : string.Empty,
                    StudentLastName = x.p.Student != null && x.p.Student.User != null ? x.p.Student.User.LastName : string.Empty,
                    SecretaryFirstName = x.secretary != null ? x.secretary.FirstName : string.Empty,
                    SecretaryLastName = x.secretary != null ? x.secretary.LastName : string.Empty,
                    RemainingInstallmentAmount = x.p.Installment != null ? x.p.Installment.Amount : 0
                })
                .ToList();
        }

        public int AddNewPayment(AddPaymentDto addDto)
        {
            if (addDto == null || addDto.AmountPaid <= 0) return 0;

            using var transaction = _context.Database.BeginTransaction();
            try
            {

                if (!_context.Students.Any(s => s.StudentId == addDto.StudentId))
                {
                    transaction.Rollback();
                    return -1; 
                }


                var installment = _context.Installments.Find(addDto.InstallmentId);
                if (installment == null)
                {
                    transaction.Rollback();
                    return -2; 
                }


                if (installment.IsPaid)
                {
                    transaction.Rollback();
                    return -3; 
                }


                if (addDto.AmountPaid > installment.Amount)
                {
                    transaction.Rollback();
                    return -4;
                }


                var payment = new DataLayer.Models.Entities.Payment
                {
                    StudentId = addDto.StudentId,
                    InstallmentId = addDto.InstallmentId,
                    AmountPaid = addDto.AmountPaid,
                    PaymentDate = addDto.PaymentDate == default ? DateTime.Now : addDto.PaymentDate,
                    SecretaryId = addDto.SecretaryId
                };

                _context.Payments.Add(payment);

                installment.Amount -= addDto.AmountPaid;

                if (installment.Amount <= 0)
                {
                    installment.Amount = 0; 
                    installment.IsPaid = true;
                }

                _context.Installments.Update(installment);

                bool isSaved = _context.SaveChanges() > 0;

                if (isSaved)
                {
                    transaction.Commit();
                    return payment.PaymentId;
                }
                else
                {
                    transaction.Rollback();
                    return 0;
                }
            }
            catch (Exception)
            {
                transaction.Rollback();
                return 0;
            }
        }

        public bool UpdatePayment(UpdatePaymentDto updateDto)
        {
            if (updateDto == null || updateDto.PaymentId <= 0) return false;

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var payment = _context.Payments.Find(updateDto.PaymentId);
                if (payment == null)
                {
                    transaction.Rollback();
                    return false;
                }

                var installment = _context.Installments.Find(payment.InstallmentId);
                if (installment == null)
                {
                    transaction.Rollback();
                    return false;
                }

                if (updateDto.AmountPaid.HasValue && updateDto.AmountPaid.Value > 0)
                {
                    decimal oldAmount = payment.AmountPaid;
                    decimal newAmount = updateDto.AmountPaid.Value;

                    if (oldAmount != newAmount)
                    {

                        decimal difference = newAmount - oldAmount;

                        if (difference > 0 && difference > installment.Amount)
                        {
                            transaction.Rollback();
                            return false; 
                        }

   
                        installment.Amount -= difference;

                        if (installment.Amount <= 0)
                        {
                            installment.Amount = 0;
                            installment.IsPaid = true;
                        }
                        else
                        {
                            installment.IsPaid = false; 
                        }

                        _context.Installments.Update(installment);
                        payment.AmountPaid = newAmount;
                    }
                }


                if (updateDto.PaymentDate.HasValue)
                {
                    payment.PaymentDate = updateDto.PaymentDate.Value;
                }

                if (updateDto.SecretaryId.HasValue && updateDto.SecretaryId.Value > 0)
                {
                    payment.SecretaryId = updateDto.SecretaryId.Value;
                }

                _context.Payments.Update(payment);

                bool isSaved = _context.SaveChanges() > 0;

                if (isSaved)
                {
                    transaction.Commit();
                    return true;
                }

                transaction.Rollback();
                return false;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }

        public bool DeletePayment(int paymentId)
        {
            if (paymentId <= 0) return false;

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var payment = _context.Payments.Find(paymentId);
                if (payment == null)
                {
                    transaction.Rollback();
                    return false;
                }

                var installment = _context.Installments.Find(payment.InstallmentId);
                if (installment != null)
                {

                    installment.Amount += payment.AmountPaid;

                    if (installment.Amount > 0)
                    {
                        installment.IsPaid = false;
                    }

                    _context.Installments.Update(installment);
                }

                _context.Payments.Remove(payment);
                bool isSaved = _context.SaveChanges() > 0;

                if (isSaved)
                {
                    transaction.Commit();
                    return true;
                }
                else
                {
                    transaction.Rollback();
                    return false;
                }
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}