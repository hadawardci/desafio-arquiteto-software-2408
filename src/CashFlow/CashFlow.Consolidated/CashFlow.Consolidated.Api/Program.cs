using CashFlow.Consolidated.Api.Authorization;
using CashFlow.Consolidated.Api.Configurations;
using CashFlow.Consolidated.Api.Filters;
using CashFlow.Consolidated.Application.Events;
using CashFlow.Consolidated.Application.ViewConsolidatedDaily;
using CashFlow.Consolidated.Domain.SharedKernel.Events;
using CashFlow.Consolidated.Infra.Data.Configurations;
using CashFlow.Consolidated.Infra.Data.DatabaseConfigurations;
using CashFlow.Consolidated.Infra.Data.Gateways;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.OpenApi.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(options =>
    {
        options.Filters.Add(typeof(GlobalExceptionFilter));
        options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseTransformer()));
    }
            )
            .AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy
                = JsonNamingPolicy.CamelCase;
            });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Cash Flow - Consolidated", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    Array.Empty<string>()
                }
            });
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ViewConsolidatedDailyInput).Assembly));

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .Build();

builder.Services
    .AddTransient<IEntriesUpdateEventHandler, EntriesUpdateEventHandler>()
    .AddSingleton(configuration.GetSection("Database").Get<DatabaseConfigurationValue>()!)
    .AddSingleton(configuration.GetSection("ApplicationUser").Get<ApplicationUserValue>()!);
builder.Services
    .ConfigureInfra(configuration)
    .AddSecurity(configuration)
    ;
//builder.Services.AddSingleton<IEntriesUpdateEventGateway, EntriesUpdateEventConsumer>();

builder.Services.AddHostedService<EntriesUpdateEventConsumer>();

builder.Services.AddCors(p => p.AddPolicy("CORS", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//var messageConsumer = app.Services.GetRequiredService<IEntriesUpdateEventGateway>();
//messageConsumer.ReceiveMessages();

app.Run();
