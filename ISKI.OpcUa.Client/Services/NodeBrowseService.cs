using Opc.Ua;
using Opc.Ua.Client;
using Microsoft.Extensions.Logging;
using ISKI.OpcUa.Client.Interfaces;
using ISKI.OpcUa.Client.Models;

namespace ISKI.OpcUa.Client.Services;

public class NodeBrowseService : INodeBrowseService
{
    private readonly ILogger<NodeBrowseService> _logger;
    private readonly IConnectionService _connection;

    public NodeBrowseService(ILogger<NodeBrowseService> logger, IConnectionService connection)
    {
        _logger = logger;
        _connection = connection;
    }

    public List<NodeBrowseResult> Browse(string nodeId)
    {
        var session = _connection.Session;
        var results = new List<NodeBrowseResult>();

        if (session == null)
        {
            _logger.LogWarning("Browse çağrısı yapıldı ama oturum yok.");
            return new List<NodeBrowseResult>
        {
            new NodeBrowseResult
            {
                DisplayName = "Oturum yok",
                NodeId = "N/A",
                NodeClass = "Error"
            }
        };
        }

        try
        {
            var browseDesc = new BrowseDescription
            {
                NodeId = new NodeId(nodeId),
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
                IncludeSubtypes = true,
                NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable),
                ResultMask = (uint)BrowseResultMask.All
            };

            session.Browse(
                null, null, 0,
                new BrowseDescriptionCollection { browseDesc },
                out var browseResults, out var diagnosticInfos
            );

            foreach (var reference in browseResults[0].References)
            {
                results.Add(new NodeBrowseResult
                {
                    DisplayName = reference.DisplayName.Text,
                    NodeId = reference.NodeId.ToString(),
                    NodeClass = reference.NodeClass.ToString()
                });
            }

            _logger.LogInformation("Browse işlemi tamamlandı. {count} node bulundu.", results.Count);

            if (results.Count == 0)
            {
                results.Add(new NodeBrowseResult
                {
                    DisplayName = "Alt node bulunamadı",
                    NodeId = nodeId,
                    NodeClass = "Empty"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Browse sırasında hata oluştu: {nodeId}", nodeId);
            results.Clear();
            results.Add(new NodeBrowseResult
            {
                DisplayName = $"Hata: {ex.Message}",
                NodeId = nodeId,
                NodeClass = "Exception"
            });
        }

        return results;
    }
}
