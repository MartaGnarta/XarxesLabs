using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using UnityEngine;
using System.Linq;

public class TCPServer : MonoBehaviour, IServer
{
    private TcpListener listener;
    private CancellationTokenSource cts;
    private readonly ConcurrentDictionary<TcpClient, string> clients = new();

    public event Action<string> OnLog;
    public event Action<string[]> OnPlayersUpdated;
    public event Action<string> OnStatusChanged;

    public void StartServer(string serverName, int port)
    {
        if (listener != null)
        {
            Log("TCP Server already running");
            return;
        }

        Task.Run(() =>
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();

                cts = new CancellationTokenSource();
                SetStatus("Running (TCP)");

                string localIP = GetLocalIP();
                Log($"TCP Server started at {localIP}:{port} (Name: {serverName})");

                AcceptClientsLoop(serverName, cts.Token);
            }
            catch (Exception ex)
            {
                Log("TCPServer start error: " + ex.Message);
                SetStatus("Stopped");
            }
        });
    }

    private async void AcceptClientsLoop(string serverName, CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                string endpoint = client.Client.RemoteEndPoint?.ToString() ?? "unknown";
                Log($"Client connected: {endpoint}");
                _ = HandleClientAsync(client, serverName, token);
            }
        }
        catch (ObjectDisposedException) { }
        catch (Exception ex)
        {
            Log("TCP Accept loop error: " + ex.Message);
            StopServer();
        }
    }

    private async Task HandleClientAsync(TcpClient client, string serverName, CancellationToken token)
    {
        string endpoint = client.Client.RemoteEndPoint?.ToString() ?? "unknown";
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (!token.IsCancellationRequested)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                if (bytesRead == 0) break;

                string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                Log($"Recv [{endpoint}]: {msg}");

                if (msg.StartsWith("NAME:"))
                {
                    string username = msg.Substring(5).Trim();
                    clients[client] = username;
                    UpdatePlayers();
                    byte[] reply = Encoding.UTF8.GetBytes("SERVERNAME:" + serverName);
                    await stream.WriteAsync(reply, 0, reply.Length, token);
                }
                else if (msg.StartsWith("MSG:"))
                {
                    string message = msg.Substring(4).Trim();
                    string username = clients.TryGetValue(client, out var n) ? n : endpoint;
                    Log($"{username}: {message}");
                }
            }
        }
        catch (Exception ex)
        {
            Log($"Client error [{endpoint}]: {ex.Message}");
        }
        finally
        {
            clients.TryRemove(client, out _);
            try { client.Close(); } catch { }
            UpdatePlayers();
            Log($"Client disconnected: {endpoint}");
        }
    }

    public void StopServer()
    {
        try
        {
            cts?.Cancel();
            listener?.Stop();
            listener = null;

            foreach (var c in clients.Keys) try { c.Close(); } catch { }
            clients.Clear();

            SetStatus("Stopped");
            UpdatePlayers();
            Log("TCP Server stopped");
        }
        catch (Exception ex)
        {
            Log("TCPServer stop error: " + ex.Message);
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
}
