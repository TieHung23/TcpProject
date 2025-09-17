using Client.Application.ClientHearing;
using Client.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Application.Controllers
{
    public class ChatController : Controller
    {
        private readonly IClient _client;

        public ChatController(IClient client)
        {
            _client = client;
        }

        [HttpGet]
        public IActionResult GetMessages()
        {
            var messages = _client.GetMessages();
            return Json(messages);
        }

        [HttpGet]
        public IActionResult Message()
        {
            var model = new ChatModel
            {
                UserName = HttpContext.Session.GetString("UserName") ?? "Unknown",
                Ip = HttpContext.Session.GetString("Ip") ?? "",
                Port = HttpContext.Session.GetInt32("Port") ?? 0,
                Messages = _client.GetMessages()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string message)
        {
            string name = HttpContext.Session.GetString("UserName") ?? "Unknown";
            string ip = HttpContext.Session.GetString("Ip") ?? "";
            int port = HttpContext.Session.GetInt32("Port") ?? 0;

            if (!string.IsNullOrWhiteSpace(message))
                await _client.SendingData($"{name}: {message}");

            var model = new ChatModel
            {
                UserName = name,
                Ip = ip,
                Port = port,
                Messages = _client.GetMessages()
            };

            return View("Message", model);
        }
    }
}
