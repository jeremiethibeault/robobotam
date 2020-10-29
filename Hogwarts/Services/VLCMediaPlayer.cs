using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Hogwarts.Configuration;
using Microsoft.Extensions.Logging;

namespace Hogwarts.Services
{
    public class VLCMediaPlayer
    {
        private readonly AppSettings _appSettings;
        private readonly Bash _bash;
        private readonly ILogger<VLCMediaPlayer> _logger;

        public VLCMediaPlayer(AppSettings appSettings, Bash bash, ILogger<VLCMediaPlayer> logger)
        {
            _logger = logger;
            _appSettings = appSettings;
            _bash = bash;
        }

        public VLCMediaPlayerProcess PlayFile(string pathToFile)
        {
            if (!File.Exists(pathToFile))
            {
                throw new InvalidOperationException($"The file '{pathToFile}' doesn't exist.");
            }

            _logger.LogInformation("Playing file '{pathToFile}'.", pathToFile);

            var process = _bash.Execute($"{_appSettings.VLCCommandLine} {pathToFile}");

            return new VLCMediaPlayerProcess(process, _logger);
        }

        public VLCMediaPlayerProcess PlayRandomFromFolder(string pathToFolder)
        {
            if (!Directory.Exists(pathToFolder))
            {
                throw new InvalidOperationException($"The folder '{pathToFolder}' doesn't exist.");
            }

            _logger.LogInformation("Playing random file from '{pathToFolder}'.", pathToFolder);
            
            var process = _bash.Execute($"{_appSettings.VLCCommandLine} --random {pathToFolder}");

            return new VLCMediaPlayerProcess(process, _logger);
        }
    }
}