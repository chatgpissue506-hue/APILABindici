# Lab Test API

A .NET Core Web API for retrieving lab test data from a SQL Server database using stored procedures.

## Recent Updates (Latest)

### Updated Stored Procedure Support

The API has been updated to support the enhanced `GetPatientLabTestData` stored procedure with the following improvements:

#### New Features Added:

1. **Enhanced Parameter Support**
   - Added optional `labTestMshID` parameter to filter by specific lab test
   - Both parameters are now supported: `@pPatientID` and `@pLabTestMshID`

2. **New Data Fields**
   - **Header Information**: Added `Ethnicity` and `Age` fields
   - **Lab Test Details**: Added `AbnormalFlagDesc` and `PriorityID` fields
   - **Allergies**: Completely restructured with new fields including:
     - `AllergyUUID`, `IsReviewed`, `MedTechID`
     - `MedicineShortName`, `MedicineClassification`, `FavouriteSubstance`
     - `DiseaseName`, `SubstanceTypeId`, `Other`
     - `FullName`, `InsertedAt`, `AllergyType`, `Name`
     - `IsNKA`, `SequenceNo`, `Severity`

3. **Enhanced Error Handling**
   - Comprehensive try-catch blocks for each dataset
   - Detailed logging for debugging
   - Graceful handling of missing data
   - SQL-specific exception handling
   - Command timeout protection (5 minutes)

4. **Improved Data Processing**
   - Record-level error handling (continues processing other records)
   - Detailed logging for each field and data type
   - Summary reporting of retrieved data counts

#### API Endpoints Updated:

- `GET /api/labtest/patientinboxdetail/{patientId}?labTestMshID={optional}`
- `GET /api/labtest/patient-labtest-updated/{patientId}?labTestMshID={optional}`

#### Model Changes:

**PatientAllergy.cs** - Completely restructured with new fields:
```csharp
public class PatientAllergy
{
    public int AllergyID { get; set; }
    public string? AllergyUUID { get; set; }
    public bool IsReviewed { get; set; }
    public int? MedTechID { get; set; }
    public DateTime? OnsetDate { get; set; }
    public int? AllergyTypeID { get; set; }
    public int? MedicineTypeID { get; set; }
    public string? MedicineShortName { get; set; }
    public string? MedicineClassification { get; set; }
    public string? FavouriteSubstance { get; set; }
    public string? DiseaseName { get; set; }
    public int? SubstanceTypeId { get; set; }
    public string? Other { get; set; }
    public string? Reaction { get; set; }
    public bool IsActive { get; set; }
    public string? FullName { get; set; }
    public string? Comment { get; set; }
    public bool IsHighlight { get; set; }
    public DateTime? InsertedAt { get; set; }
    public string? AllergyType { get; set; }
    public string? Name { get; set; }
    public bool IsNKA { get; set; }
    public int? SequenceNo { get; set; }
    public string? Severity { get; set; }
}
```

## Features

- **Database Integration**: Connects to SQL Server database using stored procedures
- **Structured Data**: Returns organized patient lab test data with header, details, allergies, and diagnoses
- **Error Handling**: Comprehensive exception handling with detailed logging
- **Flexible Filtering**: Support for patient ID and lab test MSH ID filtering
- **CORS Support**: Configured for cross-origin requests
- **Async Operations**: All database operations are asynchronous

## Configuration

### Connection String
Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=your-server;Initial Catalog=your-database;User ID=your-user;Password=your-password;TrustServerCertificate=True;Connection Timeout=120;"
  }
}
```

### CORS Settings
Configure CORS origins in `appsettings.json`:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:4200",
      "https://yourdomain.com"
    ]
  }
}
```

## API Endpoints

### Get All Lab Test Data
```
GET /api/labtest
```

### Get Lab Test Data by Patient
```
GET /api/labtest/GetPatientIndividualResults/{patientId}
```

### Get Lab Test Data by Date Range
```
GET /api/labtest/daterange?startDate={date}&endDate={date}
```

### Get Lab Test Data with Filters
```
GET /api/labtest/filter?patientId={id}&startDate={date}&endDate={date}&practiceId={id}
```

