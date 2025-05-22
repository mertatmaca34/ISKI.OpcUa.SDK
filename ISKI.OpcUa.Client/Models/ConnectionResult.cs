using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISKI.OpcUa.Client.Models;

public class ConnectionResult<T>
{
    public bool Success => ServerStatus == "Good";
    public string ServerStatus { get; set; } = "Unknown";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Message { get; set; }
    public T Data { get; set; } = default!;
}
