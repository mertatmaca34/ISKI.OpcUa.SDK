using System.Collections.Generic;

namespace ISKI.OpcUa.Client.Errors;

public static class ErrorMessages
{
    public const string DefaultCulture = "tr";

    // Turkish messages
    public const string AlreadyConnectedTr = "Zaten bağlı.";
    public const string ConnectionFailedTr = "Bağlantı kurulamadı.";
    public const string SessionNotConnectedTr = "OPC UA oturumu bağlı değil.";
    public const string DisconnectFailedTr = "Bağlantı kesilemedi.";
    public const string ReadFailedTr = "Okuma işlemi başarısız.";
    public const string WriteFailedTr = "Yazma işlemi başarısız.";
    public const string BrowseFailedTr = "Browse işlemi hatalı.";
    public const string DiscoveryFailedTr = "Sunucu keşfi hatası.";
    public const string NodeStatusInvalidTr = "OPC veri durumu geçersiz.";
    public const string UnknownExceptionTr = "Beklenmeyen hata oluştu.";

    // English messages
    public const string AlreadyConnectedEn = "Already connected.";
    public const string ConnectionFailedEn = "Failed to connect.";
    public const string SessionNotConnectedEn = "OPC UA session is not connected.";
    public const string DisconnectFailedEn = "Failed to disconnect.";
    public const string ReadFailedEn = "Read operation failed.";
    public const string WriteFailedEn = "Write operation failed.";
    public const string BrowseFailedEn = "Browse operation failed.";
    public const string DiscoveryFailedEn = "Server discovery failed.";
    public const string NodeStatusInvalidEn = "Invalid OPC data status.";
    public const string UnknownExceptionEn = "Unexpected error occurred.";

    private static readonly Dictionary<string, IReadOnlyDictionary<ErrorCode, string>> _localized
        = new()
    {
        ["tr"] = new Dictionary<ErrorCode, string>
        {
            { ErrorCode.AlreadyConnected, AlreadyConnectedTr },
            { ErrorCode.ConnectionFailed, ConnectionFailedTr },
            { ErrorCode.SessionNotConnected, SessionNotConnectedTr },
            { ErrorCode.DisconnectFailed, DisconnectFailedTr },
            { ErrorCode.ReadFailed, ReadFailedTr },
            { ErrorCode.WriteFailed, WriteFailedTr },
            { ErrorCode.BrowseFailed, BrowseFailedTr },
            { ErrorCode.DiscoveryFailed, DiscoveryFailedTr },
            { ErrorCode.NodeStatusInvalid, NodeStatusInvalidTr },
            { ErrorCode.UnknownException, UnknownExceptionTr }
        },
        ["en"] = new Dictionary<ErrorCode, string>
        {
            { ErrorCode.AlreadyConnected, AlreadyConnectedEn },
            { ErrorCode.ConnectionFailed, ConnectionFailedEn },
            { ErrorCode.SessionNotConnected, SessionNotConnectedEn },
            { ErrorCode.DisconnectFailed, DisconnectFailedEn },
            { ErrorCode.ReadFailed, ReadFailedEn },
            { ErrorCode.WriteFailed, WriteFailedEn },
            { ErrorCode.BrowseFailed, BrowseFailedEn },
            { ErrorCode.DiscoveryFailed, DiscoveryFailedEn },
            { ErrorCode.NodeStatusInvalid, NodeStatusInvalidEn },
            { ErrorCode.UnknownException, UnknownExceptionEn }
        }
    };

    public static string GetMessage(ErrorCode code, string culture = DefaultCulture)
    {
        if (_localized.TryGetValue(culture, out var dict) && dict.TryGetValue(code, out var message))
            return message;

        if (_localized[DefaultCulture].TryGetValue(code, out var defaultMsg))
            return defaultMsg;

        return _localized[DefaultCulture][ErrorCode.UnknownException];
    }
}
