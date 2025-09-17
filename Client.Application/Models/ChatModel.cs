namespace Client.Application.Models
{
    public class ChatModel
    {
        public string UserName { get; set; } = "";
        public string Ip { get; set; } = "";
        public int Port { get; set; }
        public List<string> Messages { get; set; } = new();
    }
}
