using Microsoft.OpenApi.Models;
using ReportSystemClient.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReportSystemClient(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger(ac => { });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ReportAPI", Version = "v1" });
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.MapControllers();


app.Run();