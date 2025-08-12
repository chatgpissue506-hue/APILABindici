using System;

namespace LabTestApi.Models
{
    public class ReferralTestData
    {
        public int LabTestMshID { get; set; }
        public string? SendingApplication { get; set; }
        public string? SendingFacility { get; set; }
        public string? ReceivingFacility { get; set; }
        public DateTime MessageDatetime { get; set; }
        public string? NHINumber { get; set; }
        public string? VersionId { get; set; }
        public string? FullName { get; set; }
        public string? DMSID { get; set; }
        public string? DMSIDKey { get; set; }
        public DateTime DOB { get; set; }
        public string? GenderName { get; set; }
        public string? PatientID { get; set; }
        public string? PracticeID { get; set; }
        public DateTime MshInsertedAt { get; set; }
        public bool MarkasRead { get; set; }
        public DateTime IFIInboxUpdate { get; set; }
        public DateTime InboxReceivedDate { get; set; }
        public string? OrgName { get; set; }
        public string? FolderName { get; set; }
    }
}
