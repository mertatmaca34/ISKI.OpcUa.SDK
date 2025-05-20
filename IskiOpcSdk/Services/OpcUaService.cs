using IskiOpcSdk.Interfaces;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using Opc.Ua.Security.Certificates;
using System.Security.Cryptography;

namespace IskiOpcSdk.Services;

public class OpcUaService : IOpcUaService
{
    private ApplicationConfiguration _config;
    private Session? _session;

    public OpcUaService()
    {
        _config = new ApplicationConfiguration
        {
            ApplicationName = "MyOpcSdkClient",
            ApplicationType = ApplicationType.Client,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = "Directory",
                    StorePath = "Certificates/Own",
                    SubjectName = "CN=MyOpcSdkClient"
                },
                TrustedPeerCertificates = new CertificateTrustList
                {
                    StoreType = "Directory",
                    StorePath = "Certificates/TrustedPeers"
                },
                TrustedIssuerCertificates = new CertificateTrustList
                {
                    StoreType = "Directory",
                    StorePath = "Certificates/Issuers"
                },
                RejectedCertificateStore = new CertificateTrustList
                {
                    StoreType = "Directory",
                    StorePath = "Certificates/Rejected"
                },
                AutoAcceptUntrustedCertificates = true,
                RejectSHA1SignedCertificates = false
            },
            TransportConfigurations = new TransportConfigurationCollection(),
            TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 }
        };

        _config.Validate(ApplicationType.Client).Wait();
    }

    public List<string> Browse(string nodeId)
    {
        var references = new List<string>();

        if (_session == null)
            return new List<string> { "Hata: OPC oturumu bağlı değil." };

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

            _session.Browse(
                null,
                null,
                0,
                new BrowseDescriptionCollection { browseDesc },
                out var results,
                out var diagnosticInfos
            );

            foreach (var reference in results[0].References)
            {
                references.Add($"{reference.DisplayName.Text} (NodeId: {reference.NodeId})");
            }

            if (references.Count == 0)
            {
                references.Add("Geçerli ama alt node bulunamadı.");
            }
        }
        catch (Exception ex)
        {
            references.Clear();
            references.Add($"Browse işlemi başarısız: {ex.Message}");
        }

        return references;
    }


    public async Task DisconnectAsync()
    {
        if (_session != null)
        {
            await _session.CloseAsync();
            _session.Dispose();
            _session = null;
        }
    }
    public async Task ConnectAsync(string endpointUrl)
    {
        var selectedEndpoint = CoreClientUtils.SelectEndpoint(endpointUrl, false);
        var endpointConfiguration = EndpointConfiguration.Create(_config);
        var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);

        _session = await Session.Create(
            _config, endpoint, false, "MySession", 60000, null, null
        );
    }

    public Task<List<string>> FindServersAsync(string discoveryUrl)
    {
        throw new NotImplementedException();
    }

    public async Task<string> ReadNodeAsync(string nodeId)
    {
        if (_session == null || !_session.Connected)
            return "Hata: OPC UA oturumu bağlı değil.";

        try
        {
            var node = new NodeId(nodeId);
            var value = await _session.ReadValueAsync(node);

            if (StatusCode.IsBad(value.StatusCode))
            {
                return $"Tag okunamadı. OPC Durumu: {value.StatusCode}";
            }

            return $"{value.Value})";
        }
        catch (Exception ex)
        {
            return $"Okuma hatası: {ex.Message}";
        }
    }


    public async Task WriteNodeAsync(string nodeId, object value, CancellationToken cancellationToken)
    {
        if (_session == null)
            throw new InvalidOperationException("OPC session not connected");

        var writeValue = new WriteValue
        {
            NodeId = new NodeId(nodeId),
            AttributeId = Attributes.Value,
            Value = new DataValue(new Variant(value))
        };

        var result = await _session.WriteAsync(null, new WriteValueCollection { writeValue }, cancellationToken);

        if (StatusCode.IsBad(result.Results[0]))
            throw new Exception($"OPC Write failed: {result.Results[0]}");
    }

    public async Task<List<string>> FindServersOnLocalNetworkAsync()
    {
        var foundServers = new List<string>();
        var port = 4840; // OPC UA standard discovery port
        var tasks = new List<Task>();

        for (int i = 1; i <= 254; i++)
        {
            string ip = $"192.168.1.{i}";
            string endpoint = $"opc.tcp://{ip}:{port}";

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    using (var client = DiscoveryClient.Create(new Uri(endpoint)))
                    {
                        var servers = await client.FindServersAsync(null);
                        foreach (var server in servers)
                        {
                            lock (foundServers)
                            {
                                foundServers.Add($"{server.ApplicationName.Text} - {endpoint}");
                            }
                        }
                    }
                }
                catch
                {
                    // Cevap vermeyen IP'ler için sessizce geç
                }
            }));
        }

        await Task.WhenAll(tasks);
        return foundServers;
    }
}