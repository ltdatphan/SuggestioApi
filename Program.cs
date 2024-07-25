using System.Net;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Newtonsoft.Json;
using SuggestioApi.DependencyInjection;
using SuggestioApi.RequestPipeline;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var environment = builder.Environment;
configuration.AddEnvironmentVariables();

builder.Services
    .AddCorsConfig(configuration)
    .AddServices()
    .AddSwagger()
    .AddPersistence(configuration, environment)
    .AddUserIdentity()
    .AddAuth(configuration)
    .AddExceptions();

builder.Services.AddControllers()
    .AddNewtonsoftJson(options => { options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; });

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromSeconds(10);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 5;
    });
});

// App settings
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.UseCsrfMiddleware();
app.UseExceptionHandler();
app.MapControllers();
app.MapFallback(context =>
{
    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
    return context.Response.WriteAsync("Resource not found");
});
app.UseRateLimiter();
// app.UseSanitizeMiddleware();

app.Run();