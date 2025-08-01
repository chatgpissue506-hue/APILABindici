namespace LabTestApi.Models
{
    public class PatientLabTestHeader
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
        public DateTime InboxUpdatedAt { get; set; }
        public DateTime InboxReceivedDate { get; set; }
    }
} 