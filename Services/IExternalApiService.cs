using LabTestApi.Models;

namespace LabTestApi.Services
{
    public interface IExternalApiService
    {
        Task<DiagnosisSearchResponse> SearchDiagnosisAsync(string query);
        Task<MedicationSearchResponse> SearchMedicationAsync(string search);
    }
} 