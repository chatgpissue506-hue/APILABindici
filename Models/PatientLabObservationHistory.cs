namespace LabTestApi.Models
{
    public class PatientLabObservationHistory
    {
        public int LabTestOBRID { get; set; }
        public string? SnomedCode { get; set; }
        public string? MessageSubject { get; set; }
        public string? PanelType { get; set; }
        public DateTime? ObservationDateTime { get; set; }
        public DateTime? StatusChangeDateTime { get; set; }
        public int? AppointmentID { get; set; }
        public int LabTestOBXID { get; set; }
        public string? SnomedCode_2 { get; set; }
        public string? ResultName { get; set; }
        public string? ObservationCodingSystem { get; set; }
        public string? ObservationValue { get; set; }
        public string? Units { get; set; }
        public string? ReferenceRanges { get; set; }
        public int AbnormalFlagID { get; set; }
        public string? AbnormalFlagDesc { get; set; }
        public int? LabTestNTEID { get; set; }
        public string? Source { get; set; }
        public string? Comments { get; set; }
        public int PriorityID { get; set; }
        public string? ProviderFullName { get; set; }
        public string? PatientFullAddress { get; set; }
    }
} 