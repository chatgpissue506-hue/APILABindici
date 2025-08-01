using Microsoft.AspNetCore.Mvc;
using LabTestApi.Services;
using LabTestApi.Models;

namespace LabTestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalApiController : ControllerBase
    {
        private readonly IExternalApiService _externalApiService;
        private readonly ILogger<ExternalApiController> _logger;

        public ExternalApiController(IExternalApiService externalApiService, ILogger<ExternalApiController> logger)
        {
            _externalApiService = externalApiService;
            _logger = logger;
        }

        /// <summary>
        /// Search for ICD-10 diagnosis codes using NIH Clinical Tables API
        /// </summary>
        /// <param name="query">Search term for diagnosis</param>
        /// <returns>List of matching diagnosis codes and names</returns>
        [HttpGet("diagnosis/search")]
        public async Task<ActionResult<DiagnosisSearchResponse>> SearchDiagnosis([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest("Query parameter is required");
                }

                _logger.LogInformation($"🔍 Searching diagnosis for query: {query}");
                var result = await _externalApiService.SearchDiagnosisAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error in diagnosis search: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Search for medications using RxNav API
        /// </summary>
        /// <param name="search">Search term for medication</param>
        /// <returns>List of matching medications</returns>
        [HttpGet("medication/search")]
        public async Task<ActionResult<MedicationSearchResponse>> SearchMedication([FromQuery] string search)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(search))
                {
                    return BadRequest("Search parameter is required");
                }

                _logger.LogInformation($"🔍 Searching medication for query: {search}");
                var result = await _externalApiService.SearchMedicationAsync(search);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error in medication search: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get information about available external APIs
        /// </summary>
        /// <returns>Information about external API endpoints</returns>
        [HttpGet("info")]
        public ActionResult<object> GetApiInfo()
        {
            return Ok(new
            {
                message = "External API Integration",
                version = "1.0.0",
                apis = new[]
                {
                    new
                    {
                        name = "ICD-10 Diagnosis Search",
                        endpoint = "GET /api/externalapi/diagnosis/search?query={term}",
                        description = "Search for ICD-10 diagnosis codes using NIH Clinical Tables API",
                        source = "https://clinicaltables.nlm.nih.gov/api/icd10cm/v3/search"
                    },
                    new
                    {
                        name = "Medication Search",
                        endpoint = "GET /api/externalapi/medication/search?search={term}",
                        description = "Search for medications using RxNav API",
                        source = "https://rxnav.nlm.nih.gov/REST/drugs.json"
                    }
                },
                documentation = "Visit http://localhost:5050/swagger for interactive API documentation"
            });
        }
    }
} 