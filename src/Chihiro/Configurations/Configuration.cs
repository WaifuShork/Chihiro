using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace Chihiro.Configurations
{
    public class Configuration
    {
        private readonly string _configurationPath;
        
        public Configuration(string path)
        {
            _configurationPath = path;
        }
        
        [JsonProperty("Prefix")]
        public string Prefix { get; private set; }

        [JsonProperty("Token")]
        public string Token { get; private set; }

        public async Task<Configuration> LoadAsync()
        {
            if (string.IsNullOrWhiteSpace(_configurationPath))
            {
                throw new ArgumentNullException(nameof(_configurationPath), "Path is not a optional");
            }

            try
            {
                var contents = await File.ReadAllTextAsync(_configurationPath);
                return JsonConvert.DeserializeObject<Configuration>(contents);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, $"Unable to load configuration file: '{_configurationPath}'");
                return null;
            }
        }
    }
}