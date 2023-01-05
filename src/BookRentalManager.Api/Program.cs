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
if (app.Environment.IsDevelopment())
{
    using IServiceScope serviceScope = app.Services.CreateScope();
    IServiceProvider serviceProvider = serviceScope.ServiceProvider;
    AppDbContext? appDbContext = serviceProvider.GetService<AppDbContext>();
    TestDataSeeder? testDataSeeder = serviceProvider.GetService<TestDataSeeder>();
    if (appDbContext is not null)
    {
        await appDbContext.Database.MigrateAsync();
    }
    if (testDataSeeder is not null)
    {
        await testDataSeeder.SeedTestDataAsync();
    }
}
app.Run();
