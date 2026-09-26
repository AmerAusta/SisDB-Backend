using ApiLayer.Authorization;
using BusinessLayer.Installment;
using BusinessLayer.Installment.InstallmentDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiLayer.Controllers.Installment
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstallmentsController : ControllerBase
    {
        private readonly BusinessLayer.Installment.Installment _installmentService;

        public InstallmentsController(BusinessLayer.Installment.Installment installmentService)
        {
            _installmentService = installmentService;
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("WithDetails", Name = "GetAllInstallmentsWithDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetInstallmentWithStudentDetailsDto>> GetAllInstallmentsWithDetails()
        {
            var list = _installmentService.GetAllInstallmentsWithDetails();

            if (list.IsNullOrEmpty())
                return NotFound(new { message = "No Installments Found" });

            return Ok(list);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("{installmentId}/WithDetails", Name = "GetInstallmentByIdWithDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetInstallmentWithStudentDetailsDto> GetInstallmentByIdWithDetails(int installmentId)
        {
            if (installmentId <= 0)
                return BadRequest(new { message = "Invalid ID" });

            var item = _installmentService.GetInstallmentByIdWithDetails(installmentId);

            if (item == null)
                return NotFound(new { message = "Installment Not Found" });

            return Ok(item);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Student,Parent")]
        [HttpGet("Student/{studentId}/WithDetails", Name = "GetInstallmentByStudentIdWithDetails")]
        [StudentAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetInstallmentWithStudentDetailsDto> GetInstallmentsByStudentIdWithDetails(int studentId)
        {
            if (studentId <= 0)
                return BadRequest(new { message = "Invalid Student ID" });

            var installment = _installmentService.GetInstallmentByStudentIdWithDetails(studentId);

            if (installment == null)
                return NotFound(new { message = "No Installment Found for this Student" });

            return Ok(installment);
        }

    }
}