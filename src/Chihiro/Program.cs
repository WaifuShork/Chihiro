using System;
using System.IO;
using System.Threading.Tasks;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Chihiro
{
    internal static class Program
    {
        private static readonly string LogPath = Path.Combine(Environment.CurrentDirectory, "logs/watermelon-.log");
        
        private static async Task<int> Main(string[] args)
        {
            var configPath = string.Empty;
            if (args.Length == 0)
            {
                configPath = "appsettings.json";
            }
            else if (args.Length == 1)
            {
                configPath = args[0];
            }
            
            return await new ChihiroHostService().RunHostAsync(configPath);
        }
    }
}