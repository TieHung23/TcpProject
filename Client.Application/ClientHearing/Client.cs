using System.Net.Sockets;
using System.Text;

namespace Client.Application.ClientHearing
{
    public class Client : IClient
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private static readonly List<string> _messages = new();

        public async Task ConnectToServer(string ip, int port)
        {
            if (_client != null && _client.Connected) return;

            _client = new TcpClient();
            await _client.ConnectAsync(ip, port);
            _stream = _client.GetStream();

            _ = Task.Run(async () =>
            {
                var buffer = new byte[1024];
                while (_client.Connected)
                {
                    int byteCount = await _stream.ReadAsync(buffer, 0, buffer.Length);
                    if (byteCount == 0) break;

                    var message = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    lock (_messages) _messages.Add(message);
                }
            });
        }

        public async Task SendingData(string message)
        {
            if (_stream == null) return;

            var data = Encoding.UTF8.GetBytes(message);
            await _stream.WriteAsync(data, 0, data.Length);
        }
        public List<string> GetMessages()
        {
            lock (_messages) return _messages.ToList();
        }

    }
}
