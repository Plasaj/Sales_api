using Sales_api.Interfaces.Articles;
using Sales_api.Interfaces.Reports;
using Sales_api.Interfaces.Sales;
using Sales_api.Services.Articles;
using Sales_api.Services.Reports;
using Sales_api.Services.Sales;
using sales_dal.Data;
using sales_dal.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<IDapperContext, DapperContext>();
builder.Services.AddTransient<IArticlesService, ArticlesService>();
builder.Services.AddTransient<ISalesService, SalesService>();
builder.Services.AddTransient<IReportsService, ReportsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
