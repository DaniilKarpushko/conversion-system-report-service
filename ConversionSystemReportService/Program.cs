using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Grpc.AspNetCore.Server;
using conversionSystemReportService;
using conversionSystemReportService.Extensions;
using ConversionSystemReportService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureService(builder.Configuration);


var app = builder.Build();
app.MapGrpcService<ReportServiceGrpc>();
app.Run();