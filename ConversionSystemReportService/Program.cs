using conversionSystemReportService;

var builder = Host.CreateApplicationBuilder(args);
// builder.Services.AddHostedService<>();

var host = builder.Build();
host.Run();