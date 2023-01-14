using BookRentalManager.Application.Extensions;
using BookRentalManager.Infrastructure.Data;
using BookRentalManager.Infrastructure.Data.Seeds;
using BookRentalManager.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container:
builder.Services.AddControllers();
builder.Services.AddDbContext<BookRentalManagerDbContext>(dbContextOptionsBuilder =>
{
    dbContextOptionsBuilder.UseNpgsql(
        builder.Configuration.GetConnectionString("BookRentalManagerConnectionString")
    );
});
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline:
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
using IServiceScope serviceScope = app.Services.CreateScope();
IServiceProvider serviceProvider = serviceScope.ServiceProvider;
BookRentalManagerDbContext bookRentalManagerDbContext = serviceProvider.GetRequiredService<BookRentalManagerDbContext>();
await bookRentalManagerDbContext.Database.MigrateAsync();
if (app.Environment.IsDevelopment())
{
    TestDataSeeder testDataSeeder = serviceProvider.GetRequiredService<TestDataSeeder>();
    await testDataSeeder.SeedTestDataAsync();
}
app.Run();
