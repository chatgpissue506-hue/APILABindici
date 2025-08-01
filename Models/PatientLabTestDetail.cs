namespace LabTestApi.Models
{
    public class PatientLabTestDetail
    {
        public int LabTestOBRID { get; set; }
        public string? SnomedCode { get; set; }
        public string? MessageSubject { get; set; }
        public DateTime ObservationDateTime { get; set; }
        public DateTime StatusChangeDateTime { get; set; }
        public string? AppointmentID { get; set; }
        public long LabTestOBXID { get; set; }
        public string? SnomedCode_2 { get; set; }
        public string? ResultName { get; set; }
        public string? ObservationCodingSystem { get; set; }
        public string? ObservationValue { get; set; }
        public string? Units { get; set; }
        public string? ReferenceRanges { get; set; }
        public int AbnormalFlagID { get; set; }
        public string? AbnormalFlagDesc { get; set; }
        public int LabTestNTEID { get; set; }
        public string? Source { get; set; }
        public string? Comments { get; set; }
    }
} 