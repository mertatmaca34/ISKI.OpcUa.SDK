﻿# ISKI.OpcUa.Client

**ISKI.OpcUa.Client** is a modern, production-ready OPC UA client SDK for .NET.  
It provides a clean, extensible, and testable interface for secure and efficient industrial communication.

---

## 📦 Installation

Install via NuGet:

```bash
Install-Package ISKI.OpcUa.Client
```

Or via .NET CLI:

```bash
dotnet add package ISKI.OpcUa.Client
```

---

## 🔧 Features

- ✅ OPC UA Session management with certificate support  
- ✅ Read & Write to node values  
- ✅ Browse server nodes (with metadata)  
- ✅ Discover servers on local network with configurable range
- ✅ Structured, consistent response models with `ConnectionResult<T>`  
- ✅ Full integration with ASP.NET Core Dependency Injection  
- ✅ Built-in logging support with `ILogger`
- ✅ Automatically creates certificate directories if missing

---

## 🚀 Quick Example

### Register Services in Program.cs

Add the library via the extension method to register all required services.

```csharp
using ISKI.OpcUa.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddIskiOpcUaClient();
```

### Connect to a server:

```csharp
await opcUaService.ConnectAsync("opc.tcp://192.168.1.5:4840");
```

### Read a node:

```csharp
ConnectionResult<NodeReadResult> result = await opcUaService.ReadNodeAsync("ns=2;s=MyNodeId");

if (result.Success)
{
    Console.WriteLine($"Value: {result.Data.Value}");
}
else
{
    Console.WriteLine($"Error: {result.Message}");
}
```

### Browse nodes:

```csharp
List<NodeBrowseResult> nodes = opcUaService.Browse("i=85");
foreach (var node in nodes)
{
    Console.WriteLine($"{node.DisplayName} [{node.NodeClass}] - {node.NodeId}");
}
```

### Discover servers:

```csharp
List<string> servers = await opcUaService.FindServersOnLocalNetworkAsync("192.168.100", 4840);
foreach (var server in servers)
{
    Console.WriteLine(server);
}
```

Network prefix and port are configurable to match your local setup.

### Certificate Directories

`ConnectionService` stores certificates under the `Certificates` folder of the
project. Subdirectories for `Own`, `TrustedPeers`, `Issuers` and `Rejected` are
created automatically if they do not already exist at runtime.

---

## 📘 Data Models

### `ConnectionResult<T>`

Generic wrapper model for all results.

```csharp
public class ConnectionResult<T>
{
    public bool Success => ServerStatus == "Good";
    public string ServerStatus { get; set; } = "Unknown";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Message { get; set; }
    public T Data { get; set; } = default!;
}
```

### `NodeReadResult`

```csharp
public class NodeReadResult
{
    public string NodeId { get; set; }
    public object? Value { get; set; }
    public string NodeStatus { get; set; }
}
```

### `NodeBrowseResult`

```csharp
public class NodeBrowseResult
{
    public string DisplayName { get; set; }
    public string NodeId { get; set; }
    public string NodeClass { get; set; }
}
```

---

## 🧪 Example API Integration

### Controller

```csharp
[HttpGet("read")]
public async Task<ActionResult<ConnectionResult<NodeReadResult>>> ReadNode([FromQuery] string nodeId)
{
    var result = await opcUaService.ReadNodeAsync(nodeId);
    return result.Success ? Ok(result) : BadRequest(result);
}
```

---

## 🛡️ License

MIT

---

## 🔗 Repository

[GitHub](https://github.com/mertatmaca34/ISKI.OpcUa.SDK)
