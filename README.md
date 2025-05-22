# ISKI.OpcUa.Client

**ISKI.OpcUa.Client** is a reusable OPC UA client SDK for .NET, designed to provide high-performance and fault-tolerant industrial communication.

## 📦 Install via NuGet

## 🔧 Features

- OPC UA Session management
- Browse nodes
- Read / Write node values
- Auto reconnect support (coming soon)
- Easily extensible with DI support

## 🚀 Usage Example

```csharp
var opc = new OpcUaService();
await opc.ConnectAsync("opc.tcp://192.168.1.5:4840");
var value = await opc.ReadNodeAsync("i=85");