namespace LabTestApi.Models
{
    public class PatientMedication
    {
        public int PatientID { get; set; }
        public int MedicationID { get; set; }
        public DateTime? LastRXDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string? ProviderName { get; set; }
        public string? MedicineName { get; set; }
        public string? Take { get; set; }
        public int? FrequencyID { get; set; }
        public int? RouteID { get; set; }
        public int? Quantity { get; set; }
        public int? Duration { get; set; }
        public string? DurationType { get; set; }
        public string? Directions { get; set; }
        public string? MedicationCategory { get; set; }
    }
} 