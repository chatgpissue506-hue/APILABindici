namespace LabTestApi.Models
{
    public class PatientLabTestResponse
    {
        public PatientLabTestHeader? Header { get; set; }
        public List<PatientLabTestDetail> LabTestDetails { get; set; } = new List<PatientLabTestDetail>();
        public List<PatientAllergy> Allergies { get; set; } = new List<PatientAllergy>();
        public List<PatientDiagnosis> Diagnoses { get; set; } = new List<PatientDiagnosis>();
    }
} 