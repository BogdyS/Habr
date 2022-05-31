using FluentValidation.AspNetCore;
using Habr.WebAPI;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddServices();
builder.Services.AddDataContext(builder.Configuration);
builder.Services.AddAutoMapping();
builder.Services.AddValidation();
builder.Services.AddFilters();

builder.Services.AddMvc(options => options.SuppressAsyncSuffixInActionNames = false);

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
