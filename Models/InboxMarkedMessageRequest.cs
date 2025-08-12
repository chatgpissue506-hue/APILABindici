using System;

namespace LabTestApi.Models
{
    public class InboxMarkedMessageRequest
    {
        public long? InboxFolderItemID { get; set; }
        public int UpdatedBy { get; set; }
        public int? PracticeID { get; set; }
        public int PatientID { get; set; } = 0;
        public int UserLoggingID { get; set; }
        public int? PracticeLocationID { get; set; }
        public string? GlobalAccessIdentifier { get; set; }
        public bool IsNormalFile { get; set; } = false;
    }

    public class InboxMarkedMessageResponse
    {
        public int OutputParam { get; set; }
        public int OutputParamCount { get; set; }
        public bool IsShowOnPortal { get; set; }
        public bool IsConfidential { get; set; }
    }
}
