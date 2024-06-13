
using BettingAPI.Consumer;
using BettingAPI.Repositories;
using BettingAPI.Services;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.ResourceDetectors.Container;
using OpenTelemetry.ResourceDetectors.Host;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BettingAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {

            DotNetEnv.Env.Load(".env");

            var builder = WebApplication.CreateBuilder(args);

            //open telemetry
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddOpenTelemetry(options =>
            {
                options.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri("" + Environment.GetEnvironmentVariable("OTEL_uri"));
                });
            });

            static void addResource(ResourceBuilder resourceBuilder)
            {
                resourceBuilder.AddService("BettingAPI");
            }

            builder.Services
                .AddOpenTelemetry()
                .ConfigureResource(addResource)
                .WithTracing(tracerBuilder => tracerBuilder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri("" + Environment.GetEnvironmentVariable("OTEL_uri"));
                    })
                )
                .WithMetrics(meterBuilder => meterBuilder
                    .AddProcessInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri("" + Environment.GetEnvironmentVariable("OTEL_uri"));
                    })
            );



            // Database context injection
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
            var connectionString = $"Data Source={dbHost};Initial Catalog={dbName}; User ID=sa;Password={dbPassword}; TrustServerCertificate=True";
            builder.Services.AddDbContext<BettingDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddScoped<IBetRepository, BetRepository>();
            IdentityModelEventSource.ShowPII = true;

            // Mass transit
            builder.Services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.AddConsumer<MatchConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri("" + Environment.GetEnvironmentVariable("RabbitMQConnectionURI")), h =>
                    {
                        h.Username("" + Environment.GetEnvironmentVariable("RabbitUser"));
                        h.Password("" + Environment.GetEnvironmentVariable("RabbitPassword"));
                    });

                    cfg.ConfigureEndpoints(context);

                    cfg.ReceiveEndpoint("match-change", e =>
                    {
                        e.ConfigureConsumer<MatchConsumer>(context);
                    });
                });

            });

            builder.Services.AddScoped<BettingService>();
            builder.Services.AddControllers();

            Action<ResourceBuilder> appResourceBuilder =
                resource => resource
                    .AddDetector(new ContainerResourceDetector())
                    .AddDetector(new HostDetector());

            builder.Logging
                .AddOpenTelemetry(options => options.AddOtlpExporter())
                .AddConsole();

            builder.Services.AddOpenTelemetry()
                .ConfigureResource(appResourceBuilder)

                .WithTracing(tracerBuilder => tracerBuilder
                    .AddRedisInstrumentation(
                        options => options.SetVerboseDatabaseStatements = true)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter())

                .WithMetrics(meterBuilder => meterBuilder
                    .AddProcessInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter());


            //keycloak
            builder.Configuration.AddEnvironmentVariables();
            builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
            builder.Services
                .AddAuthorization()
                .AddKeycloakAuthorization(options =>
                {
                    options.EnableRolesMapping = RolesClaimTransformationSource.ResourceAccess;
                    options.RolesResource = $"{builder.Configuration["Keycloak:resource"]}";
                })
                .AddAuthorizationBuilder();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "SportsBet Betting API", Version = "v1" });
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });

                opt.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var home_dir = Environment.GetEnvironmentVariable("HOME_DIR");
                    apiDesc.RelativePath = home_dir + apiDesc.RelativePath;
                    return true;
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();

            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<BettingDbContext>();

                if ((db.Database.GetService<IDatabaseCreator>() is RelationalDatabaseCreator service) && (!service.Exists()))
                {
                    db.Database.Migrate();
                }
            }

            app.Run();
        }
    }
}
