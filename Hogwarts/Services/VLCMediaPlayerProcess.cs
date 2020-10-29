using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Hogwarts.Services
{
    public class VLCMediaPlayerProcess : IDisposable
    {
        private readonly Process _process;
        private readonly ILogger<VLCMediaPlayer> _logger;

        public VLCMediaPlayerProcess(Process process, ILogger<VLCMediaPlayer> logger)
        {
            _process = process;
            _logger = logger;

            _process.BeginOutputReadLine();
        }

        public async Task Next()
        {
            await Execute(new[] { "next", "get_title" }, title =>
            {
                _logger.LogInformation($"Playing next media '{title}'.");
            });
        }

        public async Task Previous()
        {
            await Execute(new[] { "prev", "get_title" }, title =>
            {
                _logger.LogInformation($"Playing previous media '{title}'.");
            });
        }

        public async Task TogglePause()
        {
            await Execute(new[] { "pause", "status" }, status =>
            {
                if (status.Contains("paused"))
                {
                    _logger.LogInformation("Resuming media.");
                }
                else
                {
                    _logger.LogInformation("Pausing media.");
                }
            });
        }

        public async Task IncreaseVolume()
        {
            await Execute(new[] { "volup", "volume" }, volume =>
            {
                var pattern = @"audio volume: (\d*)";
                var matches = Regex.Matches(volume, pattern);

                _logger.LogInformation($"Increasing volume to '{matches[0].Groups[1]}'.");
            });
        }

        public async Task DecreaseVolume()
        {
            await Execute(new[] { "voldown", "volume" }, volume =>
            {
                var pattern = @"audio volume: (\d*)";
                var matches = Regex.Matches(volume, pattern);

                _logger.LogInformation($"Decreasing volume to '{matches[0].Groups[1]}'.");
            });
        }

        public async Task ToggleLoop()
        {
            await Execute("loop");

            _logger.LogInformation("Toggling loop mode.");
        }

        public async Task ToggleRandom()
        {
            await Execute("random");

            _logger.LogInformation("Toggling random mode.");
        }

        private async Task Execute(string command, Action<string> onOutputReceived = null)
        {
            await Execute(new[] { command }, onOutputReceived);
        }

        private async Task Execute(string[] commands, Action<string> onOutputReceived = null)
        {
            var tcs = new TaskCompletionSource<string>();
            var sb = new StringBuilder();

            _process.OutputDataReceived += OnOutputDataReceived;

            foreach (var command in commands)
            {
                _process.StandardInput.WriteLine(command);
            }

            if (onOutputReceived != null)
            {
                var output = await tcs.Task;

                onOutputReceived(output);
            }

            _process.OutputDataReceived -= OnOutputDataReceived;

            void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
            {
                sb.Append(e.Data);

                if (e.Data.StartsWith(">"))
                {
                    var result = sb
                        .Replace(">", string.Empty)
                        .ToString()
                        .Trim();

                    tcs.TrySetResult(result);
                }
            }
        }

        public void Dispose()
        {
            _process.Dispose();
        }
    }
}