### Get Patient Lab Test Data (Updated)
```
GET /api/labtest/patientinboxdetail/{patientId}?labTestMshID={optional}
GET /api/labtest/patient-labtest-updated/{patientId}?labTestMshID={optional}
```

### Get Patient Information
```
GET /api/labtest/patient-info/{patientId}
```

### Get Patient Allergies
```
GET /api/labtest/patient-allergies/{patientId}
```

### Get Patient Diagnoses
```
GET /api/labtest/patient-diagnoses/{patientId}
```

### Get Patient Lab Observations
```
GET /api/labtest/patient-observations/{patientId}?observationText={optional}&practiceId={optional}
```

### Get Patient Lab Observation History by Name
```
GET /api/labtest/patient-observation-history/{patientId}?startDate={optional}&endDate={optional}&panelTypeFilter={optional}
```

**Parameters:**
- `patientId` (int): Patient ID (required)
- `startDate` (DateTime, optional): Start date for filtering results
- `endDate` (DateTime, optional): End date for filtering results  
- `panelTypeFilter` (string, optional): Panel type filter (e.g., 'CBC', 'Hemoglobin')

**Examples:**
```
# Get all lab observation history for a patient
GET /api/labtest/patient-observation-history/2450776

# Get CBC lab results for a patient
GET /api/labtest/patient-observation-history/2450776?panelTypeFilter=CBC

# Get Hemoglobin panel results between dates
GET /api/labtest/patient-observation-history/2450776?startDate=2024-01-01&endDate=2024-12-31

# Get Hemoglobin panel results between dates with panel filter
GET /api/labtest/patient-observation-history/2450776?startDate=2024-01-01&endDate=2024-12-31&panelTypeFilter=Hemoglobin
```

**Response Structure:**
```json
[
  {
    "labTestOBRID": "number",
    "snomedCode": "string",
    "messageSubject": "string",
    "panelType": "string",
    "observationDateTime": "date",
    "statusChangeDateTime": "date",
    "appointmentID": "number",
    "labTestOBXID": "number",
    "snomedCode_2": "string",
    "resultName": "string",
    "observationCodingSystem": "string",
    "observationValue": "string",
    "units": "string",
    "referenceRanges": "string",
    "abnormalFlagID": "number",
    "abnormalFlagDesc": "string",
    "labTestNTEID": "number",
    "source": "string",
    "comments": "string",
    "priorityID": "number",
    "providerFullName": "string",
    "patientFullAddress": "string"
  }
]
```

### Get Patient Medication Details
```
GET /api/labtest/patient-medications/{patientId}?practiceId={optional}&practiceLocationId={optional}&pageNo={optional}&pageSize={optional}
```

**Parameters:**
- `patientId` (int): Patient ID (required)
- `practiceId` (int, optional): Practice ID (default: 127)
- `practiceLocationId` (int, optional): Practice Location ID (default: 4)
- `pageNo` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Page size (default: 20)

**Examples:**
```
# Get all medication details for a patient
GET /api/labtest/patient-medications/1771935

# Get medication details with custom practice settings
GET /api/labtest/patient-medications/1771935?practiceId=127&practiceLocationId=4

# Get medication details with pagination
GET /api/labtest/patient-medications/1771935?pageNo=1&pageSize=10

# Get medication details with all parameters
GET /api/labtest/patient-medications/1771935?practiceId=127&practiceLocationId=4&pageNo=1&pageSize=20
```

**Response Structure:**
```json
[
  {
    "patientID": "number",
    "medicationID": "number",
    "lastRXDate": "date",
    "startDate": "date",
    "providerName": "string",
    "medicineName": "string",
    "take": "string",
    "frequencyID": "number",
    "routeID": "number",
    "quantity": "number",
    "duration": "number",
    "durationType": "string",
    "directions": "string",
    "medicationCategory": "string"
  }
]
```

**Medication Categories:**
- **CM**: Current Medications - Active medications prescribed today
- **SM**: Short Term Medications - Medications prescribed within the last 91 days (not long-term)
- **LM**: Long Term Medications - Medications marked as long-term and not stopped

## Response Structure

The main endpoint returns a structured response with four sections:

