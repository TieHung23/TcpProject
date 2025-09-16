using Microsoft.Extensions.Logging;
using Server;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

ILogger<TcpServer> logger = loggerFactory.CreateLogger<TcpServer>();
TcpServer server = new(2000, logger);

await server.StartServer();