using System;

namespace LabTestApi.Models
{
    public class DocumentData
    {
        public int DocumentID { get; set; }
        public int DocumentTypeID { get; set; }
        public string? DocumentName { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
        public string? DocumentType { get; set; }
        public byte[]? DocumentBytes { get; set; }
        public int? InboxFolderItemID { get; set; }
    }

    public class DocumentRequest
    {
        public string DocumentKey { get; set; } = string.Empty;
        public int PracticeID { get; set; }
    }
}
