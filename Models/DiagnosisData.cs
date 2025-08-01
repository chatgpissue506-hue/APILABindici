namespace LabTestApi.Models
{
    public class DiagnosisData
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class DiagnosisSearchResponse
    {
        public List<DiagnosisData> Results { get; set; } = new List<DiagnosisData>();
        public int TotalCount { get; set; }
        public string Query { get; set; } = string.Empty;
    }
} 