using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.RateLimiting;
using ProyectChallenge.Api;
using ProyectChallenge.Api.Application.Middleware;
using ProyectChallenge.Api.Endpoints;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

// --- Serilog ---
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.Console());

// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Logging HTTP ---
builder.Services.AddHttpLogging(o =>
    o.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders | HttpLoggingFields.ResponseStatusCode);

// --- Rate Limiter ---
builder.Services.AddRateLimiter(options =>
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 60;
        opt.Window = TimeSpan.FromMinutes(1);
    }));



builder.Services.AddDependencyInyection();

builder.Services.AddProblemDetails();

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("Default", p => p
        .WithOrigins("https://tu-frontend.com")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();

// --- Middlewares ---
app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpLogging();
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapProductsEndpoints();

app.Run();
