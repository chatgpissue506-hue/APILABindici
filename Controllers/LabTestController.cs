using Microsoft.AspNetCore.Mvc;
using LabTestApi.Services;
using LabTestApi.Models;

namespace LabTestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LabTestController : ControllerBase
    {
        private readonly ILabTestService _labTestService;

        public LabTestController(ILabTestService labTestService)
        {
            _labTestService = labTestService;
        }

        /// <summary>
        /// Get all lab test data from the stored procedure
        /// </summary>
        /// <returns>List of lab test data</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LabTestData>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<LabTestData>>> GetLabTestData()
        {
            try
            {
                var labTestData = await _labTestService.GetLabTestDataAsync();
                return Ok(labTestData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving lab test data", details = ex.Message });
            }
        }

        /// <summary>
        /// Get lab test data for a specific patient
        /// </summary>
        /// <param name="patientId">The patient ID to filter by</param>
        /// <returns>List of lab test data for the patient</returns>
        [HttpGet("GetPatientIndividualResults/{patientId}")]
        [ProducesResponseType(typeof(IEnumerable<LabTestData>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<LabTestData>>> GetLabTestDataByPatient(string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                return BadRequest("Patient ID is required");
            }

            try
            {
                var labTestData = await _labTestService.GetLabTestDataByPatientAsync(patientId);
                return Ok(labTestData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving lab test data", details = ex.Message });
            }
        }

        /// <summary>
        /// Get lab test data within a date range
        /// </summary>
        /// <param name="startDate">Start date for the range</param>
        /// <param name="endDate">End date for the range</param>
        /// <returns>List of lab test data within the date range</returns>
        [HttpGet("daterange")]
        [ProducesResponseType(typeof(IEnumerable<LabTestData>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<LabTestData>>> GetLabTestDataByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest("Start date must be before or equal to end date");
            }

            try
            {
                var labTestData = await _labTestService.GetLabTestDataByDateRangeAsync(startDate, endDate);
                return Ok(labTestData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving lab test data", details = ex.Message });
            }
        }

        /// <summary>
        /// Get lab test data with flexible filters
        /// </summary>
        /// <param name="patientId">Patient ID filter</param>
        /// <param name="startDate">Start date filter</param>
        /// <param name="endDate">End date filter</param>
        /// <param name="practiceId">Practice ID filter</param>
        /// <returns>Filtered lab test data</returns>
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<LabTestData>>> GetLabTestDataWithFilters(
            [FromQuery] string? patientId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? practiceId = null)
        {
            try
            {
                var result = await _labTestService.GetLabTestDataWithFiltersAsync(patientId, startDate, endDate, practiceId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get lab test data for a specific patient using GetPatientLabTestData stored procedure
        /// </summary>
        /// <param name="patientId">Patient ID (bigint)</param>
        /// <param name="labTestMshID">Optional Lab Test MSH ID filter</param>
        /// <returns>Lab test data for the specified patient</returns>
        [HttpGet("patientinboxdetail/{patientId:long}")]
        public async Task<ActionResult<PatientLabTestResponse>> GetPatientLabTestData(
            long patientId,
            [FromQuery] long? labTestMshID = null)
        {
            try
            {
                var result = await _labTestService.GetPatientLabTestDataUpdatedAsync(patientId, labTestMshID);
                if (result == null)
                {
                    return NotFound($"Patient lab test data for patient ID {patientId} not found");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get patient information using GetPatientnameforLAB stored procedure
        /// </summary>
        /// <param name="patientId">Patient ID (bigint)</param>
        /// <returns>Patient information including ethnicity</returns>
        [HttpGet("patient-info/{patientId:long}")]
        public async Task<ActionResult<PatientInfo>> GetPatientInfo(long patientId)
        {
            try
            {
                var result = await _labTestService.GetPatientInfoByIDAsync(patientId);
                if (result == null)
                {
                    return NotFound($"Patient with ID {patientId} not found");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get patient lab test data using updated GetPatientLabTestData stored procedure (returns structured data)
        /// </summary>
        /// <param name="patientId">Patient ID (bigint)</param>
        /// <param name="labTestMshID">Optional Lab Test MSH ID filter</param>
        /// <returns>Structured patient lab test data with header and details</returns>
        [HttpGet("patient-labtest-updated/{patientId:long}")]
        public async Task<ActionResult<PatientLabTestResponse>> GetPatientLabTestDataUpdated(
            long patientId, 
            [FromQuery] long? labTestMshID = null)
        {
            try
            {
                var result = await _labTestService.GetPatientLabTestDataUpdatedAsync(patientId, labTestMshID);
                if (result == null)
                {
                    return NotFound($"Patient lab test data for patient ID {patientId} not found");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get patient allergies
        /// </summary>
        /// <param name="patientId">Patient ID (bigint)</param>
        /// <returns>List of patient allergies</returns>
        [HttpGet("patient-allergies/{patientId:long}")]
        public async Task<ActionResult<List<PatientAllergy>>> GetPatientAllergies(long patientId)
        {
            try
            {
                var result = await _labTestService.GetPatientAllergiesAsync(patientId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get patient diagnoses
        /// </summary>
        /// <param name="patientId">Patient ID (bigint)</param>
        /// <returns>List of patient diagnoses</returns>
        [HttpGet("patient-diagnoses/{patientId:long}")]
        public async Task<ActionResult<List<PatientDiagnosis>>> GetPatientDiagnoses(long patientId)
        {
            try
            {
                var result = await _labTestService.GetPatientDiagnosesAsync(patientId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get patient lab observations with optional filters
        /// </summary>
        /// <param name="patientId">Patient ID (int)</param>
        /// <param name="observationText">Optional observation text filter</param>
        /// <param name="practiceId">Optional practice ID filter</param>
        /// <returns>List of patient lab observations</returns>
        [HttpGet("patient-observations/{patientId:int}")]
        public async Task<ActionResult<List<PatientLabObservation>>> GetPatientLabObservations(
            int patientId,
            [FromQuery] string? observationText = null,
            [FromQuery] int? practiceId = null)
        {
            try
            {
                var result = await _labTestService.GetPatientLabObservationsAsync(patientId, observationText, practiceId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get patient lab observation history by name with optional filters
        /// </summary>
        /// <param name="patientId">Patient ID (int)</param>
        /// <param name="startDate">Optional start date filter</param>
        /// <param name="endDate">Optional end date filter</param>
        /// <param name="panelTypeFilter">Optional panel type filter (e.g., 'CBC', 'Hemoglobin')</param>
        /// <returns>List of patient lab observation history</returns>
        [HttpGet("patient-observation-history/{patientId:int}")]
        public async Task<ActionResult<List<PatientLabObservationHistory>>> GetPatientLabObservationHistory(
            int patientId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? panelTypeFilter = null)
        {
            try
            {
                var result = await _labTestService.GetPatientLabObservationHistoryByNameAsync(patientId, startDate, endDate, panelTypeFilter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}