using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Hogwarts.Services
{
    public class Bash
    {
        private readonly ILogger<Bash> _logger;

        public Bash(ILogger<Bash> logger)
        {
            _logger = logger;
        }

        public Process Execute(string command)
        {
            var escapedArgs = command.Replace("\"", "\\\"");

            _logger.LogInformation($"Executing command '{command}'.");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            process.Start();

            return process;
        }

        public Task<int> ExecuteAsync(string command)
        {
            var source = new TaskCompletionSource<int>();
            var escapedArgs = command.Replace("\"", "\\\"");

            _logger.LogInformation($"Executing command '{command}'.");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            process.Exited += (sender, args) =>
              {
                  _logger.LogWarning(process.StandardError.ReadToEnd());
                  _logger.LogInformation(process.StandardOutput.ReadToEnd());
                  
                  if (process.ExitCode == 0)
                  {
                      source.SetResult(0);
                  }
                  else
                  {
                      source.SetException(new Exception($"Command `{command}` failed with exit code `{process.ExitCode}`."));
                  }

                  process.Dispose();
              };

            try
            {
                process.Start();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "The process failed.");
                source.SetException(e);
            }

            return source.Task;
        }
    }
}