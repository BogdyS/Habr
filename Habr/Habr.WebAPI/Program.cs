using Habr.WebAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using NLog.Web;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddServices();
builder.Services.AddDataContext(builder.Configuration);
builder.Services.AddAutoMapping();
builder.Services.AddValidation();
builder.Services.AddFilters();
builder.Services.AddJwt(builder.Configuration);
builder.Services.AddVersions();

builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);

builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
    setup.AssumeDefaultVersionWhenUnspecified = true;
    setup.DefaultApiVersion = new ApiVersion(1, 0);
});
builder.Services.ConfigureOptions<SwaggerOptions>();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Host.UseNLog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/v{description.ApiVersion}/swagger.json", $"version {description.ApiVersion}");
        }
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
