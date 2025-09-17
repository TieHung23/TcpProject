using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Text;

namespace Client.Application.Controllers
{
    public class UserModel
    {
        public string Name { get; set; } = string.Empty;

        public string Ip { get; set; } = string.Empty;

        public int Port { get; set; }
    }

    public class FileController : Controller
    {
        private string GetIp() => HttpContext.Session.GetString("ServerIp") ?? "127.0.0.1";
        private int GetPort() => int.Parse(HttpContext.Session.GetString("ServerPort") ?? "2000");
        private string GetUserName() => HttpContext.Session.GetString("UserName") ?? "Anonymous";

        public IActionResult Sending()
        {
            return View(new UserModel
            {
                Name = GetUserName(),
                Port = GetPort(),
                Ip = GetIp()

            });
        }

        [HttpPost]
        public async Task<IActionResult> Sending(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Error = "No file selected!";
                return View(new UserModel
                {
                    Name = GetUserName(),
                    Port = GetPort(),
                    Ip = GetIp()

                });
            }

            try
            {
                string ip = GetIp();
                int port = GetPort();
                string username = GetUserName();

                using (var client = new TcpClient())
                {
                    await client.ConnectAsync(ip, port);
                    using (var networkStream = client.GetStream())
                    {
                        byte dataType = 1;
                        await networkStream.WriteAsync(new byte[] { dataType }, 0, 1);

                        var usernameBytes = Encoding.UTF8.GetBytes(username);
                        var usernameLength = BitConverter.GetBytes(usernameBytes.Length);
                        await networkStream.WriteAsync(usernameLength, 0, usernameLength.Length);
                        await networkStream.WriteAsync(usernameBytes, 0, usernameBytes.Length);

                        var fileNameBytes = Encoding.UTF8.GetBytes(file.FileName);
                        var fileNameLength = BitConverter.GetBytes(fileNameBytes.Length);
                        await networkStream.WriteAsync(fileNameLength, 0, fileNameLength.Length);
                        await networkStream.WriteAsync(fileNameBytes, 0, fileNameBytes.Length);

                        var fileLength = BitConverter.GetBytes(file.Length);
                        await networkStream.WriteAsync(fileLength, 0, fileLength.Length);

                        using (var fileStream = file.OpenReadStream())
                        {
                            byte[] buffer = new byte[8192];
                            int bytesRead;
                            while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await networkStream.WriteAsync(buffer, 0, bytesRead);
                            }
                        }
                    }
                }

                ViewBag.Message = $"File '{file.FileName}' sent successfully to {ip}:{port} as {username}";
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error while sending file: {ex.Message}";
            }

            return View(new UserModel
            {
                Name = GetUserName(),
                Port = GetPort(),
                Ip = GetIp()

            });
        }
    }
}
