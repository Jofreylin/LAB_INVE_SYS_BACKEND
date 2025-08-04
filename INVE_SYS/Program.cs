using INVE_SYS.Context;
using INVE_SYS.Services;
using INVE_SYS.Utilities;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
var services = builder.Services;

services.AddDbContext<INSYContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString($"DBConnection")));

services.AddScoped<IInventoryProductService, InventoryProductService>();
services.AddScoped<IStockService, StockService>();
services.AddScoped<IInventoryService, InventoryService>();
services.AddScoped<IWarehouseService, WarehouseService>();
services.AddHttpClient<IExternalApiService, ExternalApiService>();
services.AddScoped<ISupplierService, SupplierService>();


services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   
}
app.UseCors("AllowAllOrigins");
app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
