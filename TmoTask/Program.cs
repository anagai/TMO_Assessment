using Microsoft.Extensions.DependencyInjection;
using TmoTask.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDataService, DataService>();

var app = builder.Build();

var dataService = app.Services.GetRequiredService<IDataService>();

dataService.AggregateSalesData();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
