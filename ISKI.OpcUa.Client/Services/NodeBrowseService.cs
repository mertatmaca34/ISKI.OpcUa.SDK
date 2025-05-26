using Opc.Ua;
using Opc.Ua.Client;
using Microsoft.Extensions.Logging;
using ISKI.OpcUa.Client.Interfaces;
using ISKI.OpcUa.Client.Models;

namespace ISKI.OpcUa.Client.Services;

public class NodeBrowseService(ILogger<NodeBrowseService> logger, IConnectionService connection) : INodeBrowseService
{
    public List<NodeBrowseResult> Browse(string nodeId)
    {
        var session = connection.Session;
        var results = new List<NodeBrowseResult>();

        if (session == null)
        {
            logger.LogWarning("Browse çağrısı yapıldı ama oturum yok.");
            return
        [
            new() {
                DisplayName = "Oturum yok",
                NodeId = "N/A",
                NodeClass = "Error"
            }
        ];
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
                [browseDesc],
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

            logger.LogInformation("Browse işlemi tamamlandı. {count} node bulundu.", results.Count);

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
            logger.LogError(ex, "Browse sırasında hata oluştu: {nodeId}", nodeId);
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
