namespace LabTestApi.Models
{
    public class MedicationData
    {
        public string Name { get; set; } = string.Empty;
        public string ConceptId { get; set; } = string.Empty;
        public string Vocabulary { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    public class MedicationSearchResponse
    {
        public List<MedicationData> Results { get; set; } = new List<MedicationData>();
        public int TotalCount { get; set; }
        public string Query { get; set; } = string.Empty;
    }
} 