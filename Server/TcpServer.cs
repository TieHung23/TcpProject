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
            byte[] buffer = new byte[1024];

            while (true)
            {
                int byteCount;
                try
                {
                    byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);
                }
                catch { break; }

                if (byteCount == 0) break;

                var message = Encoding.UTF8.GetString(buffer, 0, byteCount);
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

                _logger.LogInformation($"Received from {ipAddressClient}: {message}");
            }
        }
    }
}
