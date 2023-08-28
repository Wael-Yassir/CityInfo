using Serilog;
using CityInfo.API.Data;
using CityInfo.API.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.StaticFiles;
using CityInfo.API.Services.MailService;
using CityInfo.API.Services.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
builder.Services.AddDbContext<CityInfoContext>(contextOptions =>
{
    contextOptions.UseSqlite(builder.Configuration["ConnectionStrings:CityInfoDBConnectionString"]);
});

builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = builder.Configuration["Authentication:Audience"],
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretKey"]))
        };
    });

// Authorization based on claims called ABAC/CBAC/PBAC:
// attribute-based, claims-based, policy-based access control
// for more complex scenario, RBAC: role-based access control is used
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeFromAntwerp", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("city", "Antwerp");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

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