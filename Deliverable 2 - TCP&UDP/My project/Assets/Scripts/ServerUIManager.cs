using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ServerUIManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField serverNameInput;
    public TMP_InputField ipInput;
    public TMP_InputField portInput;
    public TMP_Dropdown protocolDropdown;
    public Button startButton;
    public Button stopButton;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI logText;
    public TextMeshProUGUI playersText;

    [Header("Server Components")]
    public TCPServer tcpServer;
    public UDPServer udpServer;

    private IServer activeServer;

    void Awake()
    {
        // 🔧 Forzar opciones limpias del dropdown (evita errores del editor)
        protocolDropdown.ClearOptions();
        protocolDropdown.AddOptions(new List<string> { "TCP", "UDP" });
    }

    void Start()
    {
        // Desactivar Stop al inicio
        stopButton.interactable = false;
        startButton.onClick.AddListener(OnStartClicked);
        stopButton.onClick.AddListener(OnStopClicked);

        SetStatus("Stopped");
        AppendLog("Server Manager listo. Selecciona protocolo y pulsa Start Server.");
    }

    private void OnStartClicked()
    {
        // 🔍 Normalizar selección
        string rawSelection = protocolDropdown.options[protocolDropdown.value].text ?? "";
        string protocol = rawSelection.Trim().ToUpperInvariant();

        // Datos de entrada
        string serverName = string.IsNullOrWhiteSpace(serverNameInput.text) ? "UnityServer" : serverNameInput.text.Trim();
        int port = int.TryParse(portInput.text, out var p) ? p : 5000;

        // 🔌 Detener cualquier servidor activo anterior
        StopActiveServer();

        // Selección de protocolo robusta
        if (protocol.Contains("TCP"))
        {
            activeServer = tcpServer;
            AppendLog("Protocolo seleccionado: TCP");
        }
        else if (protocol.Contains("UDP"))
        {
            activeServer = udpServer;
            AppendLog("Protocolo seleccionado: UDP");
        }
        else
        {
            AppendLog($"⚠ Protocolo desconocido ('{rawSelection}'), usando TCP por defecto.");
            activeServer = tcpServer;
        }

        // Suscribirse a eventos del servidor activo
        SubscribeToServer(activeServer);

        try
        {
            activeServer.StartServer(serverName, port);
            AppendLog($"Servidor iniciado en modo {protocol} — Puerto: {port}");
            SetStatus($"Running ({protocol})");

            startButton.interactable = false;
            stopButton.interactable = true;
        }
        catch (Exception ex)
        {
            AppendLog($"❌ Error al iniciar servidor: {ex.Message}");
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
        if (activeServer != null)
        {
            UnsubscribeFromServer(activeServer);
            activeServer.StopServer();
            AppendLog("Servidor detenido.");
            activeServer = null;
        }
    }

    private void SubscribeToServer(IServer server)
    {
        if (server == null) return;

        server.OnLog += AppendLog;
        server.OnPlayersUpdated += UpdatePlayers;
        server.OnStatusChanged += SetStatus;
    }

    private void UnsubscribeFromServer(IServer server)
    {
        if (server == null) return;

        server.OnLog -= AppendLog;
        server.OnPlayersUpdated -= UpdatePlayers;
        server.OnStatusChanged -= SetStatus;
    }

    private void SetStatus(string status)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            if (statusText != null)
                statusText.text = status;
        });
    }

    private void AppendLog(string message)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            if (logText != null)
                logText.text += $"[{DateTime.Now:HH:mm:ss}] {message}\n";
            Debug.Log(message);
        });
    }

    private void UpdatePlayers(string[] players)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            if (playersText == null) return;

            if (players == null || players.Length == 0)
                playersText.text = "Players: (ninguno)";
            else
                playersText.text = "Players:\n" + string.Join("\n", players);
        });
    }
}
