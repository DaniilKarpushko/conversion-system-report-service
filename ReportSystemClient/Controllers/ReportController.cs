using Microsoft.AspNetCore.Mvc;
using ReportSystemClient.Records;
using ReportSystemClient.Services.ReportService;

namespace ReportSystemClient.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportController : ControllerBase
{
    private readonly IReportClientService _reportClientService;

    public ReportController(IReportClientService reportClientService)
    {
        _reportClientService = reportClientService;
    }
    
    [HttpPost]
    [Route("/{productId}/{start}/{end}")]
    public async Task<IActionResult> CreateReport(
        [FromRoute] int productId,
        [FromRoute] DateTime start,
        [FromRoute] DateTime end,
        CancellationToken cancellationToken)
    {
        var result = await _reportClientService.CreateReportAsync(
            productId,
            start.ToUniversalTime(),
            end.ToUniversalTime(),
            cancellationToken);

        return result switch
        {
            RequestResult.Success success => Ok(success.ReportId),
            RequestResult.Failure failure => BadRequest(failure.Message),
            _ => BadRequest(),
        };
    }
    
    [HttpGet]
    [Route("/{reportId}")] 
    public async Task<IActionResult> GetReport([FromRoute] string reportId, CancellationToken cancellationToken)
    {
        var result = await _reportClientService.GetReportAsync(reportId, cancellationToken);

        return result switch
        {
            ReportResult.Completed success => Ok(success.ToString()),
            ReportResult.Failed failure => BadRequest(failure.ToString()),
            _ => BadRequest(),
        };
    }
}