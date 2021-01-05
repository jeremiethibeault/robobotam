using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Hogwarts.Configuration;
using Microsoft.Extensions.Configuration;
using Hogwarts.Services;

namespace Hogwarts
{
    public class Program
    {
        public static IServiceProvider Services { get; private set; }

        public static async Task Main(string[] args)
        {
            Console.WriteLine($"Initializing Hogwarts...\n");

            var configuration = LoadConfiguration();

            Services = ConfigureServices(configuration);

            await Start();
        }

        private static IConfiguration LoadConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();

            configurationBuilder
                .AddJsonFile($"appsettings.{(OperatingSystemHelper.IsMacOS ? "MacOS" : "Linux")}.json")
                .AddJsonFile("appsettings.Local.json", optional: true);

            return configurationBuilder.Build();
        }

        private static IServiceProvider ConfigureServices(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            services.AddLogging(c => c.AddConsole());
            services.AddSingleton(configuration.Get<AppSettings>());
            services.AddSingleton<Bash>();
            services.AddSingleton<VLCMediaPlayer>();

            return services.BuildServiceProvider();
        }

        private static async Task Start()
        {
            var appSettings = Services.GetRequiredService<AppSettings>();
            var mediaPlayer = Services.GetRequiredService<VLCMediaPlayer>();

            var mediaPlayerProcess = mediaPlayer.PlayRandomFromFolder(appSettings.SongsFolderPath);

            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey();
                Console.WriteLine();

                switch (cki.Key)
                {
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D6:
                        await mediaPlayerProcess.Next();
                        break;

                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.D4:
                        await mediaPlayerProcess.Previous();
                        break;

                    case ConsoleKey.Spacebar:
                    case ConsoleKey.D5:
                        await mediaPlayerProcess.TogglePause();
                        break;

                    case ConsoleKey.UpArrow:
                    case ConsoleKey.D8:
                    case ConsoleKey.Add:
                        await mediaPlayerProcess.IncreaseVolume();
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.D2:
                    case ConsoleKey.Subtract:
                        await mediaPlayerProcess.DecreaseVolume();
                        break;

                    case ConsoleKey.R:
                    case ConsoleKey.Multiply:
                        await mediaPlayerProcess.ToggleRandom();
                        break;

                    case ConsoleKey.L:
                    case ConsoleKey.Divide:
                        await mediaPlayerProcess.ToggleLoop();
                        break;

                    case ConsoleKey.P:
                    case ConsoleKey.D7:
                    case ConsoleKey.Home:
                        await mediaPlayerProcess.Play();
                        break;

                    case ConsoleKey.Q:
                    case ConsoleKey.D1:
                    case ConsoleKey.End:
                        await mediaPlayerProcess.Stop();
                        break;
                }
            } while (cki.Key != ConsoleKey.Escape);
        }
    }
}
