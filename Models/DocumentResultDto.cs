using System;

namespace LabTestApi.Models
{
    public class DocumentResultDto
    {
        public int DocumentID { get; set; }
        public int DocumentTypeID { get; set; }
        public string? DocumentName { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
        public string? DocumentType { get; set; }
        public string? DocumentBase64 { get; set; }
        public string? DocumentText { get; set; }
    }
}
