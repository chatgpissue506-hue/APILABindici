using System.Text.Json;
using LabTestApi.Models;

namespace LabTestApi.Services
{
    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExternalApiService> _logger;

        public ExternalApiService(HttpClient httpClient, ILogger<ExternalApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<DiagnosisSearchResponse> SearchDiagnosisAsync(string query)
        {
            try
            {
                _logger.LogInformation($"🔍 Searching for diagnosis with query: {query}");
                
                var url = $"https://clinicaltables.nlm.nih.gov/api/icd10cm/v3/search?sf=code,name&terms={Uri.EscapeDataString(query)}";
                
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"📋 Raw response from NIH API: {content}");
                
                // Parse the JSON array response
                var jsonArray = JsonSerializer.Deserialize<JsonElement[]>(content);
                
                if (jsonArray == null || jsonArray.Length < 3)
                {
                    _logger.LogWarning("❌ Invalid response format from NIH API");
                    return new DiagnosisSearchResponse { Query = query };
                }
                
                var results = new List<DiagnosisData>();
                var codes = jsonArray[1].EnumerateArray().ToArray();
                var names = jsonArray[3].EnumerateArray().ToArray();
                
                for (int i = 0; i < Math.Min(codes.Length, names.Length); i++)
                {
                    results.Add(new DiagnosisData
                    {
                        Code = codes[i].GetString() ?? string.Empty,
                        Name = names[i].GetString() ?? string.Empty,
                        Description = $"{codes[i].GetString()} - {names[i].GetString()}"
                    });
                }
                
                _logger.LogInformation($"✅ Found {results.Count} diagnosis results");
                return new DiagnosisSearchResponse
                {
                    Results = results,
                    TotalCount = results.Count,
                    Query = query
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error searching diagnosis: {ex.Message}");
                return new DiagnosisSearchResponse 
                { 
                    Query = query,
                    Results = new List<DiagnosisData>()
                };
            }
        }

        public async Task<MedicationSearchResponse> SearchMedicationAsync(string search)
        {
            try
            {
                _logger.LogInformation($"🔍 Searching for medication with query: {search}");
                
                var url = $"https://rxnav.nlm.nih.gov/REST/drugs.json?name={Uri.EscapeDataString(search)}";
                
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"📋 Raw response from RxNav API: {content}");
                
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(content);
                
                if (!jsonResponse.TryGetProperty("drugGroup", out var drugGroup))
                {
                    _logger.LogWarning("❌ No drugGroup found in RxNav response");
                    return new MedicationSearchResponse { Query = search };
                }
                
                var results = new List<MedicationData>();
                
                if (drugGroup.TryGetProperty("conceptGroup", out var conceptGroup))
                {
                    foreach (var group in conceptGroup.EnumerateArray())
                    {
                        if (group.TryGetProperty("concept", out var concepts))
                        {
                            foreach (var concept in concepts.EnumerateArray())
                            {
                                var medication = new MedicationData();
                                
                                if (concept.TryGetProperty("name", out var name))
                                    medication.Name = name.GetString() ?? string.Empty;
                                
                                if (concept.TryGetProperty("conceptId", out var conceptId))
                                    medication.ConceptId = conceptId.GetString() ?? string.Empty;
                                
                                if (concept.TryGetProperty("vocabulary", out var vocabulary))
                                    medication.Vocabulary = vocabulary.GetString() ?? string.Empty;
                                
                                if (group.TryGetProperty("name", out var category))
                                    medication.Category = category.GetString() ?? string.Empty;
                                
                                results.Add(medication);
                            }
                        }
                    }
                }
                
                _logger.LogInformation($"✅ Found {results.Count} medication results");
                return new MedicationSearchResponse
                {
                    Results = results,
                    TotalCount = results.Count,
                    Query = search
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error searching medication: {ex.Message}");
                return new MedicationSearchResponse 
                { 
                    Query = search,
                    Results = new List<MedicationData>()
                };
            }
        }
    }
} 