using System.Text.Json;
using System.Text.Json.Serialization;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Spark.Library.Config;
using Spark.Library.Environment;

using dotnetbase.Application.Middleware;
using dotnetbase.Application.Services;
using dotnetbase.Application.Services.Middleware;
using dotnetbase.Application.Startup;
using static System.Net.Mime.MediaTypeNames;

EnvManager.LoadConfig();

var builder = WebApplication.CreateBuilder(args);




builder.Configuration.SetupSparkConfig();

builder.Services.AddHttpClient();

builder.Services.AddAppServices(builder.Configuration);


builder.Services.AddResponseCompression(options =>
{
	options.EnableForHttps = true;
});



builder.Services.ConfigureHttpJsonOptions(options =>
{
	options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
	options.SerializerOptions.WriteIndented = true;
	options.SerializerOptions.IncludeFields = true;
	options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

});

builder.Services.AddOutputCache(options =>
{
	options.AddBasePolicy(builder =>
		builder.Expire(TimeSpan.FromSeconds(10)));
	options.AddPolicy("CacheFor30Seconds", builder =>
		builder.Expire(TimeSpan.FromSeconds(30)));
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();


app.UseResponseCompression();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.Use(next => context =>
{
	context.Request.EnableBuffering();
	return next(context);
});

app.UseMiddleware<AuditLogMiddleware>();
//


app.MapControllers();

app.Services.RegisterScheduledJobs();
app.Services.RegisterEvents();
app.UseOutputCache();

app.MapHealthChecks("/health", new HealthCheckOptions
{
	ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
//http://localhost:5218/healthchecks-ui#/healthchecks
//app.MapHealthChecksUI();
//app.MapHealthChecks();

app.Run();
