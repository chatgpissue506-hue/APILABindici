namespace LabTestApi.Models
{
    public class PatientLabTestHeader
    {
        public string? NHINumber { get; set; }
        public string? FullName { get; set; }
        public DateTime DOB { get; set; }
        public int? Age { get; set; }
        public string? GenderName { get; set; }
        public string? PatientID { get; set; }
        public string? PracticeID { get; set; }
        public DateTime MshInsertedAt { get; set; }
        public string? Ethnicity { get; set; }
        public int? AgeFromProfile { get; set; }
        public string? PatientFullAddress { get; set; }
    }
} 