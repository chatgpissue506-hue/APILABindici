using LabTestApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add OpenAPI/Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Lab Test API",
        Version = "v1",
        Description = "API for retrieving lab test data from SQL Server stored procedure"
    });
});

// Add services
builder.Services.AddScoped<ILabTestService, LabTestService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab Test API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// Serve static files (for the HTML documentation)
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

// Add a simple API documentation endpoint
app.MapGet("/api-docs", () => new
{
    message = "Lab Test API Documentation",
    version = "1.0.0",
    endpoints = new[]
    {
        new { method = "GET", path = "/api/labtest", description = "Get all lab test data" },
        new { method = "GET", path = "/api/labtest/patient/{patientId}", description = "Get lab test data by patient ID" },
        new { method = "GET", path = "/api/labtest/daterange?startDate={date}&endDate={date}", description = "Get lab test data by date range" },
        new { method = "GET", path = "/api/labtest/filter?patientId={id}&startDate={date}&endDate={date}&practiceId={id}", description = "Get lab test data with flexible filters" }
    },
    baseUrl = "http://localhost:5050",
    documentation = "Visit http://localhost:5050/swagger for Swagger UI or http://localhost:5050 for HTML documentation"
});

app.Run();
