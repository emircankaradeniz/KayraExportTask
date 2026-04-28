using LogService.Infrastructure;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithProcessId()
        .Enrich.WithThreadId()
        .Enrich.WithProperty("ServiceName", "LogService")
        .WriteTo.Console()
        .WriteTo.Seq(context.Configuration["Serilog:SeqUrl"] ?? "http://localhost:5341");
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Kayra Export Log Service API",
        Version = "v1",
        Description = "Centralized log microservice for collecting and querying structured application logs."
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Kayra Export Log Service API v1");
    options.RoutePrefix = "swagger";
});

app.UseCors("DefaultCorsPolicy");

app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    service = "Kayra Export Log Service",
    status = "Running",
    timestampUtc = DateTime.UtcNow
}));

app.Run();