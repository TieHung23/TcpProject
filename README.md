# TcpProject

A simple TCP chat and file transfer application using ASP.NET Core MVC for the client and .NET for the server.

## Features

- Real-time chat between multiple clients via TCP.
- File sending from client to server.
- Modern Bootstrap-based UI.
- Session-based user management.

## Project Structure

```
TcpProject/
│
├── Client.Application/         # ASP.NET Core MVC client app
│   ├── Controllers/            # MVC controllers (Chat, File, Home)
│   ├── ClientHearing/          # TCP client logic
│   ├── Models/                 # View models
│   ├── Views/                  # Razor views for UI
│   ├── wwwroot/                # Static files (css, js, bootstrap)
│   └── Program.cs              # ASP.NET Core entry point
│
├── Server/                     # TCP server app
│   ├── TcpServer.cs            # TCP server logic
│   ├── Program.cs              # Server entry point
│   └── Server.csproj           # Server project file
│
├── TcpProject.sln              # Visual Studio solution file
└── README.md                   # This file
```

## How It Works

- **Server**: Listens for TCP connections, relays chat messages and receives files.
- **Client**: Connects to server, sends/receives chat messages, uploads files.

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 or later (recommended)

### Build and Run

#### 1. Start the Server

Open a terminal in the `Server` directory and run:

```sh
dotnet run
```

The server listens on port `2000` by default.

#### 2. Start the Client

Open a terminal in the `Client.Application` directory and run:

```sh
dotnet run
```

By default, the client web app runs at `http://localhost:5107` (see `launchSettings.json`).

#### 3. Open the Client in Browser

#### 3. Open the Client in Browser (Connect with IPv4 Address)

On the computer running the server, open a terminal and run:

```sh
ipconfig
```

Find your **IPv4 Address** under your Wi-Fi adapter (e.g., `192.168.1.10`).

On the client computer, open a browser and go to:

```
http://<server-ip>:5107
```

Replace `<server-ip>` with the IPv4 address you found (e.g., `http://192.168.1.10:5107`).

**Tutorial:**

1. On the server PC, press <kbd>Win</kbd> + <kbd>R</kbd>, type `cmd`, and press <kbd>Enter</kbd>.
2. Type `ipconfig` and press <kbd>Enter</kbd>.
3. Look for the `IPv4 Address` under your Wi-Fi adapter.
4. On the client PC, open a browser and enter `http://<server-ip>:5107` in the address bar.
5. Enter the server IP (the IPv4 address you found), port (`2000`), and your name in the chat form.
6. Click **Start Chat** to join the chat room.
7. Click **Start Sending File** to upload a file to the server.

## Chat UI

- Messages are displayed in a modern chat bubble style.
- Your messages are right-aligned and green; others are left-aligned.
- The chat updates in real time (polling every second).

## File Sending

- Go to the **Send File** page, choose a file, and send.
- Files are saved in the `Uploads` folder on the server.

## Customization

- UI styles can be changed in `Client.Application/wwwroot/css/site.css`.
- Server port can be changed in `Server/Program.cs` and client connection form.

## License

- Client uses [Bootstrap](Client.Application/wwwroot/lib/bootstrap/LICENSE), [jQuery](Client.Application/wwwroot/lib/jquery/LICENSE.txt), and [jQuery Validation](Client.Application/wwwroot/lib/jquery-validation/LICENSE.md) under the MIT License.

---

**Author:** TieHung23