```json
{
  "header": {
    "nhiNumber": "string",
    "fullName": "string",
    "dob": "date",
    "genderName": "string",
    "patientID": "string",
    "practiceID": "string",
    "mshInsertedAt": "date",
    "ethnicity": "string",
    "age": "number"
  },
  "labTestDetails": [
    {
      "labTestOBRID": "number",
      "snomedCode": "string",
      "messageSubject": "string",
      "observationDateTime": "date",
      "statusChangeDateTime": "date",
      "appointmentID": "string",
      "labTestOBXID": "number",
      "snomedCode_2": "string",
      "resultName": "string",
      "observationCodingSystem": "string",
      "observationValue": "string",
      "units": "string",
      "referenceRanges": "string",
      "abnormalFlagID": "number",
      "abnormalFlagDesc": "string",
      "labTestNTEID": "number",
      "source": "string",
      "comments": "string",
      "priorityID": "number"
    }
  ],
  "allergies": [
    {
      "allergyID": "number",
      "allergyUUID": "string",
      "isReviewed": "boolean",
      "medTechID": "number",
      "onsetDate": "date",
      "allergyTypeID": "number",
      "medicineTypeID": "number",
      "medicineShortName": "string",
      "medicineClassification": "string",
      "favouriteSubstance": "string",
      "diseaseName": "string",
      "substanceTypeId": "number",
      "other": "string",
      "reaction": "string",
      "isActive": "boolean",
      "fullName": "string",
      "comment": "string",
      "isHighlight": "boolean",
      "insertedAt": "date",
      "allergyType": "string",
      "name": "string",
      "isNKA": "boolean",
      "sequenceNo": "number",
      "severity": "string"
    }
  ],
  "diagnoses": [
    {
      "diagnosisID": "number",
      "appointmentID": "number",
      "diseaseName": "string",
      "diagnosisDate": "date",
      "diagnosisBy": "string",
      "summary": "string",
      "isLongTerm": "boolean",
      "addtoProblem": "boolean",
      "isHighlighted": "boolean",
      "sequenceNo": "number",
      "isActive": "boolean",
      "isConfidential": "boolean",
      "diagnosisType": "string",
      "isMapped": "boolean",
      "practiceID": "number",
      "onSetDate": "date",
      "mappedBy": "string",
      "mappedDate": "date",
      "isStopped": "boolean",
      "snomedDiseaseName": "string",
      "patientID": "number",
      "practiceLocationID": "number",
      "isPrimaryDiagnosis": "boolean"
    }
  ]
}
```

## Error Handling

The API includes comprehensive error handling:

- **Database Connection Errors**: Detailed logging with connection string information
- **SQL Exceptions**: Specific handling for SQL Server errors
- **Data Processing Errors**: Record-level error handling that continues processing
- **Missing Data**: Graceful handling of null values and missing datasets
- **Timeout Protection**: 5-minute command timeout to prevent long-running queries

## Logging

The API provides detailed console logging for debugging:

- Connection status and database version
- Column information for each dataset
- Field-by-field data reading with data types
- Error details with stack traces
- Summary statistics of retrieved data

## Development

### Prerequisites
- .NET 6.0 or later
- SQL Server database with the required stored procedures
- Valid database connection string

### Running the Application
```bash
dotnet run
```

The API will be available at `https://localhost:7001` (or the configured port).

### Testing
Use the provided `LabTestApi.http` file for testing endpoints with sample data.

## Troubleshooting

### Common Issues

1. **Database Connection Failed**
   - Verify the connection string in `appsettings.json`
   - Check if the database server is accessible
   - Ensure the database user has proper permissions

2. **Stored Procedure Not Found**
   - Verify the stored procedure exists in the database
   - Check the procedure name and parameters

3. **Data Type Conversion Errors**
   - Check the console logs for detailed field information
   - Verify the stored procedure returns the expected data types

4. **Timeout Issues**
   - The API includes a 5-minute timeout for database operations
   - Check if the stored procedure is optimized for performance

### Debug Information

The API provides extensive console logging that includes:
- Database connection status
- Stored procedure execution details
- Column information and data types
- Field-by-field data reading
- Error details with stack traces
- Summary statistics

Check the console output for detailed debugging information.