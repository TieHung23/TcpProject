using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class TcpServer
    {
        private readonly int _port;
        private readonly ILogger<TcpServer> _logger;

        private static readonly List<TcpClient> _clients = new();
        private static readonly List<string> _messages = new();

        public TcpServer(int port, ILogger<TcpServer> logger)
        {
            _port = port;
            _logger = logger;
        }

        public async Task StartServer()
        {
            IPAddress ipAdress = IPAddress.Any;
            TcpListener server = new TcpListener(ipAdress, _port);

            server.Start();
            _logger.LogInformation($"Server is start on port : {_port}");

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                IPEndPoint? remoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
                var ipAddressClient = remoteEndPoint?.Address.ToString() ?? "Anonymous";

                _logger.LogInformation($"Client from {ipAddressClient} is connected");

                lock (_clients) _clients.Add(client);
                _ = HandleClientAsync(client, ipAddressClient);
            }
        }

        public static List<string> GetMessages()
        {
            lock (_messages) return _messages.ToList();
        }

        private async Task HandleClientAsync(TcpClient client, string ipAddressClient)
        {
            using NetworkStream stream = client.GetStream();
            var buffer = new byte[4];

            try
            {
                while (true)
                {
                    int readType = await stream.ReadAsync(buffer, 0, 1);
                    if (readType == 0) break;

                    byte dataType = buffer[0];

                    if (dataType == 0)
                    {
                        await stream.ReadAsync(buffer, 0, 4);
                        int messageLength = BitConverter.ToInt32(buffer, 0);

                        var messageBytes = new byte[messageLength];
                        int totalRead = 0;
                        while (totalRead < messageLength)
                        {
                            int read = await stream.ReadAsync(messageBytes, totalRead, messageLength - totalRead);
                            if (read == 0) break;
                            totalRead += read;
                        }

                        var message = Encoding.UTF8.GetString(messageBytes);

                        lock (_messages) _messages.Add(message);

                        lock (_clients)
                        {
                            foreach (var cl in _clients.ToList())
                            {
                                try
                                {
                                    var writer = cl.GetStream();
                                    var data = Encoding.UTF8.GetBytes(message);
                                    writer.Write(data, 0, data.Length);
                                }
                                catch
                                {
                                    _clients.Remove(cl);
                                }
                            }
                        }

                        _logger.LogInformation($"[CHAT] Received from {ipAddressClient}: {message}");
                    }
                    else if (dataType == 1)
                    {
                        await stream.ReadAsync(buffer, 0, 4);
                        int usernameLength = BitConverter.ToInt32(buffer, 0);

                        var usernameBytes = new byte[usernameLength];
                        await stream.ReadAsync(usernameBytes, 0, usernameLength);
                        string username = Encoding.UTF8.GetString(usernameBytes);

                        await stream.ReadAsync(buffer, 0, 4);
                        int fileNameLength = BitConverter.ToInt32(buffer, 0);

                        var fileNameBytes = new byte[fileNameLength];
                        await stream.ReadAsync(fileNameBytes, 0, fileNameLength);
                        string fileName = Encoding.UTF8.GetString(fileNameBytes);

                        var fileLengthBytes = new byte[8];
                        await stream.ReadAsync(fileLengthBytes, 0, 8);
                        long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

                        Directory.CreateDirectory("Uploads");
                        string path = Path.Combine("Uploads", fileName);

                        using (var fs = File.Create(path))
                        {
                            byte[] chunk = new byte[8192];
                            long totalRead = 0;

                            while (totalRead < fileLength)
                            {
                                int read = await stream.ReadAsync(chunk, 0, chunk.Length);
                                if (read == 0) break;
                                await fs.WriteAsync(chunk, 0, read);
                                totalRead += read;
                            }
                        }

                        _logger.LogInformation($"[FILE] {username} uploaded {fileName} ({fileLength} bytes) from {ipAddressClient}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Client {ipAddressClient} disconnected: {ex.Message}");
            }
        }

    }
}
