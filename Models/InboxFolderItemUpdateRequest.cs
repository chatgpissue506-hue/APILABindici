using System;

namespace LabTestApi.Models
{
    public class InboxFolderItemUpdateRequest
    {
        public long InboxFolderItemID { get; set; }
        public string? Comments { get; set; }
        public bool? MarkedAsRead { get; set; }
        public long UserLoggingID { get; set; }
    }

    public class InboxFolderItemUpdateResponse
    {
        public long OutputInboxFolderItemID { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
