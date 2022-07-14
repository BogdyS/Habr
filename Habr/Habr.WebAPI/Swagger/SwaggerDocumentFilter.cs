using System.Text.RegularExpressions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;


namespace Habr.WebAPI;

public class SwaggerDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        Regex regex = new Regex(@"/v\d+/");
        var keys = swaggerDoc.Paths.Where(path => !regex.IsMatch(path.Key)).ToList();
        keys.ForEach(x => swaggerDoc.Paths.Remove(x.Key));
    }
}