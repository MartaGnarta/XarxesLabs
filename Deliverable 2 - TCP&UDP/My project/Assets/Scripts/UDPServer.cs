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
                udpServer = new UdpClient(new IPEndPoint(IPAddress.Any, serverPort));
                cts = new CancellationTokenSource();

                string localIP = GetLocalIP();
                SetStatus("Running (UDP)");
                Log($"UDP Server started at {localIP}:{serverPort} (Name: {this.serverName})");

                ReceiveLoop(cts.Token);
            }
            catch (Exception ex)
            {
                Log("UDPServer start error: " + ex.Message);
                SetStatus("Stopped");
            }
        });
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
                }
            }
        }
        catch (Exception ex)
        {
            if (!token.IsCancellationRequested)
                Log("UDPServer receive error: " + ex.Message);
        }
    }

    #region Helpers
    private void Log(string msg) => UnityMainThreadDispatcher.Enqueue(() => OnLog?.Invoke(msg));
    private void UpdatePlayers() => UnityMainThreadDispatcher.Enqueue(() =>
        OnPlayersUpdated?.Invoke(clients.Values.Count == 0 ? Array.Empty<string>() : clients.Values.ToArray())
    );
    private void SetStatus(string status) => UnityMainThreadDispatcher.Enqueue(() => OnStatusChanged?.Invoke(status));

    private string GetLocalIP()
    {
        string localIP = "127.0.0.1";
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
        }
        catch { }
        return localIP;
    }
    #endregion

    void OnApplicationQuit() => StopServer();
}
