using Demo.Server.Controllers;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add Opentelemtry library
builder.Services.AddOpenTelemetry()
    .WithTracing(providerBuilder =>
    providerBuilder.AddAspNetCoreInstrumentation()
        .AddSource(nameof(WeatherForecastController))
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Server"))
        .AddJaegerExporter(opts =>
        {
            opts.AgentHost = builder.Configuration["Jaeger:AgentHost"];
            opts.AgentPort = Convert.ToInt32(builder.Configuration["Jaeger:AgentPort"]);
            opts.ExportProcessorType = ExportProcessorType.Simple;
        }))
    .StartWithHost();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
