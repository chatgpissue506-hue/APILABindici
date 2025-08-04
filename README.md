# Lab Test API

A comprehensive ASP.NET Core Web API for retrieving laboratory test data from SQL Server using stored procedures.

## 🚀 Features

- **.NET 9.0** - Latest .NET framework version
- **ASP.NET Core Web API** - Modern web API framework
- **SQL Server Integration** - Direct database access using stored procedures
- **Swagger Documentation** - Interactive API documentation
- **Comprehensive Error Handling** - Detailed logging and error messages
- **Flexible Filtering** - Multiple filtering options for data retrieval

## �� API Endpoints

### Lab Test Data Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/labtest` | Get all lab test data |
| GET | `/api/labtest/patient/{patientId}` | Get lab test data by patient ID (string) |
| GET | `/api/labtest/patient-sp/{patientId:long}` | Get lab test data by patient ID using GetPatientLabTestData SP (bigint) |
| GET | `/api/labtest/patient-labtest-updated/{patientId:long}` | Get structured lab test data using updated GetPatientLabTestData SP (header + details) |
| GET | `/api/labtest/patient-allergies/{patientId:long}` | Get patient allergies |
| GET | `/api/labtest/patient-diagnoses/{patientId:long}` | Get patient diagnoses |
| GET | `/api/labtest/patient-info/{patientId:long}` | Get patient information using GetPatientnameforLAB SP (includes ethnicity) |
| GET | `/api/labtest/daterange?startDate={date}&endDate={date}` | Get lab test data by date range |
| GET | `/api/labtest/filter?patientId={id}&startDate={date}&endDate={date}&practiceId={id}` | Get lab test data with flexible filters |

### External API Integration Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/externalapi/diagnosis/search?query={term}` | Search ICD-10 diagnosis codes using NIH Clinical Tables API |
| GET | `/api/externalapi/medication/search?search={term}` | Search medications using RxNav API |
| GET | `/api/externalapi/info` | Get information about available external APIs |

### Documentation Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api-docs` | API documentation in JSON format |
| GET | `/swagger` | Swagger UI for interactive API testing |
| GET | `/` | HTML documentation page |

## 🗄️ Database Integration

### Stored Procedures

1. **`GetLabTestDataWithJoins`** - Retrieves comprehensive lab test data with all joins
2. **`GetPatientLabTestData`** - **NEW** - Retrieves lab test data for a specific patient ID

### Database Schema

The API connects to SQL Server database `PMS_NZ_V2` and accesses tables in the `appointment` schema:

- `tbllabtest_msh` - Message header information
- `tbllabtest_obr` - Observation request information  
- `tbllabtest_obx` - Observation results
- `tbllabtest_nte` - Notes and comments

## 🛠️ Technology Stack

- **Framework**: .NET 9.0
- **Web API**: ASP.NET Core Web API
- **Database**: SQL Server 2019
- **Data Access**: ADO.NET (System.Data.SqlClient)
- **Documentation**: Swagger/OpenAPI
- **Language**: C#

## 📦 NuGet Packages

```xml
<PackageReference Include="System.Data.SqlClient" Version="4.8.6" />
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="9.0.4" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="7.0.0" />
```

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK
- SQL Server 2019 or later
- Access to the target database

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/chatgpissue506-hue/APILABindici.git
   cd APILABindici
   ```

2. **Configure connection string**
   Update `appsettings.json` with your database connection:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=your-server;Initial Catalog=your-database;User ID=your-user;Password=your-password;TrustServerCertificate=True;"
     }
   }
   ```

3. **Restore packages**
   ```bash
   dotnet restore
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the API**
   - API Base URL: `http://localhost:5050`
   - Swagger UI: `http://localhost:5050/swagger`
   - HTML Documentation: `http://localhost:5050`

## 📊 Data Model

The API returns `LabTestData` objects containing:

- **Patient Information**: NHI Number, Full Name, DOB, Gender
- **Test Information**: Test codes, descriptions, observation values
- **Timestamps**: Message datetime, observation datetime, status changes
- **Results**: Values, units, reference ranges, abnormal flags
- **Metadata**: Source, comments, practice information

## 🔧 Configuration

### Connection String

The application uses the connection string from `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=dbserver-local;Initial Catalog=PMS_NZ_V2;User ID=pms_nz;Password=pms@@nz;TrustServerCertificate=True;Connection Timeout=120;"
  }
}
```

