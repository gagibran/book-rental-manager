using BookRentalManager.Application.Extensions;
using BookRentalManager.Infrastructure.Data;
using BookRentalManager.Infrastructure.Data.Seeds;
using BookRentalManager.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container:
builder.Services
    .AddControllers(configure => configure.ReturnHttpNotAcceptable = true)
    .AddNewtonsoftJson();
builder.Services.AddDbContext<BookRentalManagerDbContext>(dbContextOptionsBuilder =>
{
    dbContextOptionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("BookRentalManagerConnectionString"));
});
builder.Services.AddApiVersioning(apiVersioningOptions => apiVersioningOptions.ApiVersionReader = new UrlSegmentApiVersionReader());
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline:
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
app.UseAuthorization();
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
