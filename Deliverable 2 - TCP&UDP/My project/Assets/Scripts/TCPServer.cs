using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TCPServer : MonoBehaviour
{
    [Header("UI (TextMeshPro)")]
    public TMP_InputField serverNameInput;   // Input_ServerName (TMP)
    public TMP_InputField portInput;         // Input_Port (TMP)
    public Button startButton;               // Button_StartServer
    public Button stopButton;                // Button_StopServer
    public TextMeshProUGUI statusText;       // Text_Status (TMP)
    public TextMeshProUGUI logText;          // Text_Log (TMP)
    public TextMeshProUGUI playersText;      // Text_Players (TMP)

    private TcpListener listener;
    private CancellationTokenSource cts;
    private readonly ConcurrentDictionary<TcpClient, string> clients = new ConcurrentDictionary<TcpClient, string>();
    private readonly ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();

    void Start()
    {
        startButton.onClick.AddListener(OnStartClicked);
        stopButton.onClick.AddListener(OnStopClicked);
        stopButton.interactable = false;

        Log("Servidor listo. Pulsa Start Server.");
        statusText.text = "Stopped";
    }

    void Update()
    {
        while (mainThreadActions.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }

    private void OnStartClicked()
    {
        if (listener != null)
        {
            Log("Servidor ya iniciado.");
            return;
        }

        int port = 5000;
        if (!int.TryParse(portInput.text, out port)) port = 5000;
        string serverName = string.IsNullOrWhiteSpace(serverNameInput.text) ? "UnityServer" : serverNameInput.text.Trim();

        StartServer(port, serverName);
    }

    private void OnStopClicked()
    {
        StopServer();
    }

    public void StartServer(int port, string serverName)
    {
        try
        {
            cts = new CancellationTokenSource();
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            Enqueue(() =>
            {
                statusText.text = "Running";
                startButton.interactable = false;
                stopButton.interactable = true;
            });

            Log($"Servidor iniciado en puerto {port} (Nombre: {serverName}). Esperando conexiones...");
            Task.Run(() => AcceptLoopAsync(listener, serverName, cts.Token));
        }
        catch (Exception ex)
        {
            Log("Error iniciando servidor: " + ex.Message);
        }
    }

    private async Task AcceptLoopAsync(TcpListener tcpListener, string serverName, CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                TcpClient client = await tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
                string ep = client.Client.RemoteEndPoint?.ToString() ?? "unknown";
                Enqueue(() => Log($"Conexión entrante: {ep}"));
                _ = Task.Run(() => HandleClientAsync(client, serverName, token));
            }
        }
        catch (ObjectDisposedException) { }
        catch (Exception ex)
        {
            Enqueue(() => Log("Error en AcceptLoop: " + ex.Message));
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
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(false);
                if (bytesRead == 0) break;

                string text = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                Enqueue(() => Log($"Recv [{endpoint}]: {text}"));

                if (text.StartsWith("NAME:"))
                {
                    string username = text.Substring(5).Trim();
                    clients[client] = username;
                    Enqueue(UpdatePlayers);
                    byte[] reply = Encoding.UTF8.GetBytes("SERVERNAME:" + serverName);
                    await stream.WriteAsync(reply, 0, reply.Length, token);
                }
                else if (text.StartsWith("MSG:"))
                {
                    string msg = text.Substring(4).Trim();
                    string username = clients.TryGetValue(client, out var n) ? n : endpoint;
                    Enqueue(() => Log($"CHAT {username}: {msg}"));
                    await BroadcastAsync($"MSG_FROM:{username}:{msg}");
                }
            }
        }
        catch (Exception ex)
        {
            Enqueue(() => Log($"Error cliente {endpoint}: {ex.Message}"));
        }
        finally
        {
            clients.TryRemove(client, out _);
            try { client.Close(); } catch { }
            Enqueue(() =>
            {
                UpdatePlayers();
                Log($"Cliente desconectado: {endpoint}");
            });
        }
    }

    private async Task BroadcastAsync(string msg)
    {
        byte[] data = Encoding.UTF8.GetBytes(msg);
        foreach (var c in clients.Keys)
        {
            try
            {
                if (c.Connected)
                    await c.GetStream().WriteAsync(data, 0, data.Length);
            }
            catch { }
        }
    }

    private void UpdatePlayers()
    {
        if (clients.Count == 0) playersText.text = "Players: (ninguno)";
        else playersText.text = "Players:\n" + string.Join("\n", clients.Values);
    }

    private void Log(string s)
    {
        Debug.Log(s);
        Enqueue(() =>
        {
            logText.text += $"[{DateTime.Now:HH:mm:ss}] {s}\n";
        });
    }

    private void Enqueue(Action a) => mainThreadActions.Enqueue(a);

    public void StopServer()
    {
        cts?.Cancel();
        listener?.Stop();
        listener = null;

        foreach (var c in clients.Keys)
            try { c.Close(); } catch { }

        clients.Clear();
        Enqueue(() =>
        {
            statusText.text = "Stopped";
            startButton.interactable = true;
            stopButton.interactable = false;
            Log("Servidor detenido.");
            UpdatePlayers();
        });
    }

    void OnApplicationQuit() => StopServer();
}
