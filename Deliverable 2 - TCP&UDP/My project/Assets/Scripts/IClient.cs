using System;

public interface IClient
{
    event Action<string> OnLog;
    event Action<string> OnStatusChanged;
    event Action<string> OnChatMessage;

    void Connect(string ip, int port, string playerName);
    void Disconnect();
    void SendChatMessage(string message);
}
