using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UDPClient : MonoBehaviour, IClient
{
    public event Action<string> OnLog;
    public event Action<string> OnStatusChanged;
    public event Action<string> OnChatMessage;

    private UdpClient udp;
    private IPEndPoint serverEndPoint;
    private CancellationTokenSource cts;
    private string playerName;

    public async void Connect(string ip, int port, string playerName)
    {
        try
        {
            this.playerName = playerName;
            udp = new UdpClient();
            serverEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            cts = new CancellationTokenSource();

            OnLog?.Invoke($"Conectado a {ip}:{port} (UDP)");
            OnStatusChanged?.Invoke("Connected (UDP)");

            // Enviar nombre
            await SendRawAsync($"NAME:{playerName}");

            _ = Task.Run(() => ListenLoop(cts.Token));
        }
        catch (Exception ex)
        {
            OnLog?.Invoke($"❌ Error al conectar (UDP): {ex.Message}");
            OnStatusChanged?.Invoke("Disconnected");
        }
    }

    private async Task ListenLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var result = await udp.ReceiveAsync();
                string msg = Encoding.UTF8.GetString(result.Buffer).Trim();

                UnityMainThreadDispatcher.Enqueue(() => OnLog?.Invoke($"Server: {msg}"));

                if (msg.StartsWith("MSG_FROM:"))
                {
                    string content = msg.Substring(9);
                    UnityMainThreadDispatcher.Enqueue(() => OnChatMessage?.Invoke(content));
                }
            }
            catch (Exception ex)
            {
                if (!token.IsCancellationRequested)
                    UnityMainThreadDispatcher.Enqueue(() => OnLog?.Invoke($"UDP Error: {ex.Message}"));
            }
        }
    }

    public async void SendChatMessage(string message)
    {
        await SendRawAsync($"MSG:{message}");
    }

    private async Task SendRawAsync(string msg)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            await udp.SendAsync(data, data.Length, serverEndPoint);
        }
        catch (Exception ex)
        {
            OnLog?.Invoke($"Error enviando mensaje UDP: {ex.Message}");
        }
    }

    public void Disconnect()
    {
        try
        {
            cts?.Cancel();
            udp?.Close();
            OnLog?.Invoke("Desconectado del servidor UDP.");
        }
        catch (Exception ex)
        {
            OnLog?.Invoke($"Error al desconectar UDP: {ex.Message}");
        }

        OnStatusChanged?.Invoke("Disconnected");
    }
}
