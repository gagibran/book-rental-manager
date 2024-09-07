using System.Reflection;
using BookRentalManager.Api.ExceptionHandlers;
using BookRentalManager.Infrastructure.Data;
using BookRentalManager.Infrastructure.Data.Seeds;
using BookRentalManager.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace BookRentalManager.Api;

#pragma warning disable CS1591
public class Program
{
    const string ApplicationName = "Book Rental Manager API";

    private static async Task Main(string[] args)
    {
        WebApplicationBuilder webApplicationBuilder = WebApplication.CreateBuilder(args);
        AddServices(webApplicationBuilder);
        WebApplication webApplication = webApplicationBuilder.Build();
        await ConfigureHttpRequestPipelineAsync(webApplication, args);
        webApplication.Run();
    }

    private static void AddServices(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services
            .AddControllers(mvcOptions => mvcOptions.ReturnHttpNotAcceptable = true)
            .AddNewtonsoftJson();
        webApplicationBuilder.Services.AddDbContext<BookRentalManagerDbContext>(dbContextOptionsBuilder =>
        {
            dbContextOptionsBuilder.UseNpgsql(webApplicationBuilder.Configuration.GetConnectionString("BookRentalManagerConnectionString"));
        });
        webApplicationBuilder.Services
            .AddApiVersioning(apiVersioningOptions =>
            {
                apiVersioningOptions.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(apiExplorerAction =>
            {
                apiExplorerAction.GroupNameFormat = "'v'VVV";
                apiExplorerAction.SubstituteApiVersionInUrl = true;
            });
        webApplicationBuilder.Services.AddApplicationServices();
        webApplicationBuilder.Services.AddInfrastructureServices();
        webApplicationBuilder.Services.Configure<MvcOptions>(configureOptions =>
        {
            NewtonsoftJsonInputFormatter newtonsoftJsonInputFormatter = configureOptions.InputFormatters
                .OfType<NewtonsoftJsonInputFormatter>()
                .First();
            NewtonsoftJsonOutputFormatter newtonsoftJsonOutputFormatter = configureOptions.OutputFormatters
                .OfType<NewtonsoftJsonOutputFormatter>()
                .First();
            newtonsoftJsonInputFormatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson);
            newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson);
        });
        webApplicationBuilder.Services.AddSwaggerGen(swaggerGenOptions =>
        {
            swaggerGenOptions.EnableAnnotations();
            swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = ApplicationName,
                Description = "A system designed to be used in libraries to manage books and rentals."
            });
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            swaggerGenOptions.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        webApplicationBuilder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        webApplicationBuilder.Services.AddSingleton<OutputFormatterSelector, AcceptHeaderOutputFormatterSelector>();
    }

    private static async Task ConfigureHttpRequestPipelineAsync(WebApplication webApplication, string[] args)
    {
        webApplication.UseExceptionHandler(_ => { }); // .NET 8 bug that needs to be handled with an anonymous function.
        webApplication.UseHttpsRedirection();
        webApplication.MapControllers();
        if (webApplication.Environment.IsDevelopment() || webApplication.Environment.EnvironmentName == "Docker")
        {
            webApplication.UseSwagger();
            webApplication.UseSwaggerUI(swaggerUiOptions =>
            {
                swaggerUiOptions.DocumentTitle = ApplicationName;
                swaggerUiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                swaggerUiOptions.RoutePrefix = string.Empty;
            });
            using IServiceScope serviceScope = webApplication.Services.CreateScope();
            IServiceProvider serviceProvider = serviceScope.ServiceProvider;
            BookRentalManagerDbContext bookRentalManagerDbContext = serviceProvider.GetRequiredService<BookRentalManagerDbContext>();
            await bookRentalManagerDbContext.Database.MigrateAsync();
            if (args.Contains("--with-initial-data=true"))
            {
                await TestDataSeeder.SeedTestDataAsync(bookRentalManagerDbContext);
            }
        }
    }
}
#pragma warning restore CS1591
