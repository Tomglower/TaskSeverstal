using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Task3.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using log4net.Config;
using log4net;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseNpgsql(builder.Configuration.GetConnectionString("ConString"));
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddMvc();

var app = builder.Build();
try
{
    //Параметры для создания базы данных и применения миграции 
    var dbContext = app.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}
catch (Exception ex)
{
    Console.WriteLine("Произошла ошибка при миграции базы данных.");
}
try
{
    // Настройка log4net из конфигурационного файла
    var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
    XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
}
catch (Exception ex)
{
    Console.WriteLine("Error configuring log4net: " + ex.Message);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
