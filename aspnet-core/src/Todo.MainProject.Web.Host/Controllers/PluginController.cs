﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Todo.MainProject.Controllers;
using Todo.MainProject.Web.Host.Services;
using Todo.MainProject.Web.Host.Services.Dto;

namespace Todo.MainProject.Web.Host.Controllers
{
    public class PluginController : MainProjectControllerBase
    {
        private readonly IPluginService _pluginService;

        public PluginController(IPluginService pluginService)
        {
            _pluginService = pluginService;
        }

        [HttpGet("api/[controller]/GetPluginObjectsResult")]
        public List<PluginObject> GetPluginObjectsResult()
        {
            return _pluginService.GetPluginObjects();
        }

        [HttpGet("api/[controller]/Download")]
        public List<FileContentResult> Download(string pluginName)
        {
            if (_pluginService.IsNullService() || pluginName == null)
            {
                return null;
            }

            var pluginObjects = _pluginService.GetPluginObjects();
            var plugin = pluginObjects.FirstOrDefault(p => p.Title == pluginName);
            if (plugin == null)
            {
                return null;
            }

            var folder = plugin.Url.Replace("/", "");
            var fileProvider = _pluginService.GetFileProvider(pluginName);
            var fileEntries = fileProvider.GetDirectoryContents(folder);

            return fileEntries.Select(LoadFileFromPath).Where(file => file != null).ToList();
        }

        private FileContentResult LoadFileFromPath(IFileInfo fileEntry)
        {
            var filename = fileEntry.PhysicalPath;
            if (filename == null)
            {
                return null;
            }

            var fileBytes = System.IO.File.ReadAllBytes(filename);
            var file = File(fileBytes, GetContentType(filename), Path.GetFileName(filename));
            return file;
        }

        private static string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".ico", "image/x-icon"},
                {".js", "application/javascript"},
                {".html", "text/html" },
                {".css", "text/css" }
            };
            }
    }
}