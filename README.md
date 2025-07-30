# Lab Test API

A comprehensive ASP.NET Core Web API for laboratory test management that leverages modern .NET 9.0 features with Entity Framework Core for database operations and Swagger for API documentation and testing.

## Framework & Runtime

- **.NET 9.0** - Latest .NET framework version
- **ASP.NET Core Web API** - Web API framework
- **C#** - Programming language

## NuGet Packages Used

### Core Framework Packages
- **Microsoft.AspNetCore.OpenApi (v9.0.4)** - Provides OpenAPI/Swagger documentation support
- **Microsoft.EntityFrameworkCore (v9.0.7)** - Core Entity Framework Core package
- **Microsoft.EntityFrameworkCore.SqlServer (v9.0.7)** - SQL Server provider for Entity Framework Core
- **Microsoft.EntityFrameworkCore.Design (v9.0.7)** - Design-time tools for Entity Framework Core
- **Swashbuckle.AspNetCore (v9.0.3)** - Swagger/OpenAPI documentation generator

## Database & Data Access

- **SQL Server** - Primary database
- **Entity Framework Core** - ORM for database operations
- **Stored Procedure**: `[dbo].[GetLabTestDataWithJoins]` - Main data retrieval procedure

## Project Structure

```
LabTestApi/
├── Controllers/
│   └── LabTestController.cs          # API endpoints
├── Data/
│   └── LabTestDbContext.cs          # Entity Framework context
├── Models/
│   └── LabTestData.cs               # Data model
├── Services/
│   ├── ILabTestService.cs           # Service interface
│   └── LabTestService.cs            # Service implementation
├── appsettings.json                 # Configuration
├── Program.cs                       # Application startup
└── LabTestApi.http                 # HTTP client tests
```

## Setup Instructions

### 1. Database Configuration

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your_server_name;Database=your_database_name;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 2. Restore NuGet Packages

```bash
dotnet restore
```

### 3. Run the Application

```bash
dotnet run
```

The API will be available at:
- **API Base URL**: `https://localhost:7037` or `http://localhost:5037`
- **Swagger UI**: `https://localhost:7037/swagger` or `http://localhost:5037/swagger`

## API Endpoints

### 1. Get All Lab Test Data
```
GET /api/labtest
```
Returns all lab test data from the stored procedure.

### 2. Get Lab Test Data by Patient
```
GET /api/labtest/patient/{patientId}
```
Returns lab test data for a specific patient.

**Parameters:**
- `patientId` (string) - The patient ID to filter by

### 3. Get Lab Test Data by Date Range
```
GET /api/labtest/daterange?startDate={date}&endDate={date}
```
Returns lab test data within a specified date range.

**Parameters:**
- `startDate` (DateTime) - Start date for the range
- `endDate` (DateTime) - End date for the range

## Data Model

The `LabTestData` model represents the data returned by the stored procedure and includes:

- **Patient Information**: NHINumber, FullName, DOB, GenderName, PatientID
- **Message Information**: SendingApplication, SendingFacility, ReceivingFacility, MessageDatetime
- **Lab Test Details**: SnomedCode, ResultName, ObservationValue, Units, ReferenceRanges
- **Status Information**: MarkasRead, StatusChangeDateTime, AbnormalFlagID
- **Comments**: Source, Comments

## Development Tools

- **Visual Studio / VS Code** - IDE for development
- **HTTP Client** - Built-in HTTP testing (LabTestApi.http file)
- **Swagger UI** - Interactive API documentation and testing
- **Entity Framework CLI** - Database migration tools

## Testing

Use the provided `LabTestApi.http` file to test the API endpoints:

```http
### Get all lab test data
GET http://localhost:5037/api/labtest
Accept: application/json

### Get lab test data by patient ID
GET http://localhost:5037/api/labtest/patient/12345
Accept: application/json

### Get lab test data by date range
GET http://localhost:5037/api/labtest/daterange?startDate=2024-01-01&endDate=2024-12-31
Accept: application/json
```

## Security & Configuration

- **HTTPS Redirection** - Secure communication
- **Authorization** - Built-in authorization middleware
- **Connection String Security** - Database authentication
- **Environment-specific Configuration** - Separate dev/prod settings

## Stored Procedure

The API calls the `[dbo].[GetLabTestDataWithJoins]` stored procedure which joins multiple tables:

- LabTestMSH (Message Header)
- LabTestOBR (Observation Request)
- LabTestOBX (Observation Result)
- LabTestNTE (Notes and Comments)
- Patient and Gender lookup tables

This provides a comprehensive view of lab test data including patient information, test results, and status tracking.