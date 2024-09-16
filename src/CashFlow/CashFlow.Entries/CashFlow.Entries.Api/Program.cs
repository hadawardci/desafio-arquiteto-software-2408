using CashFlow.Entries.Api.Authorization;
using CashFlow.Entries.Api.Configurations;
using CashFlow.Entries.Api.Filters;
using CashFlow.Entries.Application.AddEntry;
using CashFlow.Entries.Infra.Data.DatabaseConfigurations;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.OpenApi.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(options
                =>
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
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Cash Flow - Entries", Version = "v1" });
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

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddEntryInput).Assembly));

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .Build();

builder.Services
    .ConfigureInfra(configuration)
    .AddSecurity(configuration);


builder.Services.AddCors(p => p.AddPolicy("CORS", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

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
