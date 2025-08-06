using LabTestApi.Models;

namespace LabTestApi.Services
{
    public interface ILabTestService
    {
        Task<IEnumerable<LabTestData>> GetLabTestDataAsync();
        Task<IEnumerable<LabTestData>> GetLabTestDataByPatientAsync(string patientId);
        Task<IEnumerable<LabTestData>> GetLabTestDataByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<LabTestData>> GetLabTestDataWithFiltersAsync(string? patientId = null, DateTime? startDate = null, DateTime? endDate = null, string? practiceId = null);
        Task<IEnumerable<LabTestData>> GetPatientLabTestDataAsync(long patientId);
        Task<PatientInfo?> GetPatientInfoByIDAsync(long patientId);
        Task<PatientLabTestResponse?> GetPatientLabTestDataUpdatedAsync(long patientId, long? labTestMshID = null);
        Task<List<PatientAllergy>> GetPatientAllergiesAsync(long patientId);
        Task<List<PatientDiagnosis>> GetPatientDiagnosesAsync(long patientId);
        Task<List<PatientLabObservation>> GetPatientLabObservationsAsync(int patientId, string? observationText = null, int? practiceId = null);
        Task<List<PatientLabObservationHistory>> GetPatientLabObservationHistoryByNameAsync(int patientId, DateTime? startDate = null, DateTime? endDate = null, string? panelTypeFilter = null);
        Task<List<PatientMedication>> GetPatientMedicationDetailsAsync(int patientId, int practiceId = 127, int practiceLocationId = 4, int pageNo = 1, int pageSize = 20);
    }
}