using Microsoft.AspNetCore.Mvc;
using MyAPI.Contracts;

namespace MyAPI.Controllers;

[ApiController]
[Route("api/binance")]
public sealed class BinanceController(IBinanceFuturesService binanceFuturesService) : ControllerBase
{
    [HttpGet("futures/balance")]
    public async Task<IActionResult> GetFuturesBalanceAsync(
        [FromQuery] long timestamp,
        [FromQuery] string signature,
        [FromQuery] int recvWindow = 5000,
        [FromHeader(Name = "X-MBX-APIKEY")] string? apiKey = null,
        CancellationToken cancellationToken = default)
    {
        if (timestamp <= 0)
        {
            return BadRequest(new { message = "timestamp must be greater than 0." });
        }

        if (string.IsNullOrWhiteSpace(signature))
        {
            return BadRequest(new { message = "signature is required." });
        }

        if (recvWindow <= 0)
        {
            return BadRequest(new { message = "recvWindow must be greater than 0." });
        }

        try
        {
            var result = await binanceFuturesService.GetBalanceAsync(
                timestamp,
                signature,
                recvWindow,
                apiKey,
                cancellationToken);

            return StatusCode(result.StatusCode, result.Content);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(StatusCodes.Status499ClientClosedRequest);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                message = "Failed to call Binance Futures API.",
                detail = ex.Message
            });
        }
    }
}