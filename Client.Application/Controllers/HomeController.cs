using Client.Application.ClientHearing;
using Client.Application.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Client.Application.Controllers
{
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
            _client.ConnectToServer("192.168.1.117", 2000);
            _client.SendingData("Hellooooo");

            return View();
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
