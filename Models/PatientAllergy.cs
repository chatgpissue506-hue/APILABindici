namespace LabTestApi.Models
{
    public class PatientAllergy
    {
        public int AllergyID { get; set; }
        public string? AllergyUUID { get; set; }
        public bool IsReviewed { get; set; }
        public int? MedTechID { get; set; }
        public DateTime? OnsetDate { get; set; }
        public int? AllergyTypeID { get; set; }
        public int? MedicineTypeID { get; set; }
        public string? MedicineShortName { get; set; }
        public string? MedicineClassification { get; set; }
        public string? FavouriteSubstance { get; set; }
        public string? DiseaseName { get; set; }
        public int? SubstanceTypeId { get; set; }
        public string? Other { get; set; }
        public string? Reaction { get; set; }
        public bool IsActive { get; set; }
        public string? FullName { get; set; }
        public string? Comment { get; set; }
        public bool IsHighlight { get; set; }
        public DateTime? InsertedAt { get; set; }
        public string? AllergyType { get; set; }
        public string? Name { get; set; }
        public bool IsNKA { get; set; }
        public int? SequenceNo { get; set; }
        public string? Severity { get; set; }
    }
} 