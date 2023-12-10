using System.Reflection;
using BookRentalManager.Api.ExceptionHandlers;
using BookRentalManager.Infrastructure.Data;
using BookRentalManager.Infrastructure.Data.Seeds;
using BookRentalManager.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

// Add services to the container:
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddControllers(mvcOptions =>
    {
        mvcOptions.Filters.Add(new ProducesAttribute(
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            MediaTypeNames.Application.Json));
        mvcOptions.ReturnHttpNotAcceptable = true;
    })
    .AddNewtonsoftJson();
builder.Services.AddDbContext<BookRentalManagerDbContext>(dbContextOptionsBuilder =>
{
    dbContextOptionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("BookRentalManagerConnectionString"));
});
builder.Services
    .AddApiVersioning(apiVersioningOptions =>
    {
        apiVersioningOptions.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(apiExplorerAction =>
    {
        apiExplorerAction.GroupNameFormat = "'v'VVV";
        apiExplorerAction.SubstituteApiVersionInUrl = true;
    });
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();
builder.Services.Configure<MvcOptions>(configureOptions =>
{
    NewtonsoftJsonOutputFormatter? newtonsoftJsonOutputFormatter = configureOptions.OutputFormatters
        .OfType<NewtonsoftJsonOutputFormatter>()
        .FirstOrDefault();
    newtonsoftJsonOutputFormatter?.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson);
});
builder.Services.AddSwaggerGen(setupAction =>
{
    setupAction.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Book Rental Manager",
        Description = "A system designed to be used in libraries to manage books and rentals."
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    setupAction.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Configure the HTTP request pipeline:
WebApplication app = builder.Build();
app.UseExceptionHandler(_ => { }); // .NET 8 bug that needs to be handled with an anonymous function.
app.UseHttpsRedirection();
app.MapControllers();
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    using IServiceScope serviceScope = app.Services.CreateScope();
    IServiceProvider serviceProvider = serviceScope.ServiceProvider;
    BookRentalManagerDbContext bookRentalManagerDbContext = serviceProvider.GetRequiredService<BookRentalManagerDbContext>();
    await bookRentalManagerDbContext.Database.MigrateAsync();
    await TestDataSeeder.SeedTestDataAsync(bookRentalManagerDbContext);
    app.UseSwagger();
    app.UseSwaggerUI(setupAction =>
    {
        setupAction.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        setupAction.RoutePrefix = string.Empty;
    });
}
app.Run();
