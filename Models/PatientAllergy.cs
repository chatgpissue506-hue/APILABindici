namespace LabTestApi.Models
{
    public class PatientAllergy
    {
        public int AllergyID { get; set; }
        public int? AppointmentID { get; set; }
        public string? AllergyType { get; set; }
        public DateTime? OnsetDate { get; set; }
        public string? DeactivationReason { get; set; }
        public byte? ReactionID { get; set; }
        public byte? ReactionTypeID { get; set; }
        public byte? SeverityID { get; set; }
        public string? Reaction { get; set; }
        public bool IsActive { get; set; }
        public int? PatientID { get; set; }
        public bool IsConfidential { get; set; }
        public int? PracticeID { get; set; }
        public string? CategoryName { get; set; }
        public string? Comment { get; set; }
        public byte? SubstanceTypeID { get; set; }
        public string? OtherSubstance { get; set; }
        public byte? MedicineTypeID { get; set; }
        public bool ShowOnPortal { get; set; }
        public bool IsReviewed { get; set; }
        public bool IsHighlight { get; set; }
        public string? MedicineName { get; set; }
    }
} 