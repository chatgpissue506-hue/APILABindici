using System.ComponentModel.DataAnnotations;

namespace LabTestApi.Models
{
    public class LabTestData
    {
        public int LabTestMshID { get; set; }
        public string? SendingApplication { get; set; }
        public string? SendingFacility { get; set; }
        public string? ReceivingFacility { get; set; }
        public DateTime MessageDatetime { get; set; }
        public string? NHINumber { get; set; }
        public string? FullName { get; set; }
        public DateTime DOB { get; set; }
        public string? GenderName { get; set; }
        public string? PatientID { get; set; }
        public string? PracticeID { get; set; }
        public DateTime MshInsertedAt { get; set; }
        public bool MarkasRead { get; set; }
        public DateTime IFIInboxUpdate { get; set; }
        public DateTime InboxReceivedDate { get; set; }
        public int LabTestOBRID { get; set; }
        public string? SnomedCode { get; set; }
        public string? PanelType { get; set; }
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
        public string? AbnormalFlagDescription { get; set; }
        public int LabTestNTEID { get; set; }
        public string? Source { get; set; }
        public string? Comments { get; set; }
        public string? Ethnicity { get; set; }
        public int PriorityID { get; set; }
        public string? ProviderFullName { get; set; }
        public string? OrgName { get; set; }
        public string? FolderName { get; set; }
        public DateTime? PrevDate { get; set; }
        public string? OBResultStatus { get; set; }
        public string? ResultCategory { get; set; }
    }
}