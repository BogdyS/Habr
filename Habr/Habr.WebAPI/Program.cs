using FluentValidation.AspNetCore;
using Habr.WebAPI;
using NLog.Web;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddServices();
builder.Services.AddDataContext(builder.Configuration);
builder.Services.AddAutoMapping();
builder.Services.AddValidation();
builder.Services.AddFilters();

builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Host.UseNLog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
