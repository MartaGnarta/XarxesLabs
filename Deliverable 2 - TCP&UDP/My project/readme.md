# Unity Network Chat Project

This project is a **network chat system** built in **Unity**, allowing multiple clients to connect to a server via **TCP** or **UDP**. It is designed for real-time communication, with a clean, professional UI and modular server/client architecture.

---

## Features

- **Dual Protocol Support**: TCP and UDP servers, selectable from the server UI.
- **Real-time Chat**: Messages are sent and received in real-time between clients and the server.
- **Server & Client Logs**: Full logging system for server and client activity.
- **Player Management**: Tracks connected players and updates UI in real-time.
- **Server-Side Messaging**: The server can send messages to all clients, which appear in the clients’ chat logs.
- **Colored Server Messages**: Messages from the server appear in **red** in the chat log for clarity.
- **Cross-Resolution UI**: Canvas and UI elements are anchored and scaled to adapt to any resolution.
- **Windowed Mode Build**: Default window size is 800x600 pixels (4:3 aspect ratio).

---

## Project Structure

### Server Scene

**UI Elements:**

- Server name input
- IP and port input (default: `127.0.0.1`)
- Protocol dropdown (TCP/UDP)
- Start/Stop server buttons
- Players list
- Server chat input and send button
- Log scroll view

**Scripts:**

- `ServerUIManager`: Handles server UI, starts/stops server, sends server messages.
- `TCPServer` / `UDPServer`: Implement server functionality for TCP and UDP, manage client connections, broadcast messages.
- `IServer`: Interface used to unify TCP and UDP server methods/events.
- `UnityMainThreadDispatcher`: Ensures thread-safe updates to Unity UI from async tasks.

### Client Scene

**UI Elements:**

- Server IP input
- Port input
- Player name input
- Join / Disconnect buttons
- Player status text
- Chat scroll view with input field
- Send message button

**Scripts:**

- `ClientUIManager`: Handles UI for connecting/disconnecting, sending chat messages, and updating logs.
- `TCPClient` / `UDPClient`: Handle client connections and messaging with the server.
- `IServerClient`: Interface to unify TCP/UDP client methods/events.
- `UnityMainThreadDispatcher`: Ensures thread-safe updates to Unity UI from async tasks.

---

## How It Works

1. **Server Initialization**
   - Select TCP or UDP protocol.
   - Enter server name and port (IP defaults to `127.0.0.1`).
   - Click **Start Server**.
   - Server logs and status update in real-time.
   
2. **Client Connection**
   - Enter server IP (`127.0.0.1`) and port.
   - Enter player name.
   - Click **Join Server**.
   - Client logs display connection messages, server messages, and chat messages.
   
3. **Chat Messaging**
   - Clients can send messages that are broadcasted to all other clients.
   - Server can send messages using its own input field, appearing in **red** in clients’ logs.
   - Logs update in real-time without blocking the UI.

4. **Disconnecting / Stopping**
   - Server can stop, disconnecting all clients.
   - Clients can disconnect, updating the player list and logs.

---

## Technical Notes

- **Thread-Safe UI Updates:** Uses `UnityMainThreadDispatcher` to safely update UI from async tasks.
- **Default Localhost Usage:** Server binds to `127.0.0.1` by default for local testing.
- **Real-Time Broadcasting:** All messages are handled asynchronously, ensuring low-latency communication.
- **Scalable UI:** Canvas anchors ensure UI scales correctly for different resolutions.

---

## Video Demo

A complete walkthrough of the project, showing server and client functionality in action:  

🎬 [Watch Demo on YouTube](https://youtu.be/2WgI0VluqFw?si=SD1GDDbxUlIM0Ohc)

---

## License

This project is open for educational and demonstration purposes. CITM UPC - Disseny i Desenvolupament de Videojocs. Xarxes i Jocs Online. 2025
