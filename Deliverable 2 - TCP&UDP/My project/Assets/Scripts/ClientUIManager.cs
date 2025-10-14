using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.Http;

public class ClientUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField ipInput;
    public TMP_InputField portInput;
    public TMP_InputField playerNameInput;
    public TMP_Dropdown protocolDropdown;
    public Button joinButton;
    public Button disconnectButton;
    public TMP_InputField chatInput;
    public Button sendButton;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI logText;

    [Header("Client Components")]
    public TCPClient tcpClient;
    public UDPClient udpClient;

    private IClient activeClient;

    void Awake()
    {
        protocolDropdown.ClearOptions();
        protocolDropdown.AddOptions(new List<string> { "TCP", "UDP" });
    }

    void Start()
    {
        disconnectButton.interactable = false;
        sendButton.interactable = false;

        joinButton.onClick.AddListener(OnJoinClicked);
        disconnectButton.onClick.AddListener(OnDisconnectClicked);
        sendButton.onClick.AddListener(OnSendClicked);

        SetStatus("Disconnected");
        AppendLog("Cliente listo. Introduce datos del servidor.");
    }

    private void OnJoinClicked()
    {
        string raw = protocolDropdown.options[protocolDropdown.value].text ?? "";
        string protocol = raw.Trim().ToUpperInvariant();
        string ip = ipInput.text.Trim();
        int port = int.TryParse(portInput.text, out var p) ? p : 5000;
        string name = playerNameInput.text.Trim();

        if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(name))
        {
            AppendLog("⚠ Debes introducir IP y nombre de jugador.");
            return;
        }

        StopActiveClient();

        if (protocol.Contains("TCP"))
        {
            activeClient = tcpClient;
            AppendLog("Usando cliente TCP.");
        }
        else
        {
            activeClient = udpClient;
            AppendLog("Usando cliente UDP.");
        }

        SubscribeToClient(activeClient);
        activeClient.Connect(ip, port, name);

        joinButton.interactable = false;
        disconnectButton.interactable = true;
        sendButton.interactable = true;
    }

    private void OnDisconnectClicked()
    {
        StopActiveClient();
        SetStatus("Disconnected");
        joinButton.interactable = true;
        disconnectButton.interactable = false;
        sendButton.interactable = false;
    }

    private void OnSendClicked()
    {
        if (activeClient == null) return;
        string msg = chatInput.text.Trim();
        if (string.IsNullOrEmpty(msg)) return;

        activeClient.SendChatMessage(msg);
        AppendLog($"Yo: {msg}");
        chatInput.text = "";
    }

    private void StopActiveClient()
    {
        if (activeClient != null)
        {
            UnsubscribeFromClient(activeClient);
            activeClient.Disconnect();
            activeClient = null;
        }
    }

    private void SubscribeToClient(IClient client)
    {
        client.OnLog += AppendLog;
        client.OnStatusChanged += SetStatus;
        client.OnChatMessage += HandleChatMessage;
    }

    private void UnsubscribeFromClient(IClient client)
    {
        client.OnLog -= AppendLog;
        client.OnStatusChanged -= SetStatus;
        client.OnChatMessage -= HandleChatMessage;
    }

    private void SetStatus(string status)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            statusText.text = status;
        });
    }

    private void AppendLog(string message)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            logText.text += $"[{DateTime.Now:HH:mm:ss}] {message}\n";
        });
    }

    private void HandleChatMessage(string message)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            logText.text += $"{message}\n";
        });
    }
}
