using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ServerUIManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField serverNameInput;
    public TMP_InputField portInput;
    public TMP_Dropdown protocolDropdown;
    public Button startButton;
    public Button stopButton;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI logText;
    public TextMeshProUGUI playersText;

    [Header("Server Chat UI")]
    public TMP_InputField serverMessageInput; 
    public Button sendMessageButton;          

    [Header("Server Components")]
    public TCPServer tcpServer;
    public UDPServer udpServer;

    private IServer activeServer;

    private Action<string> logHandler;

    void Awake()
    {
        protocolDropdown.ClearOptions();
        protocolDropdown.AddOptions(new List<string> { "TCP", "UDP" });
    }

    void Start()
    {
        startButton.onClick.AddListener(OnStartClicked);
        stopButton.onClick.AddListener(OnStopClicked);
        sendMessageButton.onClick.AddListener(OnServerSendClicked);

        stopButton.interactable = false;
        SetStatus("Stopped");
        AppendLog("Server Manager ready. Select protocol and press Start Server.");
    }

    private void OnStartClicked()
    {
        string protocol = protocolDropdown.options[protocolDropdown.value].text.Trim().ToUpperInvariant();
        string serverName = string.IsNullOrWhiteSpace(serverNameInput.text) ? "UnityServer" : serverNameInput.text.Trim();
        int port = int.TryParse(portInput.text, out var p) ? p : 5000;

        StopActiveServer();

        if (protocol.Contains("TCP")) activeServer = tcpServer;
        else if (protocol.Contains("UDP")) activeServer = udpServer;
        else
        {
            AppendLog($"⚠ Unknown protocol '{protocol}', using TCP by default.");
            activeServer = tcpServer;
        }

        SubscribeToServer(activeServer);

        try
        {
            activeServer.StartServer(serverName, port);
            SetStatus($"Running ({protocol})");
            startButton.interactable = false;
            stopButton.interactable = true;
            AppendLog($"Server started in {protocol} mode — Port: {port}");
        }
        catch (Exception ex)
        {
            AppendLog($"❌ Error starting server: {ex.Message}");
            SetStatus("Error");
        }
    }

    private void OnStopClicked()
    {
        StopActiveServer();
        SetStatus("Stopped");
        startButton.interactable = true;
        stopButton.interactable = false;
    }

    private void StopActiveServer()
    {
        if (activeServer == null) return;

        UnsubscribeFromServer(activeServer);
        activeServer.StopServer();
        AppendLog("Server stopped.");
        activeServer = null;
    }

    private void SubscribeToServer(IServer server)
    {
        if (server == null) return;

        logHandler = msg => AppendLogWithColor(msg, isServer: msg.StartsWith("(Server):"));
        server.OnLog += logHandler;
        server.OnPlayersUpdated += UpdatePlayers;
        server.OnStatusChanged += SetStatus;
    }

    private void UnsubscribeFromServer(IServer server)
    {
        if (server == null || logHandler == null) return;

        server.OnLog -= logHandler;
        server.OnPlayersUpdated -= UpdatePlayers;
        server.OnStatusChanged -= SetStatus;
        logHandler = null;
    }

    private void OnServerSendClicked()
    {
        if (activeServer == null)
        {
            AppendLog("⚠ No active server to send messages.");
            return;
        }

        string msg = serverMessageInput.text.Trim();
        if (string.IsNullOrEmpty(msg)) return;

        serverMessageInput.text = "";

        AppendLogWithColor($"(Server): {msg}", isServer: true);

        activeServer.BroadcastServerMessage(msg);
    }

    private void AppendLog(string message)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            if (logText != null)
            {
                logText.text += $"[{DateTime.Now:HH:mm:ss}] {message}\n";
            }
            Debug.Log(message);
        });
    }

    private void AppendLogWithColor(string message, bool isServer)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            if (logText != null)
            {
                string color = isServer ? "red" : "white";
                logText.text += $"<color={color}>[{DateTime.Now:HH:mm:ss}] {message}</color>\n";
            }
            Debug.Log(message);
        });
    }

    private void SetStatus(string status)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            if (statusText != null) statusText.text = status;
        });
    }

    private void UpdatePlayers(string[] players)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            if (playersText == null) return;

            if (players == null || players.Length == 0)
                playersText.text = "Players: (none)";
            else
                playersText.text = "Players:\n" + string.Join("\n", players);
        });
    }
}
