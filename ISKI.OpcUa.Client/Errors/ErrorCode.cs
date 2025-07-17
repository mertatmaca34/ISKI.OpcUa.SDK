namespace ISKI.OpcUa.Client.Errors;

public enum ErrorCode
{
    None = 0,
    AlreadyConnected = 100,
    ConnectionFailed = 101,
    SessionNotConnected = 102,
    DisconnectFailed = 103,
    ReadFailed = 104,
    WriteFailed = 105,
    BrowseFailed = 106,
    DiscoveryFailed = 107,
    NodeStatusInvalid = 108,
    UnknownException = 500
}
