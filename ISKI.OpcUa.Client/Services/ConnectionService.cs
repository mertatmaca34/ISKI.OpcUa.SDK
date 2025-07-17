using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System.IO;
using Microsoft.Extensions.Logging;
using ISKI.OpcUa.Client.Exceptions;
using ISKI.OpcUa.Client.Errors;
using ISKI.OpcUa.Client.Interfaces;

namespace ISKI.OpcUa.Client.Services;

public class ConnectionService : IConnectionService
{
    private readonly ILogger<ConnectionService> _logger;
    private Session? _session;
    private readonly ApplicationConfiguration _config;

    public Session? Session => _session;

    public ConnectionService(ILogger<ConnectionService> logger)
    {
        _logger = logger;

        _config = new ApplicationConfiguration
        {
            ApplicationName = "ISKI.OpcUa.Client",
            ApplicationType = ApplicationType.Client,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = "Directory",
                    StorePath = "Certificates/Own",
                    SubjectName = "CN=ISKI.OpcUa.Client"
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
            TransportConfigurations = [],
            TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 }
        };

        var certPaths = new[]
        {
            _config.SecurityConfiguration.ApplicationCertificate.StorePath,
            _config.SecurityConfiguration.TrustedPeerCertificates.StorePath,
            _config.SecurityConfiguration.TrustedIssuerCertificates.StorePath,
            _config.SecurityConfiguration.RejectedCertificateStore.StorePath
        };

        foreach (var path in certPaths)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        _config.Validate(ApplicationType.Client).Wait();
        _logger.LogInformation("OPC UA ApplicationConfiguration oluşturuldu.");
    }

    public async Task ConnectAsync(string endpointUrl)
    {
        try
        {
            _logger.LogInformation("OPC bağlantı kuruluyor: {endpoint}", endpointUrl);

            var selectedEndpoint = CoreClientUtils.SelectEndpoint(_config, endpointUrl, false);
            var endpointConfiguration = EndpointConfiguration.Create(_config);
            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);

            _session = await Session.Create(_config, endpoint, false, "ISKI.Session", 60000, null, null);

            _logger.LogInformation("Bağlantı başarıyla kuruldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OPC UA bağlantı kurulamadı.");
            throw new OpcServiceException(ErrorMessages.GetMessage(ErrorCode.ConnectionFailed), ex);
        }
    }

    public async Task DisconnectAsync()
    {
        if (_session != null)
        {
            _logger.LogInformation("OPC bağlantısı kapatılıyor...");

            await _session.CloseAsync();
            _session.Dispose();
            _session = null;

            _logger.LogInformation("Bağlantı kapatıldı.");
        }
        else
        {
            _logger.LogWarning("Disconnect çağrıldı ancak bağlantı yok.");
        }
    }
}
