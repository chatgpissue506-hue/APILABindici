namespace LabTestApi.Models
{
    public class PatientLabObservation
    {
        public int PatientID { get; set; }
        public string? MessageSubject { get; set; }
        public string? ResultName { get; set; }
        public string? ObservationCodingSystem { get; set; }
        public DateTime? ObservationDateTime { get; set; }
        public string? ObservationValue { get; set; }
        public string? Units { get; set; }
        public string? ReferenceRanges { get; set; }
        public int? AbnormalFlagID { get; set; }
        public string? AbnormalFlagDesc { get; set; }
        public long? LabTestNTEID { get; set; }
        public string? Source { get; set; }
        public string? Comments { get; set; }
    }
} 