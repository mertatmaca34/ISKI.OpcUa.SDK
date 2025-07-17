using Opc.Ua;
using Opc.Ua.Client;
using Microsoft.Extensions.Logging;
using ISKI.OpcUa.Client.Interfaces;
using ISKI.OpcUa.Client.Models;
using ISKI.OpcUa.Client.Errors;

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
                DisplayName = ErrorMessages.GetMessage(ErrorCode.SessionNotConnected),
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
                DisplayName = $"{ErrorMessages.GetMessage(ErrorCode.BrowseFailed)} {ex.Message}",
                NodeId = nodeId,
                NodeClass = "Exception"
            });
        }

        return results;
    }

    public NodeTreeResult BrowseTree(string nodeId)
    {
        var session = connection.Session;

        if (session == null)
        {
            logger.LogWarning("BrowseTree çağrısı yapıldı ama oturum yok.");
            return new NodeTreeResult
            {
                DisplayName = ErrorMessages.GetMessage(ErrorCode.SessionNotConnected),
                NodeId = "N/A",
                NodeClass = "Error",
                Children = []
            };
        }

        try
        {
            var tree = BrowseRecursive(session, new NodeId(nodeId));
            logger.LogInformation("BrowseTree işlemi tamamlandı.");
            return tree;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "BrowseTree sırasında hata oluştu: {nodeId}", nodeId);
            return new NodeTreeResult
            {
                DisplayName = $"{ErrorMessages.GetMessage(ErrorCode.BrowseFailed)} {ex.Message}",
                NodeId = nodeId,
                NodeClass = "Exception",
                Children = []
            };
        }
    }

    private NodeTreeResult BrowseRecursive(Session session, NodeId nodeId)
    {
        var node = session.ReadNode(nodeId);
        var current = new NodeTreeResult
        {
            DisplayName = node.DisplayName.Text,
            NodeId = nodeId.ToString(),
            NodeClass = node.NodeClass.ToString(),
            Children = []
        };

        var browseDesc = new BrowseDescription
        {
            NodeId = nodeId,
            BrowseDirection = BrowseDirection.Forward,
            ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
            IncludeSubtypes = true,
            NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable),
            ResultMask = (uint)BrowseResultMask.All
        };

        session.Browse(null, null, 0, [browseDesc], out var browseResults, out _);

        foreach (var reference in browseResults[0].References)
        {
            var childId = ExpandedNodeId.ToNodeId(reference.NodeId, session.NamespaceUris);
            current.Children.Add(BrowseRecursive(session, childId));
        }

        return current;
    }
}
