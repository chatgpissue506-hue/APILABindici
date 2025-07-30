using System.Data.SqlClient;
using LabTestApi.Models;

namespace LabTestApi.Services
{
    public class LabTestService : ILabTestService
    {
        private readonly string _connectionString;

        public LabTestService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<LabTestData>> GetLabTestDataAsync()
        {
            // Try to connect to database first
            var dbData = await TryGetDataFromDatabase();
            if (dbData.Any())
            {
                return dbData;
            }

            // Fallback to mock data if database connection fails
            return GetMockData();
        }

        private async Task<IEnumerable<LabTestData>> TryGetDataFromDatabase()
        {
            try
            {
                Console.WriteLine("🔍 Testing database connection...");
                Console.WriteLine($"📋 Connection String: {_connectionString}");
                
                var labTestData = new List<LabTestData>();
                
                using (var connection = new SqlConnection(_connectionString))
                {
                    Console.WriteLine("⏳ Opening connection...");
                    await connection.OpenAsync();
                    Console.WriteLine("✅ Successfully connected to database!");
                    
                    // Test basic connectivity
                    using (var testCommand = new SqlCommand("SELECT @@VERSION", connection))
                    {
                        var version = await testCommand.ExecuteScalarAsync();
                        Console.WriteLine($"📊 SQL Server Version: {version}");
                    }
                    
                    // Test if stored procedure exists
                    using (var testCommand = new SqlCommand("SELECT COUNT(*) FROM sys.procedures WHERE name = 'GetLabTestDataWithJoins'", connection))
                    {
                        var procedureExists = (int)await testCommand.ExecuteScalarAsync();
                        if (procedureExists > 0)
                        {
                            Console.WriteLine("✅ Stored procedure 'GetLabTestDataWithJoins' found!");
                        }
                        else
                        {
                            Console.WriteLine("❌ Stored procedure 'GetLabTestDataWithJoins' not found!");
                            return new List<LabTestData>();
                        }
                    }
                    
                    Console.WriteLine("⏳ Executing stored procedure...");
                    using var command = new SqlCommand("EXEC [dbo].[GetLabTestDataWithJoins]", connection);
                    using var reader = await command.ExecuteReaderAsync();
                    
                    Console.WriteLine("📋 Reading data from stored procedure...");
                    Console.WriteLine($"📊 Number of columns: {reader.FieldCount}");
                    
                    // Print column information
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.WriteLine($"  Column {i}: {reader.GetName(i)} ({reader.GetDataTypeName(i)})");
                    }
                    
                    while (await reader.ReadAsync())
                    {
                        try
                        {
                            // Read data safely by checking types
                            var labTestDataItem = new LabTestData();
                            
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (reader.IsDBNull(i)) continue;
                                
                                var columnName = reader.GetName(i);
                                var value = reader[i];
                                
                                // Map columns based on name rather than position
                                switch (columnName.ToLower())
                                {
                                    case "labtestmshid":
                                        labTestDataItem.LabTestMshID = Convert.ToInt32(value);
                                        break;
                                    case "sendingapplication":
                                        labTestDataItem.SendingApplication = value.ToString();
                                        break;
                                    case "sendingfacility":
                                        labTestDataItem.SendingFacility = value.ToString();
                                        break;
                                    case "receivingfacility":
                                        labTestDataItem.ReceivingFacility = value.ToString();
                                        break;
                                    case "messagedatetime":
                                        labTestDataItem.MessageDatetime = Convert.ToDateTime(value);
                                        break;
                                    case "nhinumber":
                                        labTestDataItem.NHINumber = value.ToString();
                                        break;
                                    case "fullname":
                                        labTestDataItem.FullName = value.ToString();
                                        break;
                                    case "dob":
                                        labTestDataItem.DOB = Convert.ToDateTime(value);
                                        break;
                                    case "gendername":
                                        labTestDataItem.GenderName = value.ToString();
                                        break;
                                    case "patientid":
                                        labTestDataItem.PatientID = value.ToString();
                                        break;
                                    case "practiceid":
                                        labTestDataItem.PracticeID = value.ToString();
                                        break;
                                    case "mshinsertedat":
                                        labTestDataItem.MshInsertedAt = Convert.ToDateTime(value);
                                        break;
                                    case "markasread":
                                        labTestDataItem.MarkasRead = Convert.ToBoolean(value);
                                        break;
                                    case "ifiinboxupdate":
                                        labTestDataItem.ifiinboxupdate = Convert.ToDateTime(value);
                                        break;
                                    case "inboxrecevieddate":
                                        labTestDataItem.inboxrecevieddate = Convert.ToDateTime(value);
                                        break;
                                    case "labtestobrid":
                                        labTestDataItem.LabTestOBRID = Convert.ToInt32(value);
                                        break;
                                    case "snomedcode":
                                        labTestDataItem.SnomedCode = value.ToString();
                                        break;
                                    case "mesagesubject":
                                        labTestDataItem.MesageSubject = value.ToString();
                                        break;
                                    case "observationdatetime":
                                        labTestDataItem.ObservationDateTime = Convert.ToDateTime(value);
                                        break;
                                    case "statuschangedatetime":
                                        labTestDataItem.StatusChangeDateTime = Convert.ToDateTime(value);
                                        break;
                                    case "appointmentid":
                                        labTestDataItem.AppointmentID = value.ToString();
                                        break;
                                    case "labtestobxid":
                                        labTestDataItem.LabTestOBXID = Convert.ToInt32(value);
                                        break;
                                    case "snomedcode_2":
                                        labTestDataItem.SnomedCode_2 = value.ToString();
                                        break;
                                    case "resultname":
                                        labTestDataItem.ResultName = value.ToString();
                                        break;
                                    case "observationcodingsystem":
                                        labTestDataItem.ObservationCodingSystem = value.ToString();
                                        break;
                                    case "observationvalue":
                                        labTestDataItem.ObservationValue = value.ToString();
                                        break;
                                    case "units":
                                        labTestDataItem.Units = value.ToString();
                                        break;
                                    case "referenceranges":
                                        labTestDataItem.ReferenceRanges = value.ToString();
                                        break;
                                    case "abnormalflagid":
                                        labTestDataItem.AbnormalFlagID = Convert.ToInt32(value);
                                        break;
                                    case "labtestnteid":
                                        labTestDataItem.LabTestNTEID = Convert.ToInt32(value);
                                        break;
                                    case "source":
                                        labTestDataItem.Source = value.ToString();
                                        break;
                                    case "comments":
                                        labTestDataItem.Comments = value.ToString();
                                        break;
                                }
                            }
                            
                            labTestData.Add(labTestDataItem);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Error reading row data: {ex.Message}");
                            break;
                        }
                    }
                }
                
