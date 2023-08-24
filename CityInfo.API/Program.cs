using Serilog;
using CityInfo.API.Data;
using CityInfo.API.Services.MailService;
using Microsoft.AspNetCore.StaticFiles;

// It's applicable to use third-party library for logging on a file like Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("log/cityInfo.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllers(option =>
    option.ReturnHttpNotAcceptable = false
)   
    .AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters();                            // to support different format for the response based on Accept header

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();        // to determine the file content type for file controller

#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

builder.Services.AddSingleton<CitiesDataStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

/*
 To setup endpoint routing, two pieces of middleware can be used:
    1. UseRouting: mark the position in the middleware pipeline where routing decision is made 
    2. UseEndRouting: mark the position in the middleware pipeline where the selected endpoint is executed
 This allow injecting middleware that runs between selecting and executing endpoint like UseAuthorization

 In .NET 6, MapControllers middleware can be used instead of the above 2 middlewares.
 */

app.UseAuthorization();

app.UseEndpoints(endpoint =>
{
    endpoint.MapControllers();
});

app.Run();