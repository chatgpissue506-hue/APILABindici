namespace LabTestApi.Models
{
    public class PatientDiagnosis
    {
        public int DiagnosisID { get; set; }
        public int? AppointmentID { get; set; }
        public string? DiseaseName { get; set; }
        public DateTime? DiagnosisDate { get; set; }
        public string? DiagnosisBy { get; set; }
        public string? Summary { get; set; }
        public bool IsLongTerm { get; set; }
        public bool AddtoProblem { get; set; }
        public bool IsHighlighted { get; set; }
        public int? SequenceNo { get; set; }
        public bool IsActive { get; set; }
        public bool IsConfidential { get; set; }
        public string? DiagnosisType { get; set; }
        public bool IsMapped { get; set; }
        public int? PracticeID { get; set; }
        public DateTime? OnSetDate { get; set; }
        public string? MappedBy { get; set; }
        public DateTime? MappedDate { get; set; }
        public bool IsStopped { get; set; }
        public string? SnomedDiseaseName { get; set; }
        public int? PatientID { get; set; }
        public int? PracticeLocationID { get; set; }
        public bool IsPrimaryDiagnosis { get; set; }
        public string? DiagnoseStatusName { get; set; }
    }
} 