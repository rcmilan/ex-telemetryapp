using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace telemetryapp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Logging.ClearProviders();
        builder.Logging.AddOpenTelemetry(o =>
        {
            o.SetResourceBuilder(
                ResourceBuilder.CreateEmpty()
                    .AddService(builder.Environment.ApplicationName)
                    .AddAttributes(new Dictionary<string, object>
                    {
                        // Add any desired resource attributes here
                        ["deployment.environment"] = builder.Environment.EnvironmentName
                    }));

            // Some important options to improve data quality
            o.IncludeScopes = true;
            o.IncludeFormattedMessage = true;

            o.AddOtlpExporter(a =>
            {
                a.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/logs");
                a.Protocol = OtlpExportProtocol.HttpProtobuf;
                a.Headers = "X-Seq-ApiKey=b1jp0U2ITL8woIITORYP";
            });
        });

        builder.Services.AddOpenTelemetry()
          .ConfigureResource(r => r.AddService(builder.Environment.ApplicationName))
          .WithTracing(tracing =>
          {
              tracing.AddSource("Example.Source");
              tracing.AddAspNetCoreInstrumentation();
              tracing.AddHttpClientInstrumentation();
              tracing.AddOtlpExporter(opt =>
              {
                  opt.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/traces");
                  opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                  opt.Headers = "X-Seq-ApiKey=b1jp0U2ITL8woIITORYP";
              });
          });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
