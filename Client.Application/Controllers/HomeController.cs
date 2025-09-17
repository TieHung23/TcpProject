using Client.Application.ClientHearing;
using Client.Application.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Client.Application.Controllers
{
    public class ConnectModel
    {
        public string Ip { get; set; } = "";
        public int Port { get; set; }
        public string Name { get; set; } = "";
        public bool Connected { get; set; }
        public string? ErrorMessage { get; set; }
        public string Action { get; set; } = "";
    }


    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IClient _client;


        public HomeController(ILogger<HomeController> logger, IClient client)
        {
            _logger = logger;
            _client = client;
        }

        public IActionResult Index()
        {
            return View(new ConnectModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(ConnectModel model)
        {
            try
            {
                await _client.ConnectToServer(model.Ip, model.Port);

                HttpContext.Session.SetString("UserName", model.Name);
                HttpContext.Session.SetString("Ip", model.Ip);
                HttpContext.Session.SetInt32("Port", model.Port);
                if (model.Action.Equals("chat")) return RedirectToAction("Message", "Chat");
                else return RedirectToAction("Sending", "File");
            }
            catch (Exception ex)
            {
                model.Connected = false;
                model.ErrorMessage = $"Connection failed: {ex.Message}";

                return View(model);
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
