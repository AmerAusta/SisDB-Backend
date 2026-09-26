using ApiLayer.Authorization;
using BusinessLayer.Payment;
using BusinessLayer.Payment.PaymentDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiLayer.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly BusinessLayer.Payment.Payment _paymentService;

        public PaymentsController(BusinessLayer.Payment.Payment paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("WithDetails", Name = "GetAllPaymentsWithDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetPaymentWithDetailsDto>> GetAllPaymentsWithDetails()
        {
            var list = _paymentService.GetAllPaymentsWithDetails();

            if (list.IsNullOrEmpty())
                return NotFound(new { message = "No Payments Found" });

            return Ok(list);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("{paymentId}/WithDetails", Name = "GetPaymentByIdWithDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetPaymentWithDetailsDto> GetPaymentByIdWithDetails(int paymentId)
        {
            if (paymentId <= 0)
                return BadRequest(new { message = "Invalid ID" });

            var item = _paymentService.GetPaymentByIdWithDetails(paymentId);

            if (item == null)
                return NotFound(new { message = "Payment Not Found" });

            return Ok(item);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Student,Parent")]
        [HttpGet("Student/{studentId}/WithDetails", Name = "GetPaymentsByStudentIdWithDetails")]
        [StudentAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetPaymentWithDetailsDto>> GetPaymentsByStudentIdWithDetails(int studentId)
        {
            if (studentId <= 0)
                return BadRequest(new { message = "Invalid Student ID" });

            var list = _paymentService.GetPaymentsByStudentIdWithDetails(studentId);

            if (list.IsNullOrEmpty())
                return NotFound(new { message = "No Payments Found for this Student" });

            return Ok(list);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost(Name = "AddNewPayment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddNewPayment([FromBody] AddPaymentDto addDto)
        {
            if (addDto == null)
                return BadRequest(new { message = "Payment data is required" });

            int result = _paymentService.AddNewPayment(addDto);

            return result switch
            {
                0 => StatusCode(500, new { message = "An error occurred while saving the payment." }),
                -1 => NotFound(new { message = "Student not found." }),
                -2 => NotFound(new { message = "Installment not found." }),
                -3 => BadRequest(new { message = "Installment is already fully paid." }),
                -4 => BadRequest(new { message = "Payment amount exceeds the remaining installment amount." }),
                _ => CreatedAtRoute("GetPaymentByIdWithDetails", new { paymentId = result }, new { paymentId = result, message = "Payment Added Successfully" })
            };
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut(Name = "UpdatePayment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePayment([FromBody] UpdatePaymentDto updateDto)
        {
            if (updateDto == null)
                return BadRequest(new { message = "Update data is required" });

            bool updated = _paymentService.UpdatePayment(updateDto);

            if (!updated)
                return BadRequest(new { message = "Payment Update Failed. Check data, amount limits, or ID." });

            return Ok(new { message = "Payment Updated Successfully" });
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("{paymentId}", Name = "DeletePayment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeletePayment(int paymentId)
        {
            if (paymentId <= 0)
                return BadRequest(new { message = "Invalid ID" });

            bool deleted = _paymentService.DeletePayment(paymentId);

            if (deleted)
                return Ok(new { message = "Payment Deleted Successfully and Installment Balance Restored" });

            return NotFound(new { message = "Payment Delete Failed. ID not found." });
        }
    }
}