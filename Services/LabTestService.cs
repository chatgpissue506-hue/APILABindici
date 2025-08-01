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
                    
                    // Test if stored procedure exists
                    using (var testCommand = new SqlCommand("SELECT COUNT(*) FROM sys.procedures WHERE name = 'GetPatientLabTestData'", connection))
                    {
                        var procedureExists = (int)await testCommand.ExecuteScalarAsync();
                        if (procedureExists > 0)
                        {
                            Console.WriteLine("✅ Stored procedure 'GetPatientLabTestData' found!");
                        }
                        else
                        {
                            Console.WriteLine("❌ Stored procedure 'GetPatientLabTestData' not found!");
                            return new List<LabTestData>();
                        }
                    }
                    
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
                Console.WriteLine("3. Ensure the database 'PMS_NZ_Local_NZTFS' exists");
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
                        PatientID = reader.IsDBNull(reader.GetOrdinal("PatientID")) ? null : reader.GetString(reader.GetOrdinal("PatientID")),
                        PracticeID = reader.IsDBNull(reader.GetOrdinal("PracticeID")) ? null : reader.GetString(reader.GetOrdinal("PracticeID")),
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
                    
                    // Test if stored procedure exists
                    using (var testCommand = new SqlCommand("SELECT COUNT(*) FROM sys.procedures WHERE name = 'GetPatientnameforLAB'", connection))
                    {
                        var procedureExists = (int)await testCommand.ExecuteScalarAsync();
                        if (procedureExists > 0)
                        {
                            Console.WriteLine("✅ Stored procedure 'GetPatientnameforLAB' found!");
                        }
                        else
                        {
                            Console.WriteLine("❌ Stored procedure 'GetPatientnameforLAB' not found!");
                            return null;
                        }
                    }
                    
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

        public async Task<PatientLabTestResponse?> GetPatientLabTestDataUpdatedAsync(long patientId)
        {
            try
            {
                Console.WriteLine($"🔍 Getting updated lab test data for patient ID: {patientId}");
                Console.WriteLine($"📋 Connection String: {_connectionString}");
                
                using (var connection = new SqlConnection(_connectionString))
                {
                    Console.WriteLine("⏳ Opening connection...");
                    await connection.OpenAsync();
                    Console.WriteLine("✅ Successfully connected to database!");
                    
                    // Test if stored procedure exists
                    using (var testCommand = new SqlCommand("SELECT COUNT(*) FROM sys.procedures WHERE name = 'GetPatientLabTestData'", connection))
                    {
                        var procedureExists = (int)await testCommand.ExecuteScalarAsync();
                        if (procedureExists > 0)
                        {
                            Console.WriteLine("✅ Stored procedure 'GetPatientLabTestData' found!");
                        }
                        else
                        {
                            Console.WriteLine("❌ Stored procedure 'GetPatientLabTestData' not found!");
                            return null;
                        }
                    }
                    
                    Console.WriteLine("⏳ Executing GetPatientLabTestData stored procedure...");
                    using var command = new SqlCommand("EXEC GetPatientLabTestData @pPatientID", connection);
                    command.Parameters.AddWithValue("@pPatientID", patientId);
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
                            var header = new PatientLabTestHeader
                            {
                                NHINumber = reader.IsDBNull(reader.GetOrdinal("NHINumber")) ? null : reader.GetString(reader.GetOrdinal("NHINumber")),
                                FullName = reader.IsDBNull(reader.GetOrdinal("FullName")) ? null : reader.GetString(reader.GetOrdinal("FullName")),
                                DOB = reader.IsDBNull(reader.GetOrdinal("DOB")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("DOB")),
                                GenderName = reader.IsDBNull(reader.GetOrdinal("GenderName")) ? null : reader.GetString(reader.GetOrdinal("GenderName")),
                                PatientID = reader.IsDBNull(reader.GetOrdinal("PatientID")) ? null : reader.GetString(reader.GetOrdinal("PatientID")),
                                PracticeID = reader.IsDBNull(reader.GetOrdinal("PracticeID")) ? null : reader.GetString(reader.GetOrdinal("PracticeID")),
                                MshInsertedAt = reader.IsDBNull(reader.GetOrdinal("MshInsertedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("MshInsertedAt")),
                                Ethnicity = reader.IsDBNull(reader.GetOrdinal("Ethnicity")) ? null : reader.GetString(reader.GetOrdinal("Ethnicity")),
                                Age = reader.IsDBNull(reader.GetOrdinal("Age")) ? null : int.TryParse(reader.GetString(reader.GetOrdinal("Age")), out int age) ? age : null
                            };
                            
                            response.Header = header;
                            Console.WriteLine($"✅ Retrieved header information for patient {patientId}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Error reading header data: {ex.Message}");
                        }
                    }
                    
                    // Move to next result set (second dataset)
                    if (await reader.NextResultAsync())
                    {
                        Console.WriteLine("📋 Reading second dataset (Detail information)...");
                        Console.WriteLine($"📊 Number of columns: {reader.FieldCount}");
                        
                        // Print column information for second dataset
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.WriteLine($"  Column {i}: {reader.GetName(i)} ({reader.GetDataTypeName(i)})");
                        }
                        
                        while (await reader.ReadAsync())
                        {
                            try
                            {
                                var detail = new PatientLabTestDetail
                                {
                                    LabTestOBRID = reader.IsDBNull(reader.GetOrdinal("LabTestOBRID")) ? 0 : reader.GetInt32(reader.GetOrdinal("LabTestOBRID")),
                                    SnomedCode = reader.IsDBNull(reader.GetOrdinal("SnomedCode")) ? null : reader.GetString(reader.GetOrdinal("SnomedCode")),
                                    MessageSubject = reader.IsDBNull(reader.GetOrdinal("MessageSubject")) ? null : reader.GetString(reader.GetOrdinal("MessageSubject")),
                                    ObservationDateTime = reader.IsDBNull(reader.GetOrdinal("ObservationDateTime")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("ObservationDateTime")),
                                    StatusChangeDateTime = reader.IsDBNull(reader.GetOrdinal("StatusChangeDateTime")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("StatusChangeDateTime")),
                                    AppointmentID = reader.IsDBNull(reader.GetOrdinal("AppointmentID")) ? null : reader.GetString(reader.GetOrdinal("AppointmentID")),
                                    LabTestOBXID = reader.IsDBNull(reader.GetOrdinal("LabTestOBXID")) ? 0 : reader.GetInt64(reader.GetOrdinal("LabTestOBXID")),
                                    SnomedCode_2 = reader.IsDBNull(reader.GetOrdinal("SnomedCode_2")) ? null : reader.GetString(reader.GetOrdinal("SnomedCode_2")),
                                    ResultName = reader.IsDBNull(reader.GetOrdinal("ResultName")) ? null : reader.GetString(reader.GetOrdinal("ResultName")),
                                    ObservationCodingSystem = reader.IsDBNull(reader.GetOrdinal("ObservationCodingSystem")) ? null : reader.GetString(reader.GetOrdinal("ObservationCodingSystem")),
                                    ObservationValue = reader.IsDBNull(reader.GetOrdinal("ObservationValue")) ? null : reader.GetString(reader.GetOrdinal("ObservationValue")),
                                    Units = reader.IsDBNull(reader.GetOrdinal("Units")) ? null : reader.GetString(reader.GetOrdinal("Units")),
                                    ReferenceRanges = reader.IsDBNull(reader.GetOrdinal("ReferenceRanges")) ? null : reader.GetString(reader.GetOrdinal("ReferenceRanges")),
                                    AbnormalFlagID = reader.IsDBNull(reader.GetOrdinal("AbnormalFlagID")) ? 0 : reader.GetInt32(reader.GetOrdinal("AbnormalFlagID")),
                                    AbnormalFlagDesc = reader.IsDBNull(reader.GetOrdinal("AbnormalFlagDesc")) ? null : reader.GetString(reader.GetOrdinal("AbnormalFlagDesc")),
                                    LabTestNTEID = reader.IsDBNull(reader.GetOrdinal("LabTestNTEID")) ? 0 : reader.GetInt32(reader.GetOrdinal("LabTestNTEID")),
                                    Source = reader.IsDBNull(reader.GetOrdinal("Source")) ? null : reader.GetString(reader.GetOrdinal("Source")),
                                    Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : reader.GetString(reader.GetOrdinal("Comments"))
                                };
                                
                                response.Details.Add(detail);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"❌ Error reading detail data: {ex.Message}");
                            }
                        }
                        
                        Console.WriteLine($"✅ Retrieved {response.Details.Count} detail records for patient {patientId}");
                    }
                    
                    Console.WriteLine($"✅ Retrieved complete lab test data for patient {patientId}");
                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database connection failed: {ex.Message}");
                Console.WriteLine($"📋 Connection string: {_connectionString}");
                return null;
            }
        }
    }
}