using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TCPClient : MonoBehaviour
{
    [Header("UI (TextMeshPro)")]
    public TMP_InputField serverIPInput;      // Input_ServerIP
    public TMP_InputField portInput;          // Input_Port
    public TMP_InputField playerNameInput;    // Input_PlayerName
    public TMP_InputField chatMessageInput;   // Input_ChatMessage
    public Button connectButton;              // Button_Connect
    public Button disconnectButton;           // Button_Disconnect
    public Button sendButton;                 // Button_Send
    public TextMeshProUGUI statusText;        // Text_Status
    public TextMeshProUGUI logText;           // Text_Log

    private TcpClient client;
    private NetworkStream stream;
    private CancellationTokenSource cts;
    private Task receiveTask;
    private readonly object logLock = new object();

    void Start()
    {
        connectButton.onClick.AddListener(OnConnectClicked);
        disconnectButton.onClick.AddListener(OnDisconnectClicked);
        sendButton.onClick.AddListener(OnSendClicked);

        disconnectButton.interactable = false;
        sendButton.interactable = false;
        statusText.text = "Disconnected";
        Log("Ready to connect.");
    }

    private void OnConnectClicked()
    {
        string ip = string.IsNullOrWhiteSpace(serverIPInput.text) ? "127.0.0.1" : serverIPInput.text.Trim();
        int port = 5000;
        int.TryParse(portInput.text, out port);

        string playerName = string.IsNullOrWhiteSpace(playerNameInput.text) ? "Player" + UnityEngine.Random.Range(0, 999) : playerNameInput.text.Trim();

        ConnectToServer(ip, port, playerName);
    }

    private void OnDisconnectClicked()
    {
        Disconnect();
    }

    private void OnSendClicked()
    {
        string msg = chatMessageInput.text.Trim();
        if (string.IsNullOrEmpty(msg)) return;
        SendMessageToServer("MSG:" + msg);
        chatMessageInput.text = "";
    }

    public async void ConnectToServer(string ip, int port, string playerName)
    {
        try
        {
            client = new TcpClient();
            await client.ConnectAsync(ip, port);
            stream = client.GetStream();
            cts = new CancellationTokenSource();

            // Enviar nombre al conectar
            SendMessageToServer("NAME:" + playerName);

            // Actualizar UI
            statusText.text = $"Connected to {ip}:{port}";
            connectButton.interactable = false;
            disconnectButton.interactable = true;
            sendButton.interactable = true;

            Log("Connected! Sending player name...");

            // Iniciar lectura asíncrona
            receiveTask = Task.Run(() => ReceiveLoopAsync(cts.Token));
        }
        catch (Exception ex)
        {
            Log("Error connecting: " + ex.Message);
        }
    }

    private async Task ReceiveLoopAsync(CancellationToken token)
    {
        byte[] buffer = new byte[1024];
        try
        {
            while (!token.IsCancellationRequested && client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                if (bytesRead == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                UnityMainThread(() =>
                {
                    Log($"Server: {message}");
                });
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            UnityMainThread(() => Log("Receive error: " + ex.Message));
        }
        finally
        {
            UnityMainThread(Disconnect);
        }
    }

    private void SendMessageToServer(string text)
    {
        if (client == null || !client.Connected) return;
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            stream.Write(data, 0, data.Length);
            Log($"You: {text}");
        }
        catch (Exception ex)
        {
            Log("Error sending: " + ex.Message);
        }
    }

    private void Disconnect()
    {
        try
        {
            cts?.Cancel();
            stream?.Close();
            client?.Close();
        }
        catch { }

        statusText.text = "Disconnected";
        connectButton.interactable = true;
        disconnectButton.interactable = false;
        sendButton.interactable = false;
        Log("Disconnected from server.");
    }

    private void Log(string msg)
    {
        Debug.Log(msg);
        logText.text += $"[{DateTime.Now:HH:mm:ss}] {msg}\n";
    }

    private void UnityMainThread(Action a)
    {
        if (a == null) return;
        // Ejecuta directamente si estamos en el hilo principal
        if (Thread.CurrentThread.ManagedThreadId == 1) a();
        else UnityMainThreadDispatcher.Enqueue(a);
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }
}
