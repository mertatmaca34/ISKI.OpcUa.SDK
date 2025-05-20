using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskiOpcSdk.Interfaces;

public interface IOpcUaService
{
    Task ConnectAsync(string endpointUrl);
    Task<string?> ReadNodeAsync(string nodeId);
    Task WriteNodeAsync(string nodeId, object value, CancellationToken cancellationToken);
    List<string> Browse(string nodeId);
    Task<List<string>> FindServersOnLocalNetworkAsync();
}