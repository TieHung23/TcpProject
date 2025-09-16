using System.Net.Sockets;
using System.Text;

namespace Client.Application.ClientHearing
{
    public class Client : IClient
    {
        private TcpClient? _client;
        private NetworkStream? _network;
        public async Task ConnectToServer(string ip, int port)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(ip, port);

            _network = _client.GetStream();

            Console.WriteLine($"Connected to server {ip}:{port}");
        }

        public async Task SendingData(string message)
        {
            if (_network == null)
                throw new InvalidOperationException("Not connected to server.");

            byte[] data = Encoding.UTF8.GetBytes(message);
            await _network.WriteAsync(data, 0, data.Length);

            Console.WriteLine($"Sent: {message}");

            byte[] buffer = new byte[1024];
            int bytesRead = await _network.ReadAsync(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            Console.WriteLine($"Server: {response}");
        }
    }
}
