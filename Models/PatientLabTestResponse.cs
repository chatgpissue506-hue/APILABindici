namespace LabTestApi.Models
{
    public class PatientLabTestResponse
    {
        public PatientLabTestHeader? Header { get; set; }
        public List<PatientLabTestDetail> Details { get; set; } = new List<PatientLabTestDetail>();
    }
} 