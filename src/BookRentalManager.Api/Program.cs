using BookRentalManager.Api.ExceptionHandlers;
using BookRentalManager.Infrastructure.Data;
using BookRentalManager.Infrastructure.Data.Seeds;
using BookRentalManager.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;

// Add services to the container:
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddControllers(mvcOptions =>
    {
        mvcOptions.Filters.Add(new ProducesAttribute(
            MediaTypeConstants.BookRentalManagerHateoasMediaType,
            MediaTypeConstants.ApplicationJsonMediaType));
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
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Configure the HTTP request pipeline:
WebApplication app = builder.Build();
app.UseExceptionHandler(_ => { }); // .NET 8 bug that needs to be handled with an anonymous function.
app.UseHttpsRedirection();
app.MapControllers();
if (app.Environment.IsDevelopment())
{
    using IServiceScope serviceScope = app.Services.CreateScope();
    IServiceProvider serviceProvider = serviceScope.ServiceProvider;
    BookRentalManagerDbContext bookRentalManagerDbContext = serviceProvider.GetRequiredService<BookRentalManagerDbContext>();
    await bookRentalManagerDbContext.Database.MigrateAsync();
    await TestDataSeeder.SeedTestDataAsync(bookRentalManagerDbContext);
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Run();
