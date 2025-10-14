using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using UnityEngine;
using System.Linq;

public class UDPServer : MonoBehaviour, IServer
{
    private UdpClient udpServer;
    private CancellationTokenSource cts;
    private readonly ConcurrentDictionary<string, string> clients = new();

    public event Action<string> OnLog;
    public event Action<string[]> OnPlayersUpdated;
    public event Action<string> OnStatusChanged;

    private int serverPort;
    private string serverName;

    private readonly ConcurrentQueue<Action> mainActions = new();

    void Update()
    {
        while (mainActions.TryDequeue(out var a)) a?.Invoke();
    }

    private void Enqueue(Action a) => mainActions.Enqueue(a);

    public void StartServer(string serverName, int port)
    {
        if (udpServer != null)
        {
            Log("UDP Server already running");
            return;
        }

        this.serverName = string.IsNullOrWhiteSpace(serverName) ? "UDPServer" : serverName;
        this.serverPort = port;

        Task.Run(() =>
        {
            try
            {
                udpServer = new UdpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort));
                cts = new CancellationTokenSource();

                SetStatus("Running (UDP)");
                Log($"UDP Server started at 127.0.0.1:{serverPort} (Name: {this.serverName})");

                ReceiveLoop(cts.Token);
            }
            catch (Exception ex)
            {
                Log("UDPServer start error: " + ex.Message);
                SetStatus("Stopped");
            }
        });
    }

    private async void ReceiveLoop(CancellationToken token)
    {
        IPEndPoint remoteEP = new(IPAddress.Any, 0);
        try
        {
            while (!token.IsCancellationRequested)
            {
                UdpReceiveResult result = await udpServer.ReceiveAsync();
                string msg = Encoding.UTF8.GetString(result.Buffer).Trim();
                string sender = result.RemoteEndPoint.ToString();
                Log($"Recv [{sender}]: {msg}");

                if (msg.StartsWith("NAME:"))
                {
                    string username = msg.Substring(5).Trim();
                    clients[sender] = username;
                    UpdatePlayers();

                    byte[] response = Encoding.UTF8.GetBytes("SERVERNAME:" + serverName);
                    await udpServer.SendAsync(response, response.Length, result.RemoteEndPoint);
                }
                else if (msg.StartsWith("MSG:"))
                {
                    string text = msg.Substring(4);
                    string username = clients.TryGetValue(sender, out var n) ? n : sender;
                    Log($"{username}: {text}");
                    BroadcastServerMessage($"{username}: {text}");
                }
            }
        }
        catch (Exception ex)
        {
            if (!token.IsCancellationRequested)
                Log("UDPServer receive error: " + ex.Message);
        }
    }

    public async void BroadcastServerMessage(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes($"MSG_FROM:SERVER:{message}");
            foreach (var epStr in clients.Keys)
            {
                var epParts = epStr.Split(':');
                var ep = new IPEndPoint(IPAddress.Parse(epParts[0]), int.Parse(epParts[1]));
                await udpServer.SendAsync(data, data.Length, ep);
            }
        }
        catch (Exception ex) { Log("Error sending server message: " + ex.Message); }
    }

    public void StopServer()
    {
        try
        {
            cts?.Cancel();
            udpServer?.Close();
            udpServer = null;

            clients.Clear();
            SetStatus("Stopped");
            UpdatePlayers();
            Log("UDP Server stopped");
        }
        catch (Exception ex)
        {
            Log("UDPServer stop error: " + ex.Message);
        }
    }

    #region Helpers
    private void Log(string msg) => UnityMainThreadDispatcher.Enqueue(() => OnLog?.Invoke(msg));
    private void UpdatePlayers() => UnityMainThreadDispatcher.Enqueue(() =>
        OnPlayersUpdated?.Invoke(clients.Values.Count == 0 ? Array.Empty<string>() : clients.Values.ToArray())
    );
    private void SetStatus(string status) => UnityMainThreadDispatcher.Enqueue(() => OnStatusChanged?.Invoke(status));
    #endregion

    void OnApplicationQuit() => StopServer();
}
