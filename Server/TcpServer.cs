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
                var ipAddressClient = remoteEndPoint?.Port.ToString() ?? "Anonymous";
                _logger.LogInformation($"Client from {ipAddressClient} is connected");

                _ = HandleClientAsync(client, ipAddressClient);
            }
        }

        private async Task HandleClientAsync(TcpClient client, string ipAddressClient)
        {
            using NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    _logger.LogInformation($"Client from {ipAddressClient} is disconnected");
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                _logger.LogInformation($"Receive : {message}");

                string response = $"Server received: {message}";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
        }
    }
}
