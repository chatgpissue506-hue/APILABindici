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

        // Helper method to safely get nullable int from reader
        private int? GetNullableInt32(SqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinal)) return null;
            
            // Check the actual data type and handle accordingly
            var dataType = reader.GetDataTypeName(ordinal).ToLower();
            switch (dataType)
            {
                case "tinyint":
                    return (int)reader.GetByte(ordinal);
                case "smallint":
                    return (int)reader.GetInt16(ordinal);
                case "int":
                    return reader.GetInt32(ordinal);
                case "bigint":
                    return (int)reader.GetInt64(ordinal);
                case "decimal":
                case "numeric":
                    var decimalValue = reader.GetDecimal(ordinal);
                    return (int)decimalValue;
                case "nvarchar":
                case "varchar":
                case "char":
                case "nchar":
                    // Try to get as string and parse
                    var stringValue = reader.GetString(ordinal);
                    if (int.TryParse(stringValue, out int result))
                        return result;
                    return null; // Return null instead of throwing exception for string fields
                default:
                    // Try to get as string and parse
                    var fallbackStringValue = reader.GetString(ordinal);
                    if (int.TryParse(fallbackStringValue, out int fallbackResult))
                        return fallbackResult;
                    return null; // Return null instead of throwing exception
            }
        }

        // Helper method to safely get nullable DateTime from reader
        private DateTime? GetNullableDateTime(SqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
        }

        // Helper method to safely get nullable string from reader
        private string? GetNullableString(SqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        // Helper method to safely get nullable byte from reader
        private byte? GetNullableByte(SqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinal)) return null;
            
            // Check the actual data type and handle accordingly
            var dataType = reader.GetDataTypeName(ordinal).ToLower();
            switch (dataType)
            {
                case "tinyint":
                    return reader.GetByte(ordinal);
                case "smallint":
                    return (byte)reader.GetInt16(ordinal);
                case "int":
                    return (byte)reader.GetInt32(ordinal);
                case "bigint":
                    return (byte)reader.GetInt64(ordinal);
                case "decimal":
                case "numeric":
                    var decimalValue = reader.GetDecimal(ordinal);
                    return (byte)decimalValue;
                case "nvarchar":
                case "varchar":
                case "char":
                case "nchar":
                    // Try to get as string and parse
                    var stringValue = reader.GetString(ordinal);
                    if (byte.TryParse(stringValue, out byte result))
                        return result;
                    return null; // Return null instead of throwing exception for string fields
                default:
                    // Try to get as string and parse
                    var fallbackStringValue = reader.GetString(ordinal);
                    if (byte.TryParse(fallbackStringValue, out byte fallbackResult))
                        return fallbackResult;
                    return null; // Return null instead of throwing exception
            }
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
                    
                    // Try to execute the stored procedure directly
                    Console.WriteLine("⏳ Attempting to execute GetLabTestDataWithJoins stored procedure...");
                    
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
                                    case "priorityid":
                                        labTestDataItem.PriorityID = Convert.ToInt32(value);
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
                Console.WriteLine("3. Ensure the database 'PMS_NZ_V2' exists");
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
                    Comments = "Normal result",
                    PriorityID = 3
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
                    Comments = "Good cholesterol level",
                    PriorityID = 2
                }
            };
        }

        public async Task<IEnumerable<LabTestData>> GetLabTestDataByPatientAsync(string patientId)
        {
            try
            {
                Console.WriteLine($"🔍 Getting lab test data for patient ID: {patientId}");
                Console.WriteLine($"📋 Connection String: {_connectionString}");
                
                var labTestData = new List<LabTestData>();
                
                using (var connection = new SqlConnection(_connectionString))
                {
                    Console.WriteLine("⏳ Opening connection...");
                    await connection.OpenAsync();
                    Console.WriteLine("✅ Successfully connected to database!");
                    
                    // Use a direct query that filters by patient ID
                    var query = @"
                        SELECT 
                            msh.LabTestMshID,
                            msh.SendingApplication,
                            msh.SendingFacility,
                            msh.ReceivingFacility,
                            msh.MessageDatetime,
                            msh.InternalPatientID as NHINumber,
                            CASE 
                                WHEN tp.FullName IS NULL THEN 
                                    CONCAT(msh.PatientFamilyName, ' ', msh.PatientGivenName, ' ', msh.PatientMiddelName)
                                ELSE tp.FullName 
                            END AS FullName,
                            msh.DOB,
                            tg.GenderName,
                            msh.PatientID,
                            msh.PracticeID,
                            msh.InsertedAt AS MshInsertedAt,
                            ifi.MarkasRead,
                            ifi.updatedat as ifiinboxupdate,
                            ifi.Resultdate as inboxrecevieddate,
                            obr.LabTestOBRID,
                            obr.USCode as SnomedCode,
                            obr.USDescription As MesageSubject,
                            obr.ObservationDateTime,
                            obr.StatusChangeDateTime,
                            obr.AppointmentID,
                            obx.LabTestOBXID,
                            obx.ObservationIdentifier as SnomedCode_2,
                            obx.ObservationText as ResultName,
                            obx.ObservationCodingSystem,
                            obx.ObservationValue,
                            obx.Units,
                            obx.ReferenceRanges,
                            obx.AbnormalFlagID,
                            nte.LabTestNTEID,
                            nte.Source,
                            nte.Comments
                        FROM appointment.tbllabtest_msh AS msh
                        LEFT JOIN appointment.tbllabtest_obr AS obr ON msh.LabTestMshID = obr.LabTestMshID
                        LEFT JOIN appointment.tbllabtest_obx AS obx ON obr.LabTestOBRID = obx.LabTestOBRID
                        LEFT JOIN appointment.tbllabtest_nte AS nte ON obx.LabTestOBXID = nte.LabTestOBXID
                        LEFT JOIN prompt.tblinboxfolderitem ifi ON ifi.externalref = msh.messagecontrolid
                        LEFT JOIN [Lookup].[tblGender] tg ON tg.GenderCode = msh.Gender
                        LEFT JOIN profile.tblprofile tp ON tp.Profileid = msh.PatientID
                        WHERE msh.PatientID = @PatientID";
                    
                    using var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PatientID", patientId);
                    using var reader = await command.ExecuteReaderAsync();
                    
                    Console.WriteLine("📋 Reading data from direct query...");
                    Console.WriteLine($"📊 Number of columns: {reader.FieldCount}");
                    
                    while (await reader.ReadAsync())
                    {
                        try
                        {
                            // Read data safely by checking types
                            var labTestDataItem = new LabTestData();
                            
                            // Map columns safely based on actual column count
                            var columnCount = reader.FieldCount;
                            
                            // Only map columns that actually exist in the result set
                            for (int i = 0; i < columnCount; i++)
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
                                        labTestDataItem.LabTestOBXID = Convert.ToInt64(value);
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
                                    case "priorityid":
                                        labTestDataItem.PriorityID = Convert.ToInt32(value);
                                        break;
                                }
                            }
                            
                            // Add all records since the query is already filtered by patient ID
                            labTestData.Add(labTestDataItem);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Error reading row data: {ex.Message}");
                            break;
                        }
                    }
                }
                
                Console.WriteLine($"✅ Retrieved {labTestData.Count} records for patient {patientId}");
                return labTestData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database connection failed: {ex.Message}");
                Console.WriteLine($"📋 Connection string: {_connectionString}");
                return new List<LabTestData>();
            }
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

        public async Task<IEnumerable<LabTestData>> GetPatientLabTestDataAsync(long patientId)
        {
            try
            {
                Console.WriteLine($"🔍 Getting lab test data for patient ID: {patientId}");
                Console.WriteLine($"📋 Connection String: {_connectionString}");
                
                var labTestData = new List<LabTestData>();
                
                using (var connection = new SqlConnection(_connectionString))
                {
                    Console.WriteLine("⏳ Opening connection...");
                    await connection.OpenAsync();
                    Console.WriteLine("✅ Successfully connected to database!");
                    
                    // First, get patient information
                    var patientInfo = await GetPatientInfoAsync(connection, patientId);
                    
                    // Try to execute the stored procedure directly
                    Console.WriteLine("⏳ Attempting to execute GetPatientLabTestData stored procedure...");
                    
                    Console.WriteLine("⏳ Executing GetPatientLabTestData stored procedure...");
                    using var command = new SqlCommand("EXEC GetPatientLabTestData @pPatientID", connection);
                    command.Parameters.AddWithValue("@pPatientID", patientId);
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
                            
                            // Set patient information from the patient info query
                            if (patientInfo != null)
                            {
                                labTestDataItem.NHINumber = patientInfo.NHINumber;
                                labTestDataItem.FullName = patientInfo.FullName;
                                labTestDataItem.DOB = patientInfo.DOB;
                                labTestDataItem.GenderName = patientInfo.GenderName;
                                labTestDataItem.PatientID = patientInfo.PatientID;
                                labTestDataItem.PracticeID = patientInfo.PracticeID;
                                labTestDataItem.Ethnicity = patientInfo.Ethnicity;
                            }
                            
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (reader.IsDBNull(i)) continue;
                                
                                var columnName = reader.GetName(i);
                                var value = reader[i];
                                
                                // Map columns based on name rather than position
                                switch (columnName.ToLower())
                                {
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
                                    case "priorityid":
                                        labTestDataItem.PriorityID = Convert.ToInt32(value);
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
                
                Console.WriteLine($"✅ Retrieved {labTestData.Count} records for patient {patientId}");
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
                Console.WriteLine("3. Ensure the database 'PMS_NZ_V2' exists");
                Console.WriteLine("4. Check if SQL Server is running and accepting connections");
                Console.WriteLine("5. Verify network connectivity to the server");
                return new List<LabTestData>();
            }
        }

        private async Task<LabTestData?> GetPatientInfoAsync(SqlConnection connection, long patientId)
        {
            try
            {
                Console.WriteLine($"🔍 Getting patient information for patient ID: {patientId}");
                
                // Query to get patient information (removed Ethnicity since it doesn't exist in the database)
                var query = @"
                    SELECT DISTINCT
                        msh.InternalPatientID as NHINumber,
                        CASE 
                            WHEN tp.FullName IS NULL THEN 
                                CONCAT(msh.PatientFamilyName, ' ', msh.PatientGivenName, ' ', msh.PatientMiddelName)
                            ELSE tp.FullName 
                        END AS FullName,
                        msh.DOB,
                        tg.GenderName,
                        msh.PatientID,
                        msh.PracticeID
                    FROM appointment.tbllabtest_msh AS msh
                    LEFT JOIN [Lookup].[tblGender] tg ON tg.GenderCode = msh.Gender
                    LEFT JOIN profile.tblprofile tp ON tp.Profileid = msh.PatientID
                    WHERE msh.PatientID = @PatientID";
                
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PatientID", patientId.ToString());
                using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    var patientInfo = new LabTestData
                    {
                        NHINumber = reader.IsDBNull(reader.GetOrdinal("NHINumber")) ? null : reader.GetString(reader.GetOrdinal("NHINumber")),
                        FullName = reader.IsDBNull(reader.GetOrdinal("FullName")) ? null : reader.GetString(reader.GetOrdinal("FullName")),
                        DOB = reader.IsDBNull(reader.GetOrdinal("DOB")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("DOB")),
                        GenderName = reader.IsDBNull(reader.GetOrdinal("GenderName")) ? null : reader.GetString(reader.GetOrdinal("GenderName")),
                        PatientID = reader.IsDBNull(reader.GetOrdinal("PatientID")) ? null : reader.GetInt32(reader.GetOrdinal("PatientID")).ToString(),
                        PracticeID = reader.IsDBNull(reader.GetOrdinal("PracticeID")) ? null : reader.GetInt32(reader.GetOrdinal("PracticeID")).ToString(),
                        Ethnicity = null // Set to null since the column doesn't exist in the database
                    };
                    
                    Console.WriteLine($"✅ Retrieved patient information for patient {patientId}");
                    return patientInfo;
                }
                
                Console.WriteLine($"⚠️ No patient information found for patient {patientId}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting patient information: {ex.Message}");
                return null;
            }
        }

        public async Task<PatientInfo?> GetPatientInfoByIDAsync(long patientId)
        {
            try
            {
                Console.WriteLine($"🔍 Getting patient information for patient ID: {patientId}");
                Console.WriteLine($"📋 Connection String: {_connectionString}");
                
                using (var connection = new SqlConnection(_connectionString))
                {
                    Console.WriteLine("⏳ Opening connection...");
                    await connection.OpenAsync();
                    Console.WriteLine("✅ Successfully connected to database!");
                    
                    // Try to execute the stored procedure directly without checking if it exists
                    Console.WriteLine("⏳ Attempting to execute GetPatientnameforLAB stored procedure...");
                    
                    Console.WriteLine("⏳ Executing GetPatientnameforLAB stored procedure...");
                    using var command = new SqlCommand("EXEC [dbo].[GetPatientnameforLAB] @pPatientID", connection);
                    command.Parameters.AddWithValue("@pPatientID", patientId);
                    using var reader = await command.ExecuteReaderAsync();
                    
                    Console.WriteLine("📋 Reading data from stored procedure...");
                    Console.WriteLine($"📊 Number of columns: {reader.FieldCount}");
                    
                    // Print column information
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.WriteLine($"  Column {i}: {reader.GetName(i)} ({reader.GetDataTypeName(i)})");
                    }
                    
                    if (await reader.ReadAsync())
                    {
                        try
                        {
                            var patientInfo = new PatientInfo
                            {
                                FullName = reader.IsDBNull(reader.GetOrdinal("FullName")) ? null : reader.GetString(reader.GetOrdinal("FullName")),
                                DOB = reader.IsDBNull(reader.GetOrdinal("DOB")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("DOB")),
                                GenderName = reader.IsDBNull(reader.GetOrdinal("GenderName")) ? null : reader.GetString(reader.GetOrdinal("GenderName")),
                                ProfileID = reader.IsDBNull(reader.GetOrdinal("ProfileiD")) ? null : reader.GetInt32(reader.GetOrdinal("ProfileiD")).ToString(),
                                PracticeID = reader.IsDBNull(reader.GetOrdinal("PracticeID")) ? null : reader.GetInt32(reader.GetOrdinal("PracticeID")).ToString(),
                                Ethnicity = reader.IsDBNull(reader.GetOrdinal("Ethnicity")) ? null : reader.GetString(reader.GetOrdinal("Ethnicity")),
                                PatientName = reader.IsDBNull(reader.GetOrdinal("PatientName")) ? null : reader.GetString(reader.GetOrdinal("PatientName")),
                                NhiNumber = reader.IsDBNull(reader.GetOrdinal("NhiNumber")) ? null : reader.GetString(reader.GetOrdinal("NhiNumber")),
                                Age = reader.IsDBNull(reader.GetOrdinal("Age")) ? null : int.TryParse(reader.GetString(reader.GetOrdinal("Age")), out int age) ? age : null
                            };
                            
                            Console.WriteLine($"✅ Retrieved patient information for patient {patientId}");
                            return patientInfo;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Error reading patient data: {ex.Message}");
                            return null;
                        }
                    }
                    
                    Console.WriteLine($"⚠️ No patient information found for patient {patientId}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database connection failed: {ex.Message}");
                Console.WriteLine($"📋 Connection string: {_connectionString}");
                return null;
            }
        }

        public async Task<PatientLabTestResponse?> GetPatientLabTestDataUpdatedAsync(long patientId, long? labTestMshID = null)
        {
            try
            {
                Console.WriteLine($"🔍 Getting updated lab test data for patient ID: {patientId}");
                if (labTestMshID.HasValue)
                {
                    Console.WriteLine($"🔍 Filtering by Lab Test MSH ID: {labTestMshID.Value}");
                }
                Console.WriteLine($"📋 Connection String: {_connectionString}");
                
                using (var connection = new SqlConnection(_connectionString))
                {
                    Console.WriteLine("⏳ Opening connection...");
                    await connection.OpenAsync();
                    Console.WriteLine("✅ Successfully connected to database!");
                    
                    // Execute the updated stored procedure with both parameters
                    Console.WriteLine("⏳ Executing GetPatientLabTestData stored procedure...");
                    
                    using var command = new SqlCommand("EXEC [dbo].[GetPatientLabTestData] @pPatientID, @pLabTestMshID", connection);
                    command.Parameters.AddWithValue("@pPatientID", patientId);
                    command.Parameters.AddWithValue("@pLabTestMshID", labTestMshID ?? (object)DBNull.Value);
                    
                    // Set command timeout to prevent long-running queries
                    command.CommandTimeout = 300; // 5 minutes
                    
                    using var reader = await command.ExecuteReaderAsync();
                    
                    var response = new PatientLabTestResponse();
                    
                    // Read first dataset (Header/Patient information)
                    Console.WriteLine("📋 Reading first dataset (Header information)...");
                    Console.WriteLine($"📊 Number of columns: {reader.FieldCount}");
                    
                    // Print column information for first dataset
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.WriteLine($"  Column {i}: {reader.GetName(i)} ({reader.GetDataTypeName(i)})");
                    }
                    
                    if (await reader.ReadAsync())
                    {
                        try
                        {
                            Console.WriteLine("🔍 Reading header fields with detailed logging:");
                            
                            // NHINumber
                            var nhiNumberOrdinal = reader.GetOrdinal("NHINumber");
                            var nhiNumberValue = reader.IsDBNull(nhiNumberOrdinal) ? null : reader.GetString(nhiNumberOrdinal);
                            Console.WriteLine($"  NHINumber: {nhiNumberValue} (Type: {reader.GetDataTypeName(nhiNumberOrdinal)})");
                            
                            // FullName
                            var fullNameOrdinal = reader.GetOrdinal("FullName");
                            var fullNameValue = reader.IsDBNull(fullNameOrdinal) ? null : reader.GetString(fullNameOrdinal);
                            Console.WriteLine($"  FullName: {fullNameValue} (Type: {reader.GetDataTypeName(fullNameOrdinal)})");
                            
                            // DOB
                            var dobOrdinal = reader.GetOrdinal("DOB");
                            var dobValue = reader.IsDBNull(dobOrdinal) ? DateTime.MinValue : reader.GetDateTime(dobOrdinal);
                            Console.WriteLine($"  DOB: {dobValue} (Type: {reader.GetDataTypeName(dobOrdinal)})");
                            
                            // GenderName
                            var genderNameOrdinal = reader.GetOrdinal("GenderName");
                            var genderNameValue = reader.IsDBNull(genderNameOrdinal) ? null : reader.GetString(genderNameOrdinal);
                            Console.WriteLine($"  GenderName: {genderNameValue} (Type: {reader.GetDataTypeName(genderNameOrdinal)})");
                            
                            // PatientID
                            var patientIdOrdinal = reader.GetOrdinal("PatientID");
                            var patientIdValue = GetNullableInt32(reader, "PatientID")?.ToString();
                            Console.WriteLine($"  PatientID: {patientIdValue} (Type: {reader.GetDataTypeName(patientIdOrdinal)})");
                            
                            // PracticeID
                            var practiceIdOrdinal = reader.GetOrdinal("PracticeID");
                            var practiceIdValue = GetNullableInt32(reader, "PracticeID")?.ToString();
                            Console.WriteLine($"  PracticeID: {practiceIdValue} (Type: {reader.GetDataTypeName(practiceIdOrdinal)})");
                            
                            // MshInsertedAt
                            var mshInsertedAtOrdinal = reader.GetOrdinal("MshInsertedAt");
                            var mshInsertedAtValue = reader.IsDBNull(mshInsertedAtOrdinal) ? DateTime.MinValue : reader.GetDateTime(mshInsertedAtOrdinal);
                            Console.WriteLine($"  MshInsertedAt: {mshInsertedAtValue} (Type: {reader.GetDataTypeName(mshInsertedAtOrdinal)})");
                            
                            // Ethnicity
                            var ethnicityOrdinal = reader.GetOrdinal("Ethnicity");
                            var ethnicityValue = reader.IsDBNull(ethnicityOrdinal) ? null : reader.GetString(ethnicityOrdinal);
                            Console.WriteLine($"  Ethnicity: {ethnicityValue} (Type: {reader.GetDataTypeName(ethnicityOrdinal)})");
                            
                            // Age
                            var ageOrdinal = reader.GetOrdinal("Age");
                            int? ageValue = null;
                            if (!reader.IsDBNull(ageOrdinal))
                            {
                                var dataType = reader.GetDataTypeName(ageOrdinal).ToLower();
                                if (dataType == "decimal" || dataType == "numeric")
                                {
                                    var decimalValue = reader.GetDecimal(ageOrdinal);
                                    ageValue = (int)decimalValue;
                                }
                                else if (dataType == "int" || dataType == "bigint")
                                {
                                    ageValue = reader.GetInt32(ageOrdinal);
                                }
                                else
                                {
                                    // Try as string
                                    if (int.TryParse(reader.GetString(ageOrdinal), out int age))
                                    {
                                        ageValue = age;
                                    }
                                }
                            }
                            Console.WriteLine($"  Age: {ageValue} (Type: {reader.GetDataTypeName(ageOrdinal)})");
                            
                            var header = new PatientLabTestHeader
                            {
                                NHINumber = nhiNumberValue,
                                FullName = fullNameValue,
                                DOB = dobValue,
                                GenderName = genderNameValue,
                                PatientID = patientIdValue,
                                PracticeID = practiceIdValue,
                                MshInsertedAt = mshInsertedAtValue,
                                Ethnicity = ethnicityValue,
                                Age = ageValue
                            };
                            
                            response.Header = header;
                            Console.WriteLine($"✅ Retrieved header information for patient {patientId}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Error reading header data: {ex.Message}");
                            Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                            // Continue processing other datasets even if header fails
                        }
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ No header information found for patient {patientId}");
                    }
                    
                    // Move to next result set (second dataset)
                    if (await reader.NextResultAsync())
                    {
                        Console.WriteLine("📋 Reading second dataset (Lab Test Detail information)...");
                        Console.WriteLine($"📊 Number of columns: {reader.FieldCount}");
                        
                        // Print column information for second dataset
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.WriteLine($"  Column {i}: {reader.GetName(i)} ({reader.GetDataTypeName(i)})");
                        }
                        
                        int detailCount = 0;
                        while (await reader.ReadAsync())
                        {
                            try
                            {
                                detailCount++;
                                Console.WriteLine($"🔍 Reading lab test detail record #{detailCount}:");
                                
                                // LabTestOBRID
                                var labTestOBRIDOrdinal = reader.GetOrdinal("LabTestOBRID");
                                var labTestOBRIDValue = reader.IsDBNull(labTestOBRIDOrdinal) ? 0 : reader.GetInt32(labTestOBRIDOrdinal);
                                Console.WriteLine($"  LabTestOBRID: {labTestOBRIDValue} (Type: {reader.GetDataTypeName(labTestOBRIDOrdinal)})");
                                
                                // SnomedCode
                                var snomedCodeOrdinal = reader.GetOrdinal("SnomedCode");
                                var snomedCodeValue = reader.IsDBNull(snomedCodeOrdinal) ? null : reader.GetString(snomedCodeOrdinal);
                                Console.WriteLine($"  SnomedCode: {snomedCodeValue} (Type: {reader.GetDataTypeName(snomedCodeOrdinal)})");
                                
                                // MessageSubject
                                var messageSubjectOrdinal = reader.GetOrdinal("MessageSubject");
                                var messageSubjectValue = reader.IsDBNull(messageSubjectOrdinal) ? null : reader.GetString(messageSubjectOrdinal);
                                Console.WriteLine($"  MessageSubject: {messageSubjectValue} (Type: {reader.GetDataTypeName(messageSubjectOrdinal)})");
                                
                                // ObservationDateTime
                                var observationDateTimeOrdinal = reader.GetOrdinal("ObservationDateTime");
                                var observationDateTimeValue = reader.IsDBNull(observationDateTimeOrdinal) ? DateTime.MinValue : reader.GetDateTime(observationDateTimeOrdinal);
                                Console.WriteLine($"  ObservationDateTime: {observationDateTimeValue} (Type: {reader.GetDataTypeName(observationDateTimeOrdinal)})");
                                
                                // StatusChangeDateTime
                                var statusChangeDateTimeOrdinal = reader.GetOrdinal("StatusChangeDateTime");
                                var statusChangeDateTimeValue = reader.IsDBNull(statusChangeDateTimeOrdinal) ? DateTime.MinValue : reader.GetDateTime(statusChangeDateTimeOrdinal);
                                Console.WriteLine($"  StatusChangeDateTime: {statusChangeDateTimeValue} (Type: {reader.GetDataTypeName(statusChangeDateTimeOrdinal)})");
                                
                                // AppointmentID
                                var appointmentIDOrdinal = reader.GetOrdinal("AppointmentID");
                                var appointmentIDValue = GetNullableInt32(reader, "AppointmentID")?.ToString();
                                Console.WriteLine($"  AppointmentID: {appointmentIDValue} (Type: {reader.GetDataTypeName(appointmentIDOrdinal)})");
                                
                                // LabTestOBXID
                                var labTestOBXIDOrdinal = reader.GetOrdinal("LabTestOBXID");
                                var labTestOBXIDValue = reader.IsDBNull(labTestOBXIDOrdinal) ? 0 : reader.GetInt32(labTestOBXIDOrdinal);
                                Console.WriteLine($"  LabTestOBXID: {labTestOBXIDValue} (Type: {reader.GetDataTypeName(labTestOBXIDOrdinal)})");
                                
                                // SnomedCode_2
                                var snomedCode2Ordinal = reader.GetOrdinal("SnomedCode_2");
                                var snomedCode2Value = reader.IsDBNull(snomedCode2Ordinal) ? null : reader.GetString(snomedCode2Ordinal);
                                Console.WriteLine($"  SnomedCode_2: {snomedCode2Value} (Type: {reader.GetDataTypeName(snomedCode2Ordinal)})");
                                
                                // ResultName
                                var resultNameOrdinal = reader.GetOrdinal("ResultName");
                                var resultNameValue = reader.IsDBNull(resultNameOrdinal) ? null : reader.GetString(resultNameOrdinal);
                                Console.WriteLine($"  ResultName: {resultNameValue} (Type: {reader.GetDataTypeName(resultNameOrdinal)})");
                                
                                // ObservationCodingSystem
                                var observationCodingSystemOrdinal = reader.GetOrdinal("ObservationCodingSystem");
                                var observationCodingSystemValue = reader.IsDBNull(observationCodingSystemOrdinal) ? null : reader.GetString(observationCodingSystemOrdinal);
                                Console.WriteLine($"  ObservationCodingSystem: {observationCodingSystemValue} (Type: {reader.GetDataTypeName(observationCodingSystemOrdinal)})");
                                
                                // ObservationValue
                                var observationValueOrdinal = reader.GetOrdinal("ObservationValue");
                                var observationValueValue = reader.IsDBNull(observationValueOrdinal) ? null : reader.GetString(observationValueOrdinal);
                                Console.WriteLine($"  ObservationValue: {observationValueValue} (Type: {reader.GetDataTypeName(observationValueOrdinal)})");
                                
                                // Units
                                var unitsOrdinal = reader.GetOrdinal("Units");
                                var unitsValue = reader.IsDBNull(unitsOrdinal) ? null : reader.GetString(unitsOrdinal);
                                Console.WriteLine($"  Units: {unitsValue} (Type: {reader.GetDataTypeName(unitsOrdinal)})");
                                
                                // ReferenceRanges
                                var referenceRangesOrdinal = reader.GetOrdinal("ReferenceRanges");
                                var referenceRangesValue = reader.IsDBNull(referenceRangesOrdinal) ? null : reader.GetString(referenceRangesOrdinal);
                                Console.WriteLine($"  ReferenceRanges: {referenceRangesValue} (Type: {reader.GetDataTypeName(referenceRangesOrdinal)})");
                                
                                // AbnormalFlagID
                                var abnormalFlagIDOrdinal = reader.GetOrdinal("AbnormalFlagID");
                                var abnormalFlagIDValue = reader.IsDBNull(abnormalFlagIDOrdinal) ? 0 : reader.GetInt32(abnormalFlagIDOrdinal);
                                Console.WriteLine($"  AbnormalFlagID: {abnormalFlagIDValue} (Type: {reader.GetDataTypeName(abnormalFlagIDOrdinal)})");
                                
                                // AbnormalFlagDesc
                                var abnormalFlagDescOrdinal = reader.GetOrdinal("AbnormalFlagDesc");
                                var abnormalFlagDescValue = reader.IsDBNull(abnormalFlagDescOrdinal) ? null : reader.GetString(abnormalFlagDescOrdinal);
                                Console.WriteLine($"  AbnormalFlagDesc: {abnormalFlagDescValue} (Type: {reader.GetDataTypeName(abnormalFlagDescOrdinal)})");
                                
                                // LabTestNTEID
                                var labTestNTEIDOrdinal = reader.GetOrdinal("LabTestNTEID");
                                var labTestNTEIDValue = reader.IsDBNull(labTestNTEIDOrdinal) ? 0 : reader.GetInt32(labTestNTEIDOrdinal);
                                Console.WriteLine($"  LabTestNTEID: {labTestNTEIDValue} (Type: {reader.GetDataTypeName(labTestNTEIDOrdinal)})");
                                
                                // Source
                                var sourceOrdinal = reader.GetOrdinal("Source");
                                var sourceValue = reader.IsDBNull(sourceOrdinal) ? null : reader.GetString(sourceOrdinal);
                                Console.WriteLine($"  Source: {sourceValue} (Type: {reader.GetDataTypeName(sourceOrdinal)})");
                                
                                // Comments
                                var commentsOrdinal = reader.GetOrdinal("Comments");
                                var commentsValue = reader.IsDBNull(commentsOrdinal) ? null : reader.GetString(commentsOrdinal);
                                Console.WriteLine($"  Comments: {commentsValue} (Type: {reader.GetDataTypeName(commentsOrdinal)})");
                                
                                // PriorityID
                                var priorityIDOrdinal = reader.GetOrdinal("PriorityID");
                                var priorityIDValue = reader.IsDBNull(priorityIDOrdinal) ? 0 : reader.GetInt32(priorityIDOrdinal);
                                Console.WriteLine($"  PriorityID: {priorityIDValue} (Type: {reader.GetDataTypeName(priorityIDOrdinal)})");
                                
                                var detail = new PatientLabTestDetail
                                {
                                    LabTestOBRID = labTestOBRIDValue,
                                    SnomedCode = snomedCodeValue,
                                    MessageSubject = messageSubjectValue,
                                    ObservationDateTime = observationDateTimeValue,
                                    StatusChangeDateTime = statusChangeDateTimeValue,
                                    AppointmentID = appointmentIDValue,
                                    LabTestOBXID = labTestOBXIDValue,
                                    SnomedCode_2 = snomedCode2Value,
                                    ResultName = resultNameValue,
                                    ObservationCodingSystem = observationCodingSystemValue,
                                    ObservationValue = observationValueValue,
                                    Units = unitsValue,
                                    ReferenceRanges = referenceRangesValue,
                                    AbnormalFlagID = abnormalFlagIDValue,
                                    AbnormalFlagDesc = abnormalFlagDescValue,
                                    LabTestNTEID = labTestNTEIDValue,
                                    Source = sourceValue,
                                    Comments = commentsValue,
                                    PriorityID = priorityIDValue
                                };
                                
                                response.LabTestDetails.Add(detail);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"❌ Error reading lab test detail data (record #{detailCount}): {ex.Message}");
                                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                                // Continue processing other records
                            }
                        }
                        
                        Console.WriteLine($"✅ Retrieved {response.LabTestDetails.Count} lab test detail records for patient {patientId}");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ No lab test detail information found for patient {patientId}");
                    }
                    
                    // Move to next result set (third dataset - Allergies)
                    if (await reader.NextResultAsync())
                    {
                        Console.WriteLine("📋 Reading third dataset (Allergies information)...");
                        Console.WriteLine($"📊 Number of columns: {reader.FieldCount}");
                        
                        // Print column information for third dataset
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.WriteLine($"  Column {i}: {reader.GetName(i)} ({reader.GetDataTypeName(i)})");
                        }
                        
                        int allergyCount = 0;
                        while (await reader.ReadAsync())
                        {
                            try
                            {
                                allergyCount++;
                                Console.WriteLine($"🔍 Reading allergy record #{allergyCount}:");
                                
                                // AllergyID
                                var allergyIDOrdinal = reader.GetOrdinal("AllergyID");
                                var allergyIDValue = reader.IsDBNull(allergyIDOrdinal) ? 0 : reader.GetInt32(allergyIDOrdinal);
                                Console.WriteLine($"  AllergyID: {allergyIDValue} (Type: {reader.GetDataTypeName(allergyIDOrdinal)})");
                                
                                // AllergyUUID
                                var allergyUUIDOrdinal = reader.GetOrdinal("AllergyUUID");
                                var allergyUUIDValue = GetNullableString(reader, "AllergyUUID");
                                Console.WriteLine($"  AllergyUUID: {allergyUUIDValue} (Type: {reader.GetDataTypeName(allergyUUIDOrdinal)})");
                                
                                // IsReviewed
                                var isReviewedOrdinal = reader.GetOrdinal("IsReviewed");
                                var isReviewedValue = reader.IsDBNull(isReviewedOrdinal) ? false : reader.GetBoolean(isReviewedOrdinal);
                                Console.WriteLine($"  IsReviewed: {isReviewedValue} (Type: {reader.GetDataTypeName(isReviewedOrdinal)})");
                                
                                // MedTechID
                                var medTechIDOrdinal = reader.GetOrdinal("MedTechID");
                                var medTechIDValue = GetNullableInt32(reader, "MedTechID");
                                Console.WriteLine($"  MedTechID: {medTechIDValue} (Type: {reader.GetDataTypeName(medTechIDOrdinal)})");
                                
                                // OnsetDate
                                var onsetDateOrdinal = reader.GetOrdinal("OnsetDate");
                                var onsetDateValue = GetNullableDateTime(reader, "OnsetDate");
                                Console.WriteLine($"  OnsetDate: {onsetDateValue} (Type: {reader.GetDataTypeName(onsetDateOrdinal)})");
                                
                                // AllergyTypeID
                                var allergyTypeIDOrdinal = reader.GetOrdinal("AllergyTypeID");
                                var allergyTypeIDValue = GetNullableInt32(reader, "AllergyTypeID");
                                Console.WriteLine($"  AllergyTypeID: {allergyTypeIDValue} (Type: {reader.GetDataTypeName(allergyTypeIDOrdinal)})");
                                
                                // MedicineTypeID
                                var medicineTypeIDOrdinal = reader.GetOrdinal("MedicineTypeID");
                                var medicineTypeIDValue = GetNullableInt32(reader, "MedicineTypeID");
                                Console.WriteLine($"  MedicineTypeID: {medicineTypeIDValue} (Type: {reader.GetDataTypeName(medicineTypeIDOrdinal)})");
                                
                                // MedicineShortName
                                var medicineShortNameOrdinal = reader.GetOrdinal("MedicineShortName");
                                var medicineShortNameValue = GetNullableString(reader, "MedicineShortName");
                                Console.WriteLine($"  MedicineShortName: {medicineShortNameValue} (Type: {reader.GetDataTypeName(medicineShortNameOrdinal)})");
                                
                                // MedicineClassification
                                var medicineClassificationOrdinal = reader.GetOrdinal("MedicineClassification");
                                var medicineClassificationValue = GetNullableString(reader, "MedicineClassification");
                                Console.WriteLine($"  MedicineClassification: {medicineClassificationValue} (Type: {reader.GetDataTypeName(medicineClassificationOrdinal)})");
                                
                                // FavouriteSubstance
                                var favouriteSubstanceOrdinal = reader.GetOrdinal("FavouriteSubstance");
                                var favouriteSubstanceValue = GetNullableString(reader, "FavouriteSubstance");
                                Console.WriteLine($"  FavouriteSubstance: {favouriteSubstanceValue} (Type: {reader.GetDataTypeName(favouriteSubstanceOrdinal)})");
                                
                                // DiseaseName
                                var diseaseNameOrdinal = reader.GetOrdinal("DiseaseName");
                                var diseaseNameValue = GetNullableString(reader, "DiseaseName");
                                Console.WriteLine($"  DiseaseName: {diseaseNameValue} (Type: {reader.GetDataTypeName(diseaseNameOrdinal)})");
                                
                                // SubstanceTypeId
                                var substanceTypeIdOrdinal = reader.GetOrdinal("SubstanceTypeId");
                                var substanceTypeIdValue = GetNullableInt32(reader, "SubstanceTypeId");
                                Console.WriteLine($"  SubstanceTypeId: {substanceTypeIdValue} (Type: {reader.GetDataTypeName(substanceTypeIdOrdinal)})");
                                
                                // Other
                                var otherOrdinal = reader.GetOrdinal("Other");
                                var otherValue = GetNullableString(reader, "Other");
                                Console.WriteLine($"  Other: {otherValue} (Type: {reader.GetDataTypeName(otherOrdinal)})");
                                
                                // Reaction
                                var reactionOrdinal = reader.GetOrdinal("Reaction");
                                var reactionValue = GetNullableString(reader, "Reaction");
                                Console.WriteLine($"  Reaction: {reactionValue} (Type: {reader.GetDataTypeName(reactionOrdinal)})");
                                
                                // IsActive
                                var isActiveOrdinal = reader.GetOrdinal("IsActive");
                                var isActiveValue = reader.IsDBNull(isActiveOrdinal) ? false : reader.GetBoolean(isActiveOrdinal);
                                Console.WriteLine($"  IsActive: {isActiveValue} (Type: {reader.GetDataTypeName(isActiveOrdinal)})");
                                
                                // FullName
                                var fullNameOrdinal = reader.GetOrdinal("FullName");
                                var fullNameValue = GetNullableString(reader, "FullName");
                                Console.WriteLine($"  FullName: {fullNameValue} (Type: {reader.GetDataTypeName(fullNameOrdinal)})");
                                
                                // Comment
                                var commentOrdinal = reader.GetOrdinal("Comment");
                                var commentValue = GetNullableString(reader, "Comment");
                                Console.WriteLine($"  Comment: {commentValue} (Type: {reader.GetDataTypeName(commentOrdinal)})");
                                
                                // IsHighlight
                                var isHighlightOrdinal = reader.GetOrdinal("IsHighlight");
                                var isHighlightValue = reader.IsDBNull(isHighlightOrdinal) ? false : reader.GetBoolean(isHighlightOrdinal);
                                Console.WriteLine($"  IsHighlight: {isHighlightValue} (Type: {reader.GetDataTypeName(isHighlightOrdinal)})");
                                
                                // InsertedAt
                                var insertedAtOrdinal = reader.GetOrdinal("InsertedAt");
                                var insertedAtValue = GetNullableDateTime(reader, "InsertedAt");
                                Console.WriteLine($"  InsertedAt: {insertedAtValue} (Type: {reader.GetDataTypeName(insertedAtOrdinal)})");
                                
                                // AllergyType
                                var allergyTypeOrdinal = reader.GetOrdinal("AllergyType");
                                var allergyTypeValue = GetNullableString(reader, "AllergyType");
                                Console.WriteLine($"  AllergyType: {allergyTypeValue} (Type: {reader.GetDataTypeName(allergyTypeOrdinal)})");
                                
                                // Name
                                var nameOrdinal = reader.GetOrdinal("Name");
                                var nameValue = GetNullableString(reader, "Name");
                                Console.WriteLine($"  Name: {nameValue} (Type: {reader.GetDataTypeName(nameOrdinal)})");
                                
                                // IsNKA
                                var isNKAOrdinal = reader.GetOrdinal("IsNKA");
                                var isNKAValue = reader.IsDBNull(isNKAOrdinal) ? false : reader.GetBoolean(isNKAOrdinal);
                                Console.WriteLine($"  IsNKA: {isNKAValue} (Type: {reader.GetDataTypeName(isNKAOrdinal)})");
                                
                                // SequenceNo
                                var sequenceNoOrdinal = reader.GetOrdinal("SequenceNo");
                                var sequenceNoValue = GetNullableInt32(reader, "SequenceNo");
                                Console.WriteLine($"  SequenceNo: {sequenceNoValue} (Type: {reader.GetDataTypeName(sequenceNoOrdinal)})");
                                
                                // Severity
                                var severityOrdinal = reader.GetOrdinal("Severity");
                                var severityValue = GetNullableString(reader, "Severity");
                                Console.WriteLine($"  Severity: {severityValue} (Type: {reader.GetDataTypeName(severityOrdinal)})");
                                
                                var allergy = new PatientAllergy
                                {
                                    AllergyID = allergyIDValue,
                                    AllergyUUID = allergyUUIDValue,
                                    IsReviewed = isReviewedValue,
                                    MedTechID = medTechIDValue,
                                    OnsetDate = onsetDateValue,
                                    AllergyTypeID = allergyTypeIDValue,
                                    MedicineTypeID = medicineTypeIDValue,
                                    MedicineShortName = medicineShortNameValue,
                                    MedicineClassification = medicineClassificationValue,
                                    FavouriteSubstance = favouriteSubstanceValue,
                                    DiseaseName = diseaseNameValue,
                                    SubstanceTypeId = substanceTypeIdValue,
                                    Other = otherValue,
                                    Reaction = reactionValue,
                                    IsActive = isActiveValue,
                                    FullName = fullNameValue,
                                    Comment = commentValue,
                                    IsHighlight = isHighlightValue,
                                    InsertedAt = insertedAtValue,
                                    AllergyType = allergyTypeValue,
                                    Name = nameValue,
                                    IsNKA = isNKAValue,
                                    SequenceNo = sequenceNoValue,
                                    Severity = severityValue
                                };
                                
                                response.Allergies.Add(allergy);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"❌ Error reading allergy data (record #{allergyCount}): {ex.Message}");
                                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                                // Continue processing other records
                            }
                        }
                        
                        Console.WriteLine($"✅ Retrieved {response.Allergies.Count} allergy records for patient {patientId}");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ No allergy information found for patient {patientId}");
                    }
                    
                    // Move to next result set (fourth dataset - Diagnoses)
                    if (await reader.NextResultAsync())
                    {
                        Console.WriteLine("📋 Reading fourth dataset (Diagnoses information)...");
                        Console.WriteLine($"📊 Number of columns: {reader.FieldCount}");
                        
                        // Print column information for fourth dataset
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.WriteLine($"  Column {i}: {reader.GetName(i)} ({reader.GetDataTypeName(i)})");
                        }
                        
                        int diagnosisCount = 0;
                        while (await reader.ReadAsync())
                        {
                            try
                            {
                                diagnosisCount++;
                                Console.WriteLine($"🔍 Reading diagnosis record #{diagnosisCount}:");
                                
                                // DiagnosisID
                                var diagnosisIDOrdinal = reader.GetOrdinal("DiagnosisID");
                                var diagnosisIDValue = reader.IsDBNull(diagnosisIDOrdinal) ? 0 : reader.GetInt32(diagnosisIDOrdinal);
                                Console.WriteLine($"  DiagnosisID: {diagnosisIDValue} (Type: {reader.GetDataTypeName(diagnosisIDOrdinal)})");
                                
                                // AppointmentID
                                var appointmentIDOrdinal = reader.GetOrdinal("AppointmentID");
                                var appointmentIDValue = GetNullableInt32(reader, "AppointmentID");
                                Console.WriteLine($"  AppointmentID: {appointmentIDValue} (Type: {reader.GetDataTypeName(appointmentIDOrdinal)})");
                                
                                // DiseaseName
                                var diseaseNameOrdinal = reader.GetOrdinal("DiseaseName");
                                var diseaseNameValue = GetNullableString(reader, "DiseaseName");
                                Console.WriteLine($"  DiseaseName: {diseaseNameValue} (Type: {reader.GetDataTypeName(diseaseNameOrdinal)})");
                                
                                // DiagnosisDate
                                var diagnosisDateOrdinal = reader.GetOrdinal("DiagnosisDate");
                                var diagnosisDateValue = GetNullableDateTime(reader, "DiagnosisDate");
                                Console.WriteLine($"  DiagnosisDate: {diagnosisDateValue} (Type: {reader.GetDataTypeName(diagnosisDateOrdinal)})");
                                
                                // DiagnosisBy
                                var diagnosisByOrdinal = reader.GetOrdinal("DiagnosisBy");
                                var diagnosisByValue = GetNullableInt32(reader, "DiagnosisBy")?.ToString();
                                Console.WriteLine($"  DiagnosisBy: {diagnosisByValue} (Type: {reader.GetDataTypeName(diagnosisByOrdinal)})");
                                
                                // Summary
                                var summaryOrdinal = reader.GetOrdinal("Summary");
                                var summaryValue = GetNullableString(reader, "Summary");
                                Console.WriteLine($"  Summary: {summaryValue} (Type: {reader.GetDataTypeName(summaryOrdinal)})");
                                
                                // IsLongTerm
                                var isLongTermOrdinal = reader.GetOrdinal("IsLongTerm");
                                var isLongTermValue = reader.IsDBNull(isLongTermOrdinal) ? false : reader.GetBoolean(isLongTermOrdinal);
                                Console.WriteLine($"  IsLongTerm: {isLongTermValue} (Type: {reader.GetDataTypeName(isLongTermOrdinal)})");
                                
                                // AddtoProblem
                                var addtoProblemOrdinal = reader.GetOrdinal("AddtoProblem");
                                var addtoProblemValue = reader.IsDBNull(addtoProblemOrdinal) ? false : reader.GetBoolean(addtoProblemOrdinal);
                                Console.WriteLine($"  AddtoProblem: {addtoProblemValue} (Type: {reader.GetDataTypeName(addtoProblemOrdinal)})");
                                
                                // IsHighlighted
                                var isHighlightedOrdinal = reader.GetOrdinal("IsHighlighted");
                                var isHighlightedValue = reader.IsDBNull(isHighlightedOrdinal) ? false : reader.GetBoolean(isHighlightedOrdinal);
                                Console.WriteLine($"  IsHighlighted: {isHighlightedValue} (Type: {reader.GetDataTypeName(isHighlightedOrdinal)})");
                                
                                // SequenceNo
                                var sequenceNoOrdinal = reader.GetOrdinal("SequenceNo");
                                var sequenceNoValue = GetNullableByte(reader, "SequenceNo");
                                Console.WriteLine($"  SequenceNo: {sequenceNoValue} (Type: {reader.GetDataTypeName(sequenceNoOrdinal)})");
                                
                                // IsActive
                                var isActiveOrdinal = reader.GetOrdinal("IsActive");
                                var isActiveValue = reader.IsDBNull(isActiveOrdinal) ? false : reader.GetBoolean(isActiveOrdinal);
                                Console.WriteLine($"  IsActive: {isActiveValue} (Type: {reader.GetDataTypeName(isActiveOrdinal)})");
                                
                                // IsConfidential
                                var isConfidentialOrdinal = reader.GetOrdinal("IsConfidential");
                                var isConfidentialValue = reader.IsDBNull(isConfidentialOrdinal) ? false : reader.GetBoolean(isConfidentialOrdinal);
                                Console.WriteLine($"  IsConfidential: {isConfidentialValue} (Type: {reader.GetDataTypeName(isConfidentialOrdinal)})");
                                
                                // DiagnosisType
                                var diagnosisTypeOrdinal = reader.GetOrdinal("DiagnosisType");
                                var diagnosisTypeValue = GetNullableString(reader, "DiagnosisType");
                                Console.WriteLine($"  DiagnosisType: {diagnosisTypeValue} (Type: {reader.GetDataTypeName(diagnosisTypeOrdinal)})");
                                
                                // IsMapped
                                var isMappedOrdinal = reader.GetOrdinal("IsMapped");
                                var isMappedValue = reader.IsDBNull(isMappedOrdinal) ? false : reader.GetBoolean(isMappedOrdinal);
                                Console.WriteLine($"  IsMapped: {isMappedValue} (Type: {reader.GetDataTypeName(isMappedOrdinal)})");
                                
                                // PracticeID
                                var practiceIDOrdinal = reader.GetOrdinal("PracticeID");
                                var practiceIDValue = GetNullableInt32(reader, "PracticeID");
                                Console.WriteLine($"  PracticeID: {practiceIDValue} (Type: {reader.GetDataTypeName(practiceIDOrdinal)})");
                                
                                // OnSetDate
                                var onSetDateOrdinal = reader.GetOrdinal("OnSetDate");
                                var onSetDateValue = GetNullableDateTime(reader, "OnSetDate");
                                Console.WriteLine($"  OnSetDate: {onSetDateValue} (Type: {reader.GetDataTypeName(onSetDateOrdinal)})");
                                
                                // MappedBy
                                var mappedByOrdinal = reader.GetOrdinal("MappedBy");
                                var mappedByValue = GetNullableInt32(reader, "MappedBy")?.ToString();
                                Console.WriteLine($"  MappedBy: {mappedByValue} (Type: {reader.GetDataTypeName(mappedByOrdinal)})");
                                
                                // MappedDate
                                var mappedDateOrdinal = reader.GetOrdinal("MappedDate");
                                var mappedDateValue = GetNullableDateTime(reader, "MappedDate");
                                Console.WriteLine($"  MappedDate: {mappedDateValue} (Type: {reader.GetDataTypeName(mappedDateOrdinal)})");
                                
                                // IsStopped
                                var isStoppedOrdinal = reader.GetOrdinal("IsStopped");
                                var isStoppedValue = reader.IsDBNull(isStoppedOrdinal) ? false : reader.GetBoolean(isStoppedOrdinal);
                                Console.WriteLine($"  IsStopped: {isStoppedValue} (Type: {reader.GetDataTypeName(isStoppedOrdinal)})");
                                
                                // SnomedDiseaseName
                                var snomedDiseaseNameOrdinal = reader.GetOrdinal("SnomedDiseaseName");
                                var snomedDiseaseNameValue = GetNullableString(reader, "SnomedDiseaseName");
                                Console.WriteLine($"  SnomedDiseaseName: {snomedDiseaseNameValue} (Type: {reader.GetDataTypeName(snomedDiseaseNameOrdinal)})");
                                
                                // PatientID
                                var patientIDOrdinal = reader.GetOrdinal("PatientID");
                                var patientIDValue = GetNullableInt32(reader, "PatientID");
                                Console.WriteLine($"  PatientID: {patientIDValue} (Type: {reader.GetDataTypeName(patientIDOrdinal)})");
                                
                                // PracticeLocationID
                                var practiceLocationIDOrdinal = reader.GetOrdinal("PracticeLocationID");
                                var practiceLocationIDValue = GetNullableInt32(reader, "PracticeLocationID");
                                Console.WriteLine($"  PracticeLocationID: {practiceLocationIDValue} (Type: {reader.GetDataTypeName(practiceLocationIDOrdinal)})");
                                
                                // IsPrimaryDiagnosis
                                var isPrimaryDiagnosisOrdinal = reader.GetOrdinal("IsPrimaryDiagnosis");
                                var isPrimaryDiagnosisValue = reader.IsDBNull(isPrimaryDiagnosisOrdinal) ? false : reader.GetBoolean(isPrimaryDiagnosisOrdinal);
                                Console.WriteLine($"  IsPrimaryDiagnosis: {isPrimaryDiagnosisValue} (Type: {reader.GetDataTypeName(isPrimaryDiagnosisOrdinal)})");
                                
                                var diagnosis = new PatientDiagnosis
                                {
                                    DiagnosisID = diagnosisIDValue,
                                    AppointmentID = appointmentIDValue,
                                    DiseaseName = diseaseNameValue,
                                    DiagnosisDate = diagnosisDateValue,
                                    DiagnosisBy = diagnosisByValue,
                                    Summary = summaryValue,
                                    IsLongTerm = isLongTermValue,
                                    AddtoProblem = addtoProblemValue,
                                    IsHighlighted = isHighlightedValue,
                                    SequenceNo = sequenceNoValue,
                                    IsActive = isActiveValue,
                                    IsConfidential = isConfidentialValue,
                                    DiagnosisType = diagnosisTypeValue,
                                    IsMapped = isMappedValue,
                                    PracticeID = practiceIDValue,
                                    OnSetDate = onSetDateValue,
                                    MappedBy = mappedByValue,
                                    MappedDate = mappedDateValue,
                                    IsStopped = isStoppedValue,
                                    SnomedDiseaseName = snomedDiseaseNameValue,
                                    PatientID = patientIDValue,
                                    PracticeLocationID = practiceLocationIDValue,
                                    IsPrimaryDiagnosis = isPrimaryDiagnosisValue
                                };
                                
                                response.Diagnoses.Add(diagnosis);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"❌ Error reading diagnosis data (record #{diagnosisCount}): {ex.Message}");
                                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                                // Continue processing other records
                            }
                        }
                        
                        Console.WriteLine($"✅ Retrieved {response.Diagnoses.Count} diagnosis records for patient {patientId}");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ No diagnosis information found for patient {patientId}");
                    }
                    
                    Console.WriteLine($"✅ Retrieved complete lab test data for patient {patientId}");
                    Console.WriteLine($"📊 Summary: Header={(response.Header != null ? "Yes" : "No")}, LabDetails={response.LabTestDetails.Count}, Allergies={response.Allergies.Count}, Diagnoses={response.Diagnoses.Count}");
                    return response;
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"❌ SQL Database error: {sqlEx.Message}");
                Console.WriteLine($"❌ Error Number: {sqlEx.Number}");
                Console.WriteLine($"❌ Stack trace: {sqlEx.StackTrace}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database connection failed: {ex.Message}");
                Console.WriteLine($"📋 Connection string: {_connectionString}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<List<PatientAllergy>> GetPatientAllergiesAsync(long patientId)
        {
            try
            {
                Console.WriteLine($"🔍 Getting allergies for patient ID: {patientId} using SP");
                
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Use the same SP but navigate to the third result set (allergies)
                    using var command = new SqlCommand("EXEC GetPatientLabTestData @pPatientID", connection);
                    command.Parameters.AddWithValue("@pPatientID", patientId);
                    using var reader = await command.ExecuteReaderAsync();
                    
                    var allergies = new List<PatientAllergy>();
                    
                    // Navigate to third result set (allergies)
                    // First result set: Header/Patient info - read and skip
                    Console.WriteLine("📋 Reading first result set (header)...");
                    int firstSetCount = 0;
                    while (await reader.ReadAsync()) { firstSetCount++; }
                    Console.WriteLine($"📊 First result set had {firstSetCount} rows");
                    await reader.NextResultAsync();
                    
                    // Second result set: Lab test details - read and skip
                    Console.WriteLine("📋 Reading second result set (lab details)...");
                    int secondSetCount = 0;
                    while (await reader.ReadAsync()) { secondSetCount++; }
                    Console.WriteLine($"📊 Second result set had {secondSetCount} rows");
                    await reader.NextResultAsync();
                    
                    // Third result set: Allergies
                    Console.WriteLine("📋 Reading allergies dataset from SP...");
                    Console.WriteLine($"📊 Number of columns: {reader.FieldCount}");
                    Console.WriteLine($"📋 Result set available: {!reader.IsClosed}");
                    
                                            while (await reader.ReadAsync())
                        {
                            try
                            {
                                Console.WriteLine("🔍 Reading allergy fields with detailed logging (GetPatientAllergiesAsync):");
                                
                                // AllergyID
                                var allergyIDOrdinal = reader.GetOrdinal("AllergyID");
                                var allergyIDValue = reader.IsDBNull(allergyIDOrdinal) ? 0 : reader.GetInt32(allergyIDOrdinal);
                                Console.WriteLine($"  AllergyID: {allergyIDValue} (Type: {reader.GetDataTypeName(allergyIDOrdinal)})");
                                
                                // AllergyUUID
                                var allergyUUIDOrdinal = reader.GetOrdinal("AllergyUUID");
                                var allergyUUIDValue = GetNullableString(reader, "AllergyUUID");
                                Console.WriteLine($"  AllergyUUID: {allergyUUIDValue} (Type: {reader.GetDataTypeName(allergyUUIDOrdinal)})");
                                
                                // IsReviewed
                                var isReviewedOrdinal = reader.GetOrdinal("IsReviewed");
                                var isReviewedValue = reader.IsDBNull(isReviewedOrdinal) ? false : reader.GetBoolean(isReviewedOrdinal);
                                Console.WriteLine($"  IsReviewed: {isReviewedValue} (Type: {reader.GetDataTypeName(isReviewedOrdinal)})");
                                
                                // MedTechID
                                var medTechIDOrdinal = reader.GetOrdinal("MedTechID");
                                var medTechIDValue = GetNullableInt32(reader, "MedTechID");
                                Console.WriteLine($"  MedTechID: {medTechIDValue} (Type: {reader.GetDataTypeName(medTechIDOrdinal)})");
                                
                                // OnsetDate
                                var onsetDateOrdinal = reader.GetOrdinal("OnsetDate");
                                var onsetDateValue = GetNullableDateTime(reader, "OnsetDate");
                                Console.WriteLine($"  OnsetDate: {onsetDateValue} (Type: {reader.GetDataTypeName(onsetDateOrdinal)})");
                                
                                // AllergyTypeID
                                var allergyTypeIDOrdinal = reader.GetOrdinal("AllergyTypeID");
                                var allergyTypeIDValue = GetNullableInt32(reader, "AllergyTypeID");
                                Console.WriteLine($"  AllergyTypeID: {allergyTypeIDValue} (Type: {reader.GetDataTypeName(allergyTypeIDOrdinal)})");
                                
                                // MedicineTypeID
                                var medicineTypeIDOrdinal = reader.GetOrdinal("MedicineTypeID");
                                var medicineTypeIDValue = GetNullableInt32(reader, "MedicineTypeID");
                                Console.WriteLine($"  MedicineTypeID: {medicineTypeIDValue} (Type: {reader.GetDataTypeName(medicineTypeIDOrdinal)})");
                                
                                // MedicineShortName
                                var medicineShortNameOrdinal = reader.GetOrdinal("MedicineShortName");
                                var medicineShortNameValue = GetNullableString(reader, "MedicineShortName");
                                Console.WriteLine($"  MedicineShortName: {medicineShortNameValue} (Type: {reader.GetDataTypeName(medicineShortNameOrdinal)})");
                                
                                // MedicineClassification
                                var medicineClassificationOrdinal = reader.GetOrdinal("MedicineClassification");
                                var medicineClassificationValue = GetNullableString(reader, "MedicineClassification");
                                Console.WriteLine($"  MedicineClassification: {medicineClassificationValue} (Type: {reader.GetDataTypeName(medicineClassificationOrdinal)})");
                                
                                // FavouriteSubstance
                                var favouriteSubstanceOrdinal = reader.GetOrdinal("FavouriteSubstance");
                                var favouriteSubstanceValue = GetNullableString(reader, "FavouriteSubstance");
                                Console.WriteLine($"  FavouriteSubstance: {favouriteSubstanceValue} (Type: {reader.GetDataTypeName(favouriteSubstanceOrdinal)})");
                                
                                // DiseaseName
                                var diseaseNameOrdinal = reader.GetOrdinal("DiseaseName");
                                var diseaseNameValue = GetNullableString(reader, "DiseaseName");
                                Console.WriteLine($"  DiseaseName: {diseaseNameValue} (Type: {reader.GetDataTypeName(diseaseNameOrdinal)})");
                                
                                // SubstanceTypeId
                                var substanceTypeIdOrdinal = reader.GetOrdinal("SubstanceTypeId");
                                var substanceTypeIdValue = GetNullableInt32(reader, "SubstanceTypeId");
                                Console.WriteLine($"  SubstanceTypeId: {substanceTypeIdValue} (Type: {reader.GetDataTypeName(substanceTypeIdOrdinal)})");
                                
                                // Other
                                var otherOrdinal = reader.GetOrdinal("Other");
                                var otherValue = GetNullableString(reader, "Other");
                                Console.WriteLine($"  Other: {otherValue} (Type: {reader.GetDataTypeName(otherOrdinal)})");
                                
                                // Reaction
                                var reactionOrdinal = reader.GetOrdinal("Reaction");
                                var reactionValue = reader.IsDBNull(reactionOrdinal) ? null : reader.GetString(reactionOrdinal);
                                Console.WriteLine($"  Reaction: {reactionValue} (Type: {reader.GetDataTypeName(reactionOrdinal)})");
                                
                                // IsActive
                                var isActiveOrdinal = reader.GetOrdinal("IsActive");
                                var isActiveValue = reader.IsDBNull(isActiveOrdinal) ? false : reader.GetBoolean(isActiveOrdinal);
                                Console.WriteLine($"  IsActive: {isActiveValue} (Type: {reader.GetDataTypeName(isActiveOrdinal)})");
                                
                                // FullName
                                var fullNameOrdinal = reader.GetOrdinal("FullName");
                                var fullNameValue = GetNullableString(reader, "FullName");
                                Console.WriteLine($"  FullName: {fullNameValue} (Type: {reader.GetDataTypeName(fullNameOrdinal)})");
                                
                                // Comment
                                var commentOrdinal = reader.GetOrdinal("Comment");
                                var commentValue = GetNullableString(reader, "Comment");
                                Console.WriteLine($"  Comment: {commentValue} (Type: {reader.GetDataTypeName(commentOrdinal)})");
                                
                                // IsHighlight
                                var isHighlightOrdinal = reader.GetOrdinal("IsHighlight");
                                var isHighlightValue = reader.IsDBNull(isHighlightOrdinal) ? false : reader.GetBoolean(isHighlightOrdinal);
                                Console.WriteLine($"  IsHighlight: {isHighlightValue} (Type: {reader.GetDataTypeName(isHighlightOrdinal)})");
                                
                                // InsertedAt
                                var insertedAtOrdinal = reader.GetOrdinal("InsertedAt");
                                var insertedAtValue = GetNullableDateTime(reader, "InsertedAt");
                                Console.WriteLine($"  InsertedAt: {insertedAtValue} (Type: {reader.GetDataTypeName(insertedAtOrdinal)})");
                                
                                // AllergyType
                                var allergyTypeOrdinal = reader.GetOrdinal("AllergyType");
                                var allergyTypeValue = reader.IsDBNull(allergyTypeOrdinal) ? null : reader.GetString(allergyTypeOrdinal);
                                Console.WriteLine($"  AllergyType: {allergyTypeValue} (Type: {reader.GetDataTypeName(allergyTypeOrdinal)})");
                                
                                // Name
                                var nameOrdinal = reader.GetOrdinal("Name");
                                var nameValue = GetNullableString(reader, "Name");
                                Console.WriteLine($"  Name: {nameValue} (Type: {reader.GetDataTypeName(nameOrdinal)})");
                                
                                // IsNKA
                                var isNKAOrdinal = reader.GetOrdinal("IsNKA");
                                var isNKAValue = reader.IsDBNull(isNKAOrdinal) ? false : reader.GetBoolean(isNKAOrdinal);
                                Console.WriteLine($"  IsNKA: {isNKAValue} (Type: {reader.GetDataTypeName(isNKAOrdinal)})");
                                
                                // SequenceNo
                                var sequenceNoOrdinal = reader.GetOrdinal("SequenceNo");
                                var sequenceNoValue = GetNullableInt32(reader, "SequenceNo");
                                Console.WriteLine($"  SequenceNo: {sequenceNoValue} (Type: {reader.GetDataTypeName(sequenceNoOrdinal)})");
                                
                                // Severity
                                var severityOrdinal = reader.GetOrdinal("Severity");
                                var severityValue = GetNullableString(reader, "Severity");
                                Console.WriteLine($"  Severity: {severityValue} (Type: {reader.GetDataTypeName(severityOrdinal)})");
                                
                                var allergy = new PatientAllergy
                                {
                                    AllergyID = allergyIDValue,
                                    AllergyUUID = GetNullableString(reader, "AllergyUUID"),
                                    IsReviewed = reader.IsDBNull(reader.GetOrdinal("IsReviewed")) ? false : reader.GetBoolean(reader.GetOrdinal("IsReviewed")),
                                    MedTechID = GetNullableInt32(reader, "MedTechID"),
                                    OnsetDate = onsetDateValue,
                                    AllergyTypeID = GetNullableInt32(reader, "AllergyTypeID"),
                                    MedicineTypeID = medicineTypeIDValue,
                                    MedicineShortName = GetNullableString(reader, "MedicineShortName"),
                                    MedicineClassification = GetNullableString(reader, "MedicineClassification"),
                                    FavouriteSubstance = GetNullableString(reader, "FavouriteSubstance"),
                                    DiseaseName = GetNullableString(reader, "DiseaseName"),
                                    SubstanceTypeId = GetNullableInt32(reader, "SubstanceTypeId"),
                                    Other = GetNullableString(reader, "Other"),
                                    Reaction = reactionValue,
                                    IsActive = isActiveValue,
                                    FullName = GetNullableString(reader, "FullName"),
                                    Comment = commentValue,
                                    IsHighlight = isHighlightValue,
                                    InsertedAt = GetNullableDateTime(reader, "InsertedAt"),
                                    AllergyType = allergyTypeValue,
                                    Name = GetNullableString(reader, "Name"),
                                    IsNKA = reader.IsDBNull(reader.GetOrdinal("IsNKA")) ? false : reader.GetBoolean(reader.GetOrdinal("IsNKA")),
                                    SequenceNo = GetNullableInt32(reader, "SequenceNo"),
                                    Severity = GetNullableString(reader, "Severity")
                                };
                                
                                allergies.Add(allergy);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"❌ Error reading allergy data: {ex.Message}");
                                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                            }
                        }
                    
                    Console.WriteLine($"✅ Retrieved {allergies.Count} allergy records for patient {patientId}");
                    return allergies;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting allergies: {ex.Message}");
                return new List<PatientAllergy>();
            }
        }

        public async Task<List<PatientDiagnosis>> GetPatientDiagnosesAsync(long patientId)
        {
            try
            {
                Console.WriteLine($"🔍 Getting diagnoses for patient ID: {patientId} using SP");
                
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Use the same SP but navigate to the fourth result set (diagnoses)
                    using var command = new SqlCommand("EXEC GetPatientLabTestData @pPatientID", connection);
                    command.Parameters.AddWithValue("@pPatientID", patientId);
                    using var reader = await command.ExecuteReaderAsync();
                    
                    var diagnoses = new List<PatientDiagnosis>();
                    
                    // Navigate to fourth result set (diagnoses)
                    // First result set: Header/Patient info - read and skip
                    Console.WriteLine("📋 Reading first result set (header)...");
                    int firstSetCount = 0;
                    while (await reader.ReadAsync()) { firstSetCount++; }
                    Console.WriteLine($"📊 First result set had {firstSetCount} rows");
                    await reader.NextResultAsync();
                    
                    // Second result set: Lab test details - read and skip
                    Console.WriteLine("📋 Reading second result set (lab details)...");
                    int secondSetCount = 0;
                    while (await reader.ReadAsync()) { secondSetCount++; }
                    Console.WriteLine($"📊 Second result set had {secondSetCount} rows");
                    await reader.NextResultAsync();
                    
                    // Third result set: Allergies - read and skip
                    Console.WriteLine("📋 Reading third result set (allergies)...");
                    int thirdSetCount = 0;
                    while (await reader.ReadAsync()) { thirdSetCount++; }
                    Console.WriteLine($"📊 Third result set had {thirdSetCount} rows");
                    await reader.NextResultAsync();
                    
                    // Fourth result set: Diagnoses
                    Console.WriteLine("📋 Reading diagnoses dataset from SP...");
                    Console.WriteLine($"📊 Number of columns: {reader.FieldCount}");
                    Console.WriteLine($"📋 Result set available: {!reader.IsClosed}");
                    
                                            while (await reader.ReadAsync())
                        {
                            try
                            {
                                Console.WriteLine("🔍 Reading diagnosis fields with detailed logging (GetPatientDiagnosesAsync):");
                                
                                // DiagnosisID
                                var diagnosisIDOrdinal = reader.GetOrdinal("DiagnosisID");
                                var diagnosisIDValue = reader.IsDBNull(diagnosisIDOrdinal) ? 0 : reader.GetInt32(diagnosisIDOrdinal);
                                Console.WriteLine($"  DiagnosisID: {diagnosisIDValue} (Type: {reader.GetDataTypeName(diagnosisIDOrdinal)})");
                                
                                // AppointmentID
                                var appointmentIDOrdinal = reader.GetOrdinal("AppointmentID");
                                var appointmentIDValue = GetNullableInt32(reader, "AppointmentID");
                                Console.WriteLine($"  AppointmentID: {appointmentIDValue} (Type: {reader.GetDataTypeName(appointmentIDOrdinal)})");
                                
                                // DiseaseName
                                var diseaseNameOrdinal = reader.GetOrdinal("DiseaseName");
                                var diseaseNameValue = reader.IsDBNull(diseaseNameOrdinal) ? null : reader.GetString(diseaseNameOrdinal);
                                Console.WriteLine($"  DiseaseName: {diseaseNameValue} (Type: {reader.GetDataTypeName(diseaseNameOrdinal)})");
                                
                                // DiagnosisDate
                                var diagnosisDateOrdinal = reader.GetOrdinal("DiagnosisDate");
                                var diagnosisDateValue = GetNullableDateTime(reader, "DiagnosisDate");
                                Console.WriteLine($"  DiagnosisDate: {diagnosisDateValue} (Type: {reader.GetDataTypeName(diagnosisDateOrdinal)})");
                                
                                // DiagnosisBy
                                var diagnosisByOrdinal = reader.GetOrdinal("DiagnosisBy");
                                var diagnosisByValue = reader.IsDBNull(diagnosisByOrdinal) ? null : reader.GetInt32(diagnosisByOrdinal).ToString();
                                Console.WriteLine($"  DiagnosisBy: {diagnosisByValue} (Type: {reader.GetDataTypeName(diagnosisByOrdinal)})");
                                
                                // Summary
                                var summaryOrdinal = reader.GetOrdinal("Summary");
                                var summaryValue = reader.IsDBNull(summaryOrdinal) ? null : reader.GetString(summaryOrdinal);
                                Console.WriteLine($"  Summary: {summaryValue} (Type: {reader.GetDataTypeName(summaryOrdinal)})");
                                
                                // IsLongTerm
                                var isLongTermOrdinal = reader.GetOrdinal("IsLongTerm");
                                var isLongTermValue = reader.IsDBNull(isLongTermOrdinal) ? false : reader.GetBoolean(isLongTermOrdinal);
                                Console.WriteLine($"  IsLongTerm: {isLongTermValue} (Type: {reader.GetDataTypeName(isLongTermOrdinal)})");
                                
                                // AddtoProblem
                                var addtoProblemOrdinal = reader.GetOrdinal("AddtoProblem");
                                var addtoProblemValue = reader.IsDBNull(addtoProblemOrdinal) ? false : reader.GetBoolean(addtoProblemOrdinal);
                                Console.WriteLine($"  AddtoProblem: {addtoProblemValue} (Type: {reader.GetDataTypeName(addtoProblemOrdinal)})");
                                
                                // IsHighlighted
                                var isHighlightedOrdinal = reader.GetOrdinal("IsHighlighted");
                                var isHighlightedValue = reader.IsDBNull(isHighlightedOrdinal) ? false : reader.GetBoolean(isHighlightedOrdinal);
                                Console.WriteLine($"  IsHighlighted: {isHighlightedValue} (Type: {reader.GetDataTypeName(isHighlightedOrdinal)})");
                                
                                // SequenceNo
                                var sequenceNoOrdinal = reader.GetOrdinal("SequenceNo");
                                var sequenceNoValue = GetNullableInt32(reader, "SequenceNo");
                                Console.WriteLine($"  SequenceNo: {sequenceNoValue} (Type: {reader.GetDataTypeName(sequenceNoOrdinal)})");
                                
                                // IsActive
                                var isActiveOrdinal = reader.GetOrdinal("IsActive");
                                var isActiveValue = reader.IsDBNull(isActiveOrdinal) ? false : reader.GetBoolean(isActiveOrdinal);
                                Console.WriteLine($"  IsActive: {isActiveValue} (Type: {reader.GetDataTypeName(isActiveOrdinal)})");
                                
                                // IsConfidential
                                var isConfidentialOrdinal = reader.GetOrdinal("IsConfidential");
                                var isConfidentialValue = reader.IsDBNull(isConfidentialOrdinal) ? false : reader.GetBoolean(isConfidentialOrdinal);
                                Console.WriteLine($"  IsConfidential: {isConfidentialValue} (Type: {reader.GetDataTypeName(isConfidentialOrdinal)})");
                                
                                // DiagnosisType
                                var diagnosisTypeOrdinal = reader.GetOrdinal("DiagnosisType");
                                var diagnosisTypeValue = reader.IsDBNull(diagnosisTypeOrdinal) ? null : reader.GetString(diagnosisTypeOrdinal);
                                Console.WriteLine($"  DiagnosisType: {diagnosisTypeValue} (Type: {reader.GetDataTypeName(diagnosisTypeOrdinal)})");
                                
                                // IsMapped
                                var isMappedOrdinal = reader.GetOrdinal("IsMapped");
                                var isMappedValue = reader.IsDBNull(isMappedOrdinal) ? false : reader.GetBoolean(isMappedOrdinal);
                                Console.WriteLine($"  IsMapped: {isMappedValue} (Type: {reader.GetDataTypeName(isMappedOrdinal)})");
                                
                                // PracticeID
                                var practiceIDOrdinal = reader.GetOrdinal("PracticeID");
                                var practiceIDValue = GetNullableInt32(reader, "PracticeID");
                                Console.WriteLine($"  PracticeID: {practiceIDValue} (Type: {reader.GetDataTypeName(practiceIDOrdinal)})");
                                
                                // OnSetDate
                                var onSetDateOrdinal = reader.GetOrdinal("OnSetDate");
                                var onSetDateValue = GetNullableDateTime(reader, "OnSetDate");
                                Console.WriteLine($"  OnSetDate: {onSetDateValue} (Type: {reader.GetDataTypeName(onSetDateOrdinal)})");
                                
                                // MappedBy
                                var mappedByOrdinal = reader.GetOrdinal("MappedBy");
                                var mappedByValue = GetNullableInt32(reader, "MappedBy")?.ToString();
                                Console.WriteLine($"  MappedBy: {mappedByValue} (Type: {reader.GetDataTypeName(mappedByOrdinal)})");
                                
                                // MappedDate
                                var mappedDateOrdinal = reader.GetOrdinal("MappedDate");
                                var mappedDateValue = GetNullableDateTime(reader, "MappedDate");
                                Console.WriteLine($"  MappedDate: {mappedDateValue} (Type: {reader.GetDataTypeName(mappedDateOrdinal)})");
                                
                                // IsStopped
                                var isStoppedOrdinal = reader.GetOrdinal("IsStopped");
                                var isStoppedValue = reader.IsDBNull(isStoppedOrdinal) ? false : reader.GetBoolean(isStoppedOrdinal);
                                Console.WriteLine($"  IsStopped: {isStoppedValue} (Type: {reader.GetDataTypeName(isStoppedOrdinal)})");
                                
                                // SnomedDiseaseName
                                var snomedDiseaseNameOrdinal = reader.GetOrdinal("SnomedDiseaseName");
                                var snomedDiseaseNameValue = GetNullableString(reader, "SnomedDiseaseName");
                                Console.WriteLine($"  SnomedDiseaseName: {snomedDiseaseNameValue} (Type: {reader.GetDataTypeName(snomedDiseaseNameOrdinal)})");
                                
                                // PatientID
                                var patientIDOrdinal = reader.GetOrdinal("PatientID");
                                var patientIDValue = GetNullableInt32(reader, "PatientID");
                                Console.WriteLine($"  PatientID: {patientIDValue} (Type: {reader.GetDataTypeName(patientIDOrdinal)})");
                                
                                // PracticeLocationID
                                var practiceLocationIDOrdinal = reader.GetOrdinal("PracticeLocationID");
                                var practiceLocationIDValue = GetNullableInt32(reader, "PracticeLocationID");
                                Console.WriteLine($"  PracticeLocationID: {practiceLocationIDValue} (Type: {reader.GetDataTypeName(practiceLocationIDOrdinal)})");
                                
                                // IsPrimaryDiagnosis
                                var isPrimaryDiagnosisOrdinal = reader.GetOrdinal("IsPrimaryDiagnosis");
                                var isPrimaryDiagnosisValue = reader.IsDBNull(isPrimaryDiagnosisOrdinal) ? false : reader.GetBoolean(isPrimaryDiagnosisOrdinal);
                                Console.WriteLine($"  IsPrimaryDiagnosis: {isPrimaryDiagnosisValue} (Type: {reader.GetDataTypeName(isPrimaryDiagnosisOrdinal)})");
                                
                                var diagnosis = new PatientDiagnosis
                                {
                                    DiagnosisID = diagnosisIDValue,
                                    AppointmentID = appointmentIDValue,
                                    DiseaseName = diseaseNameValue,
                                    DiagnosisDate = diagnosisDateValue,
                                    DiagnosisBy = diagnosisByValue,
                                    Summary = summaryValue,
                                    IsLongTerm = isLongTermValue,
                                    AddtoProblem = addtoProblemValue,
                                    IsHighlighted = isHighlightedValue,
                                    SequenceNo = sequenceNoValue,
                                    IsActive = isActiveValue,
                                    IsConfidential = isConfidentialValue,
                                    DiagnosisType = diagnosisTypeValue,
                                    IsMapped = isMappedValue,
                                    PracticeID = practiceIDValue,
                                    OnSetDate = onSetDateValue,
                                    MappedBy = mappedByValue,
                                    MappedDate = mappedDateValue,
                                    IsStopped = isStoppedValue,
                                    SnomedDiseaseName = snomedDiseaseNameValue,
                                    PatientID = patientIDValue,
                                    PracticeLocationID = practiceLocationIDValue,
                                    IsPrimaryDiagnosis = isPrimaryDiagnosisValue
                                };
                                
                                diagnoses.Add(diagnosis);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"❌ Error reading diagnosis data: {ex.Message}");
                                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                            }
                        }
                    
                                    Console.WriteLine($"✅ Retrieved {diagnoses.Count} diagnosis records for patient {patientId}");
                return diagnoses;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error getting diagnoses: {ex.Message}");
            return new List<PatientDiagnosis>();
        }
    }

    public async Task<List<PatientLabObservation>> GetPatientLabObservationsAsync(int patientId, string? observationText = null, int? practiceId = null)
    {
        var observations = new List<PatientLabObservation>();
        
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                Console.WriteLine($"🔍 Getting lab observations for patient {patientId}...");
                
                using (var command = new SqlCommand("[dbo].[Usp_GetPatientGroupLabData_Priority]", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PatientID", patientId);
                    command.Parameters.AddWithValue("@ObservationText", observationText ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PracticeID", practiceId ?? (object)DBNull.Value);
                    
                    Console.WriteLine($"📊 Executing stored procedure with parameters:");
                    Console.WriteLine($"  PatientID: {patientId}");
                    Console.WriteLine($"  ObservationText: {observationText ?? "NULL"}");
                    Console.WriteLine($"  PracticeID: {practiceId?.ToString() ?? "NULL"}");
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            try
                            {
                                var observation = new PatientLabObservation
                                {
                                    PatientID = reader.GetInt32(reader.GetOrdinal("PatientID")),
                                    MessageSubject = GetNullableString(reader, "MessageSubject"),
                                    ResultName = GetNullableString(reader, "ResultName"),
                                    ObservationCodingSystem = GetNullableString(reader, "ObservationCodingSystem"),
                                    ObservationDateTime = GetNullableDateTime(reader, "ObservationDateTime"),
                                    ObservationValue = GetNullableString(reader, "ObservationValue"),
                                    Units = GetNullableString(reader, "Units"),
                                    ReferenceRanges = GetNullableString(reader, "ReferenceRanges"),
                                    AbnormalFlagID = GetNullableInt32(reader, "AbnormalFlagID"),
                                    AbnormalFlagDesc = GetNullableString(reader, "AbnormalFlagDesc"),
                                    LabTestNTEID = reader.IsDBNull(reader.GetOrdinal("LabTestNTEID")) ? null : (long?)reader.GetInt32(reader.GetOrdinal("LabTestNTEID")),
                                    Source = GetNullableString(reader, "Source"),
                                    Comments = GetNullableString(reader, "Comments")
                                };
                                
                                observations.Add(observation);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"❌ Error reading observation data: {ex.Message}");
                                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                            }
                        }
                    }
                }
                
                Console.WriteLine($"✅ Retrieved {observations.Count} lab observation records for patient {patientId}");
                return observations;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error getting lab observations: {ex.Message}");
            return new List<PatientLabObservation>();
        }
    }
}
}