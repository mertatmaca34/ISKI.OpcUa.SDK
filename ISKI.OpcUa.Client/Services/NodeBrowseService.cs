using Opc.Ua;
using Opc.Ua.Client;
using Microsoft.Extensions.Logging;
using ISKI.OpcUa.Client.Interfaces;

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

    public List<string> Browse(string nodeId)
    {
        var session = _connection.Session;
        var references = new List<string>();

        if (session == null)
        {
            _logger.LogWarning("Browse çağrısı yapıldı ama oturum yok.");
            return new List<string> { "Hata: OPC oturumu bağlı değil." };
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
                out var results, out var diagnosticInfos
            );

            foreach (var reference in results[0].References)
            {
                var refStr = $"{reference.DisplayName.Text} (NodeId: {reference.NodeId})";
                references.Add(refStr);
            }

            _logger.LogInformation("Browse işlemi tamamlandı. {count} node bulundu.", references.Count);

            if (references.Count == 0)
                references.Add("Geçerli ama alt node bulunamadı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Browse sırasında hata oluştu: {nodeId}", nodeId);
            references.Clear();
            references.Add($"Browse işlemi başarısız: {ex.Message}");
        }

        return references;
    }
}
