# ISKI.OpcUa.Client

**ISKI.OpcUa.Client** is a lightweight and extensible OPC UA client SDK for .NET applications. Built for industrial-grade reliability, it enables seamless integration with OPC UA-enabled devices, supporting real-time data communication and flexible session handling.

## 📆 NuGet Installation

```bash
dotnet add package ISKI.OpcUa.Client
```

> Supports **.NET 8**.

---

## 🔧 Features

* ✅ Robust OPC UA Session management
* ✅ Node browsing support
* ✅ Read & Write operations on data nodes
* ✅ Certificate-based secure connections
* ✅ Dependency Injection (DI) friendly design
* 🕓 Auto-reconnect logic *(coming soon)*

---

## 🚀 Quick Start

```csharp
using ISKI.OpcUa.Client.Services;

var opcClient = new OpcUaService(); // or inject via DI
await opcClient.ConnectAsync("opc.tcp://192.168.1.5:4840");

string value = await opcClient.ReadNodeAsync("ns=2;s=Temperature.Tag1");
Console.WriteLine($"Current Value: {value}");
```

---

## 📁 Project Structure

```
ISKI.OpcUa.Client/
├── Interfaces/
├── Services/
├── Models/
├── Certificates/
├── Configuration/
├── Utils/
├── Constants/
├── Enums/
├── Exceptions/
```

---

## 📃 License

This project is licensed under the [MIT License](./LICENSE).

---

## 👨‍💻 Author

Developed and maintained by **Mert Atmaca**
GitHub: [@mertatmaca](https://github.com/mertatmaca)
Company: **ISKI**
