using BookRentalManager.Application.Extensions;
using BookRentalManager.Infrastructure.Data;
using BookRentalManager.Infrastructure.Data.Seeds;
using BookRentalManager.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;

// Add services to the container:
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddControllers(mvcOptions => mvcOptions.ReturnHttpNotAcceptable = true)
    .AddNewtonsoftJson();
builder.Services.AddDbContext<BookRentalManagerDbContext>(dbContextOptionsBuilder =>
{
    dbContextOptionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("BookRentalManagerConnectionString"));
});
builder.Services.AddApiVersioning(apiVersioningOptions =>
{
    apiVersioningOptions.ApiVersionReader = new UrlSegmentApiVersionReader();
});
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();
builder.Services.Configure<MvcOptions>(config =>
{
    NewtonsoftJsonOutputFormatter? newtonsoftJsonOutputFormatter = config.OutputFormatters
        .OfType<NewtonsoftJsonOutputFormatter>()
        .FirstOrDefault();
    if (newtonsoftJsonOutputFormatter is not null)
    {
        newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add(MediaTypeConstants.BookRentalManagerHateoasMediaType);
    }
});
builder.Services.AddMemoryCache();

// Configure the HTTP request pipeline:
WebApplication app = builder.Build();
app.UseExceptionHandler("/internalServerError");
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
app.MapControllers();
if (app.Environment.IsDevelopment())
{
    using IServiceScope serviceScope = app.Services.CreateScope();
    IServiceProvider serviceProvider = serviceScope.ServiceProvider;
    BookRentalManagerDbContext bookRentalManagerDbContext = serviceProvider.GetRequiredService<BookRentalManagerDbContext>();
    await bookRentalManagerDbContext.Database.MigrateAsync();
    TestDataSeeder testDataSeeder = serviceProvider.GetRequiredService<TestDataSeeder>();
    await testDataSeeder.SeedTestDataAsync();
}
app.Run();