                Console.WriteLine($"✅ Retrieved {labTestData.Count} records from database");
                return labTestData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database connection failed: {ex.Message}");
                Console.WriteLine($"📋 Connection string: {_connectionString}");
                Console.WriteLine();
                Console.WriteLine("🔧 Troubleshooting tips:");
                Console.WriteLine("1. Check if 'dbserver-local' is accessible from this machine");
                Console.WriteLine("2. Verify the username 'pms_nz' and password are correct");
                Console.WriteLine("3. Ensure the database 'PMS_NZ_Local_NZTFS' exists");
                Console.WriteLine("4. Check if SQL Server is running and accepting connections");
                Console.WriteLine("5. Verify network connectivity to the server");
                return new List<LabTestData>();
            }
        }

        private IEnumerable<LabTestData> GetMockData()
        {
            return new List<LabTestData>
            {
                new LabTestData
                {
                    LabTestMshID = 1,
                    SendingApplication = "LAB_SYSTEM",
                    SendingFacility = "MAIN_LAB",
                    ReceivingFacility = "HOSPITAL_A",
                    MessageDatetime = DateTime.Now.AddDays(-1),
                    NHINumber = "NHI123456789",
                    FullName = "John Doe",
                    DOB = new DateTime(1980, 1, 1),
                    GenderName = "Male",
                    PatientID = "P001",
                    PracticeID = "PRACTICE001",
                    MshInsertedAt = DateTime.Now,
                    MarkasRead = false,
                    LabTestOBRID = 1,
                    SnomedCode = "TEST001",
                    MesageSubject = "Blood Test Results",
                    ObservationDateTime = DateTime.Now,
                    LabTestOBXID = 1,
                    ResultName = "Hemoglobin",
                    ObservationValue = "14.2",
                    Units = "g/dL",
                    ReferenceRanges = "12.0-16.0",
                    AbnormalFlagID = 0,
                    Source = "LAB",
                    Comments = "Normal result"
                },
                new LabTestData
                {
                    LabTestMshID = 2,
                    SendingApplication = "LAB_SYSTEM",
                    SendingFacility = "MAIN_LAB",
                    ReceivingFacility = "HOSPITAL_A",
                    MessageDatetime = DateTime.Now.AddDays(-2),
                    NHINumber = "NHI987654321",
                    FullName = "Jane Smith",
                    DOB = new DateTime(1985, 5, 15),
                    GenderName = "Female",
                    PatientID = "P002",
                    PracticeID = "PRACTICE001",
                    MshInsertedAt = DateTime.Now.AddDays(-1),
                    MarkasRead = true,
                    LabTestOBRID = 2,
                    SnomedCode = "TEST002",
                    MesageSubject = "Cholesterol Test",
                    ObservationDateTime = DateTime.Now.AddDays(-1),
                    LabTestOBXID = 2,
                    ResultName = "Total Cholesterol",
                    ObservationValue = "180",
                    Units = "mg/dL",
                    ReferenceRanges = "<200",
                    AbnormalFlagID = 0,
                    Source = "LAB",
                    Comments = "Good cholesterol level"
                }
            };
        }

        public async Task<IEnumerable<LabTestData>> GetLabTestDataByPatientAsync(string patientId)
        {
            var allData = await GetLabTestDataAsync();
            return allData.Where(x => x.PatientID == patientId);
        }

        public async Task<IEnumerable<LabTestData>> GetLabTestDataByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var allData = await GetLabTestDataAsync();
            return allData.Where(x => x.MessageDatetime >= startDate && x.MessageDatetime <= endDate);
        }

        public async Task<IEnumerable<LabTestData>> GetLabTestDataWithFiltersAsync(
            string? patientId = null, 
            DateTime? startDate = null, 
            DateTime? endDate = null, 
            string? practiceId = null)
        {
            var allData = await GetLabTestDataAsync();
            var filteredData = allData.AsEnumerable();

            if (!string.IsNullOrEmpty(patientId))
                filteredData = filteredData.Where(x => x.PatientID == patientId);

            if (startDate.HasValue)
                filteredData = filteredData.Where(x => x.MessageDatetime >= startDate.Value);

            if (endDate.HasValue)
                filteredData = filteredData.Where(x => x.MessageDatetime <= endDate.Value);

            if (!string.IsNullOrEmpty(practiceId))
                filteredData = filteredData.Where(x => x.PracticeID == practiceId);

            return filteredData;
        }
    }
}