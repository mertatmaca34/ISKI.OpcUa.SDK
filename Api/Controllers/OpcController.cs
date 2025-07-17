using ISKI.OpcUa.Client.Interfaces;
using ISKI.OpcUa.Client.Models;
using ISKI.OpcUa.Client.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OpcController(
    ILogger<OpcController> logger,
    IConnectionService connectionService,
    INodeReadWriteService readWriteService,
    INodeBrowseService browseService,
    IDiscoveryService discoveryService
) : ControllerBase
{
    private static bool _connected;

    [HttpPost("connect")]
    public async Task<ActionResult<ConnectionResult<object>>> Connect([FromQuery] string endpoint)
    {
        try
        {
            // Gerçekten bağlantı var mı kontrol et
            if (_connected && connectionService.Session is not null && connectionService.Session.Connected)
            {
                return Ok(new ConnectionResult<object>
                {
                    ServerStatus = "AlreadyConnected",
                    Message = ErrorMessages.GetMessage(ErrorCode.AlreadyConnected),
                });
            }

            await connectionService.ConnectAsync(endpoint);
            _connected = true;

            logger.LogInformation("OPC UA bağlantısı kuruldu. Endpoint: {endpoint}", endpoint);

            return Ok(new ConnectionResult<object>
            {
                ServerStatus = "Good",
                Message = "Bağlantı kuruldu.",
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "OPC UA bağlantısı kurulamadı.");
            return StatusCode(500, new ConnectionResult<object>
            {
                ServerStatus = "Exception",
                Message = $"{ErrorMessages.GetMessage(ErrorCode.ConnectionFailed)} {ex.Message}",
            });
        }
    }

    [HttpGet("read")]
    public async Task<ActionResult<ConnectionResult<NodeReadResult>>> ReadNode([FromQuery] string nodeId)
    {
        var result = await readWriteService.ReadNodeAsync(nodeId);

        var response = new ConnectionResult<NodeReadResult>
        {
            ServerStatus = result.ServerStatus,
            Message = result.Message,
            Timestamp = result.Timestamp,
            Data = result.Data
        };

        if (!response.Success)
        {
            logger.LogWarning("ReadNode başarısız: {nodeId}, Status: {status}", nodeId, result.Data.NodeStatus);
            return BadRequest(response);
        }

        logger.LogInformation("ReadNode başarılı: {nodeId}, Value: {value}", nodeId, result.Data.Value);
        return Ok(response);
    }

    [HttpPost("write")]
    public async Task<ActionResult<ConnectionResult<object>>> WriteNode(
        [FromQuery] string nodeId,
        [FromQuery] string value,
        CancellationToken cancellationToken)
    {
        var result = await readWriteService.WriteNodeAsync(nodeId, value, cancellationToken);

        var response = new ConnectionResult<object>
        {
            ServerStatus = result.ServerStatus,
            Message = result.Message,
            Timestamp = result.Timestamp,
            Data = "null"
        };

        if (!response.Success)
        {
            logger.LogWarning("WriteNode başarısız: {nodeId}, Status: {status}", nodeId, result.ServerStatus);
            return BadRequest(response);
        }

        logger.LogInformation("WriteNode başarılı: {nodeId} = {value}", nodeId, value);
        return Ok(response);
    }

    [HttpGet("browse")]
    public ActionResult<ConnectionResult<List<NodeBrowseResult>>> Browse([FromQuery] string nodeId = "i=85")
    {
        try
        {
            var nodes = browseService.Browse(nodeId);

            logger.LogInformation("Browse: Node {nodeId}, Bulunan {count}", nodeId, nodes.Count);

            return Ok(new ConnectionResult<List<NodeBrowseResult>>
            {
                ServerStatus = "Good",
                Message = "Browse işlemi başarılı.",
                Data = nodes
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Browse hatası.");

            return StatusCode(500, new ConnectionResult<List<NodeBrowseResult>>
            {
                ServerStatus = "Exception",
                Message = $"{ErrorMessages.GetMessage(ErrorCode.BrowseFailed)} {ex.Message}",
            });
        }
    }


    [HttpGet("discover")]
    public async Task<ActionResult<ConnectionResult<List<string>>>> Discover([FromQuery] string networkPrefix = "192.168.1", [FromQuery] int port = 4840)
    {
        try
        {
            var servers = await discoveryService.FindServersOnLocalNetworkAsync(networkPrefix, port);

            logger.LogInformation("Discover tamamlandı. Sunucu sayısı: {count}", servers.Count);

            return Ok(new ConnectionResult<List<string>>
            {
                ServerStatus = "Good",
                Message = "Keşif başarılı.",
                Data = servers
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Discover hatası.");

            return StatusCode(500, new ConnectionResult<List<string>>
            {
                ServerStatus = "Exception",
                Message = $"{ErrorMessages.GetMessage(ErrorCode.DiscoveryFailed)} {ex.Message}",
            });
        }
    }

    [HttpPost("disconnect")]
    public async Task<ActionResult<ConnectionResult<object>>> Disconnect()
    {
        try
        {
            await connectionService.DisconnectAsync();
            _connected = false;

            logger.LogInformation("Bağlantı kesildi.");

            return Ok(new ConnectionResult<object>
            {
                ServerStatus = "Disconnected",
                Message = "Bağlantı kesildi.",
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Disconnect hatası.");

            return StatusCode(500, new ConnectionResult<object>
            {
                ServerStatus = "Exception",
                Message = $"{ErrorMessages.GetMessage(ErrorCode.DisconnectFailed)} {ex.Message}",
            });
        }
    }
}