### CORS Configuration

The API supports Cross-Origin Resource Sharing (CORS) with different policies for development and production:

#### Development Environment
- **Policy**: `AllowAll`
- **Allows**: Any origin, method, and header
- **Use Case**: Local development and testing

#### Production Environment
- **Policy**: `Restricted`
- **Configuration**: Uses settings from `appsettings.json`
- **Customizable**: Origins, methods, and headers can be configured

#### CORS Settings in appsettings.json
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:4200",
      "http://localhost:8080",
      "https://yourdomain.com"
    ],
    "AllowedMethods": [
      "GET",
      "POST",
      "PUT",
      "DELETE",
      "OPTIONS"
    ],
    "AllowedHeaders": [
      "Content-Type",
      "Authorization",
      "X-Requested-With"
    ]
  }
}
```

### Environment-Specific Settings

- `appsettings.json` - Production settings
- `appsettings.Development.json` - Development settings

## 🧪 Testing

### Using HTTP Client

The project includes `LabTestApi.http` for testing endpoints:

```http
### Get all lab test data
GET http://localhost:5050/api/labtest

### Get lab test data by patient ID (string)
GET http://localhost:5050/api/labtest/patient/P001

### Get lab test data by patient ID (bigint) - NEW ENDPOINT
GET http://localhost:5050/api/labtest/patient/46359

### Get lab test data by date range
GET http://localhost:5050/api/labtest/daterange?startDate=2024-01-01&endDate=2024-12-31

### Get lab test data with filters
GET http://localhost:5050/api/labtest/filter?patientId=P001&startDate=2024-01-01&endDate=2024-12-31&practiceId=PRACTICE001
```

### Using Swagger UI

1. Navigate to `http://localhost:5050/swagger`
2. Select an endpoint
3. Click "Try it out"
4. Enter parameters
5. Click "Execute"

## 🔍 Troubleshooting

### Common Issues

1. **Connection Failed**
   - Verify SQL Server is running
   - Check connection string parameters
   - Ensure network connectivity

2. **Stored Procedure Not Found**
   - Verify stored procedures exist in the database
   - Check user permissions

3. **Data Type Mismatch**
   - The API includes comprehensive error handling
   - Check console logs for detailed error messages

### Logging

The application provides detailed console logging:
- Connection status
- SQL Server version
- Stored procedure existence
- Data retrieval statistics
- Error details with troubleshooting tips

## 📈 Performance

- **Connection Pooling**: Automatic connection management
- **Async Operations**: Non-blocking database operations
- **Error Handling**: Graceful fallback to mock data
- **Logging**: Detailed performance and error tracking

## 🔒 Security

- **Connection String Security**: Database authentication
- **HTTPS Redirection**: Secure communication (when configured)
- **Input Validation**: Parameter validation and sanitization
- **Error Handling**: Secure error messages without exposing sensitive data

## 📝 License

This project is part of the APILABindici repository.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

---

**Repository**: [https://github.com/chatgpissue506-hue/APILABindici](https://github.com/chatgpissue506-hue/APILABindici)

## 🔗 External API Sources

### ICD-10 Diagnosis Search
- **Source**: [NIH Clinical Tables API](https://clinicaltables.nlm.nih.gov/api/icd10cm/v3/search)
- **Endpoint**: `GET /api/externalapi/diagnosis/search?query={term}`
- **Description**: Search for ICD-10 diagnosis codes and names
- **Example**: `GET /api/externalapi/diagnosis/search?query=diabetes`

### Medication Search
- **Source**: [RxNav API](https://rxnav.nlm.nih.gov/REST/drugs.json)
- **Endpoint**: `GET /api/externalapi/medication/search?search={term}`
- **Description**: Search for medications and drug information
- **Example**: `GET /api/externalapi/medication/search?search=aspirin`

## 📋 Example Usage

### Search for Diabetes Diagnosis
```bash
curl -X GET "http://localhost:5050/api/externalapi/diagnosis/search?query=diabetes" \
  -H "Accept: application/json"
```

### Search for Aspirin Medication
```bash
curl -X GET "http://localhost:5050/api/externalapi/medication/search?search=aspirin" \
  -H "Accept: application/json"
```

### Get External API Information
```bash
curl -X GET "http://localhost:5050/api/externalapi/info" \
  -H "Accept: application/json"
```