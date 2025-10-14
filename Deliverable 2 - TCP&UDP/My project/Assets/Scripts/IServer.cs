using System;

public interface IServer
{
    void StartServer(string serverName, int port);
    void StopServer();
    event Action<string> OnLog;
    event Action<string[]> OnPlayersUpdated;
    event Action<string> OnStatusChanged;
}
