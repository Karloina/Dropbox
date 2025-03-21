﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dropbox.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DropboxController : ControllerBase
    {
        private readonly ILogger<DropboxController> _logger;
        private readonly string _targetFolderPath;
        private static ConcurrentDictionary<string, string> _activeUsers = new ConcurrentDictionary<string, string>();

        public DropboxController(ILogger<DropboxController> logger)
        {
            _logger = logger;
            _targetFolderPath = @"D:\studia\Masters\SEM3\Wsp\ScanCatalog";
        }

        [HttpGet]
        public IEnumerable<ThreadData> Get()
        {
            return FileManager.tasks.Values.Select(x => new ThreadData()
            {
                Guid = x.Id.ToString(),
                Status = $"{x.GetType().Name} - {x.FileName} - {x.Login} - {x.TransferStatus}"
            }).ToArray();
        }

        [HttpPost("/files/{login}")]
        public async Task<IActionResult> Upload(IFormFile file, string login)
        {
            if (file.Length > 0)
            {
                FileManager.UploadFile(login, file.FileName, file, _targetFolderPath);
            }

            return Ok();
        }

        [HttpGet("/files/{login}/{filename}")]
        public async Task<IActionResult> Download(string login, string filename)
        {
            string filePath = @$"{_targetFolderPath}\{login}\{filename}";
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            _logger.LogInformation($"downloading file [{filePath}].");
            var downloadFile = FileManager.DownloadFile(login, filename, _targetFolderPath);

            return File(downloadFile, MediaTypeNames.Application.Octet, filename);
        }

        [HttpGet("/files/{login}")]
        public async Task<IActionResult> GetFilesForUser(string login)
        {
           return Ok(Directory.GetFiles($@"{_targetFolderPath}\{login}").Select(Path.GetFileName).ToArray());
        }

        [HttpGet("/authenticate/{login}")]
        public async Task<IActionResult> LogIn(string login)
        {
            Directory.CreateDirectory($@"{_targetFolderPath}\{login}");

            if (!_activeUsers.ContainsKey(login))
            {
                _activeUsers.TryAdd(login, login);
            }

            return Ok();
        }

        [HttpDelete("/authenticate/{login}")]
        public async Task<IActionResult> LogOut(string login)
        {
            if (_activeUsers.ContainsKey(login))
            {
                _activeUsers.Remove(login, out login);
            }

            return Ok();
        }

        [HttpGet("/users")]
        public async Task<IActionResult> ActiveUsers()
        {
            return Ok(_activeUsers.Keys);
        }
    }
}
