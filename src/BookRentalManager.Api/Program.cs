using BookRentalManager.Application.Extensions;
using BookRentalManager.Infrastructure.Data;
using BookRentalManager.Infrastructure.Data.Seeds;
using BookRentalManager.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container:
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(dbContextOptionsBuilder =>
{
    dbContextOptionsBuilder.UseNpgsql(
        builder.Configuration.GetConnectionString("DevelopmentConnectionString")
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
AppDbContext appDbContext = serviceProvider.GetRequiredService<AppDbContext>();
await appDbContext.Database.MigrateAsync();
if (app.Environment.IsDevelopment())
{
    TestDataSeeder testDataSeeder = serviceProvider.GetRequiredService<TestDataSeeder>();
    await testDataSeeder.SeedTestDataAsync();
}
app.Run();
