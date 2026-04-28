using LogService.Application.Abstractions;
using LogService.Application.Logs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Threading;
using DomainLogLevel = LogService.Domain.LogLevel;

namespace LogService.API.Controllers;

[ApiController]
[Route("api/logs")]
public sealed class LogsController : ControllerBase
{
    private readonly ILogApplicationService _logApplicationService;

    public LogsController(ILogApplicationService logApplicationService)
    {
        _logApplicationService = logApplicationService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateLog(
        CreateLogRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _logApplicationService.CreateAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { message = result.Error });
        }

        return CreatedAtAction(nameof(GetLatestLogs), new { count = 50 }, result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetLatestLogs(
        [FromQuery] int count,
        CancellationToken cancellationToken)
    {
        var result = await _logApplicationService.GetLatestAsync(count, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpGet("level/{level}")]
    public async Task<IActionResult> GetLogsByLevel(
    DomainLogLevel level,
    [FromQuery] int count,
    CancellationToken cancellationToken)
    {
        var result = await _logApplicationService.GetByLevelAsync(level, count, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Value);
    }
}