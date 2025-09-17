namespace Client.Application.ClientHearing
{
    public interface IClient
    {
        Task ConnectToServer(string ip, int port);

        Task SendingData(string message);

        List<string> GetMessages();
    }
}
