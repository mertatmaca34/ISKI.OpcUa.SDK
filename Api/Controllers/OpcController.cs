using IskiOpcSdk.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OpcController(IOpcUaService opcService) : ControllerBase
{
    private static bool _connected = false;

    [HttpPost("connect")]
    public async Task<IActionResult> Connect([FromQuery] string endpoint)
    {
        if (_connected)
            return Ok("Zaten bağlı");

        await opcService.ConnectAsync(endpoint);
        _connected = true;
        return Ok("OPC UA bağlantısı kuruldu.");
    }

    [HttpGet("read")]
    public async Task<IActionResult> ReadNode([FromQuery] string nodeId)
    {
        var value = await opcService.ReadNodeAsync(nodeId);
        return Ok(new { nodeId, value });
    }

    [HttpPost("write")]
    public async Task<IActionResult> WriteNode(
        [FromQuery] string nodeId,
        [FromQuery] string value,
        CancellationToken cancellationToken)
    {
        await opcService.WriteNodeAsync(nodeId, value, cancellationToken);
        return Ok("Yazma işlemi başarılı.");
    }

    [HttpGet("browse")]
    public IActionResult Browse([FromQuery] string nodeId = "i=85")
    {
        var nodes = opcService.Browse(nodeId);
        return Ok(nodes);
    }

    [HttpGet("discover")]
    public async Task<IActionResult> Discover()
    {
        var servers = await opcService.FindServersOnLocalNetworkAsync();
        return Ok(servers);
    }
}