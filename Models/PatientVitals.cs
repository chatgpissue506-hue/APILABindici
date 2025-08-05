namespace LabTestApi.Models
{
    public class PatientVitals
    {
        public DateTime? BPInsertedAt { get; set; }
        public DateTime? HeightInsertedAt { get; set; }
        public DateTime? WeightInsertedAt { get; set; }
        public DateTime? BMIInsertedAt { get; set; }
        public string? Height { get; set; }
        public string? BMI { get; set; }
        public string? Weight { get; set; }
        public string? BPSys { get; set; }
        public string? BPDia { get; set; }
        public DateTime? CVRAInsertedAt { get; set; }
        public string? CVRA { get; set; }
        public string? NZPP { get; set; }
        public string? WaistCircumference { get; set; }
        public DateTime? WaistCircumferenceInsertedAt { get; set; }
    }
} 