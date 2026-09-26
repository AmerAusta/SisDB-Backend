using ApiLayer.Authorization;
using BusinessLayer.Certificate;
using BusinessLayer.Certificate.CertificateDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace ApiLayer.Controllers.Certificate
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly ILogger<CertificatesController> _logger;
        private readonly BusinessLayer.Certificate.Certificate _certificateService;

        public CertificatesController(
            ILogger<CertificatesController> logger,
            BusinessLayer.Certificate.Certificate certificateService)
        {
            _logger = logger;
            _certificateService = certificateService;
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpGet("", Name = "GetAllCertificates")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetCertificateDto>> GetAllCertificates()
        {
            var certificates = _certificateService.GetAllCertificates();

            if (certificates.IsNullOrEmpty()) return NotFound(new { message = "No Certificates Found" });

            return Ok(certificates);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpGet("{certificateId}", Name = "GetCertificateById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetCertificateDto> GetCertificateById(int certificateId)
        {
            if (certificateId <= 0) return BadRequest(new { message = "Invalid ID" });

            var certificate = _certificateService.GetCertificateById(certificateId);

            if (certificate == null) return NotFound(new { message = "Certificate Not Found" });

            return Ok(certificate);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("Student/{studentId}", Name = "GetCertificatesByStudentId")]
        [StudentAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetCertificateDto>> GetCertificatesByStudentId(int studentId)
        {
            if (studentId <= 0) return BadRequest(new { message = "Invalid Student ID" });

            var certificates = _certificateService.GetCertificatesByStudentId(studentId);

            if (certificates.IsNullOrEmpty()) return NotFound(new { message = "No Certificates Found for this Student" });

            return Ok(certificates);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student,Parent")]
        [HttpGet("Student/{studentId}/Full-Details", Name = "GetCertificatesWithFullDetailsByStudentId")]
        [StudentAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetCertificateWithFullDetailsDto>> GetCertificatesWithFullDetailsByStudentId(int studentId)
        {
            if (studentId <= 0) return BadRequest(new { message = "Invalid Student ID" });

            var certificates = _certificateService.GetCertificatesWithFullDetailsByStudentId(studentId);

            if (certificates.IsNullOrEmpty()) return NotFound(new { message = "No Full Details Certificates Found for this Student" });

            return Ok(certificates);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpGet("Full-Details", Name = "GetAllCertificatesWithFullDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetCertificateWithFullDetailsDto>> GetAllCertificatesWithFullDetails()
        {
            var certificates = _certificateService.GetAllCertificatesWithFullDetails();

            if (certificates.IsNullOrEmpty()) return NotFound(new { message = "No Certificates Found" });

            return Ok(certificates);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpPost("Add", Name = "AddNewCertificate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddNewCertificate([FromBody] AddCertificateDto addDto)
        {
            if (addDto == null || addDto.StudentId <= 0 || addDto.ClassId <= 0 || string.IsNullOrWhiteSpace(addDto.Title))
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            int certificateId = _certificateService.AddNewCertificate(addDto);

            if (certificateId == -1)
                return BadRequest(new { message = "Student ID not found." });

            if (certificateId == -2)
                return BadRequest(new { message = "Class ID not found." });

            if (certificateId == -3)
                return BadRequest(new { message = "This certificate already exists for this student in this class." });

            if (certificateId == -4)
                return BadRequest(new { message = "Grade text is too long. It must not exceed 10 characters." });

            if (certificateId == -5)
                return BadRequest(new { message = "Grade must be a number between 0 and 100." });

            if (certificateId > 0)
            {
                return Ok(new
                {
                    message = "Certificate Added Successfully",
                    certificateId = certificateId
                });
            }

            return BadRequest(new { message = "Failed to Add Certificate." });
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpPut("Update", Name = "UpdateCertificate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateCertificate([FromBody] UpdateCertificateDto updateDto)
        {
            if (updateDto == null || updateDto.CertificateId <= 0)
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Grade))
            {
                if (!int.TryParse(updateDto.Grade, out int gradeValue) || gradeValue < 0 || gradeValue > 100)
                {
                    return BadRequest(new { message = "Grade must be a number between 0 and 100." });
                }
            }

            bool isUpdated = _certificateService.UpdateCertificate(updateDto);

            if (isUpdated)
                return Ok(new { message = "Certificate Updated Successfully" });

            return BadRequest(new { message = "Certificate Update Failed. Certificate, Student, or Class ID not found." });
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpDelete("{certificateId}", Name = "DeleteCertificate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteCertificate(int certificateId)
        {
            if (certificateId <= 0) return BadRequest(new { message = "Invalid ID" });

            bool isDeleted = _certificateService.DeleteCertificate(certificateId);

            if (isDeleted)
                return Ok(new { message = "Certificate Deleted Successfully" });

            return NotFound(new { message = "Certificate Delete Failed. Certificate ID not found." });
        }

    }
}