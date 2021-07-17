using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Chihiro.Attributes;
using Chihiro.Configurations;
using Chihiro.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Qmmands;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Chihiro
{
    public class ChihiroHostService : IServiceProvider
    {
        public DiscordClient Client { get; private set; }
        public DiscordUser CurrentUser => Client.CurrentUser;
        public int Latency => Client.Ping;

        private Configuration _configuration;
        private IServiceProvider _provider;

        public async Task<int> RunHostAsync(string path)
        {
            try
            {
                await BuildHostAsync(path);
                await Client.ConnectAsync();
                await Task.Delay(-1);
                return 0;
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        
        private async Task BuildHostAsync(string path)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Async(x =>
                {
                    x.Console(theme: SystemConsoleTheme.Literate);
                    x.File(Path.Combine("logs", "log-.txt"), shared: true, rollingInterval: RollingInterval.Day);
                })
                .CreateLogger();

            _configuration = new Configuration(path);
            _configuration = await _configuration.LoadAsync();

            var logFactory = new LoggerFactory().AddSerilog();

            Client = new DiscordClient(new DiscordConfiguration()
            {
                Token = _configuration.Token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug,
                LoggerFactory = logFactory,
                Intents = DiscordIntents.AllUnprivileged
            });

            var commandService = new CommandService(new CommandServiceConfiguration
            {
                DefaultRunMode = RunMode.Parallel,
                StringComparison = StringComparison.InvariantCultureIgnoreCase,
            });

            var chihiroServices = typeof(ChihiroHostService).Assembly.GetTypes()
                .Where(x => typeof(ChihiroService).IsAssignableFrom(x)
                            && !x.GetTypeInfo().IsInterface
                            && !x.GetTypeInfo().IsAbstract);

            var services = new ServiceCollection();
            foreach (var serviceType in chihiroServices)
            {
                services.AddSingleton(serviceType);
            }

            _provider = services
                .AddSingleton(this)
                .AddSingleton(_configuration)
                .AddSingleton(commandService)
                .BuildServiceProvider();

            var autoStartServices = typeof(ChihiroHostService).Assembly.GetTypes()
                .Where(x => typeof(ChihiroService).IsAssignableFrom(x) &&
                            x.GetCustomAttribute<AutoStartAttribute>() != null &&
                            !x.GetTypeInfo().IsInterface &&
                            !x.GetTypeInfo().IsAbstract).ToList();

            foreach (var serviceType in autoStartServices)
            {
                _provider.GetRequiredService(serviceType);
            }
        }
        
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ChihiroHostService) || serviceType == GetType())
            {
                return this;
            }

            return _provider.GetService(serviceType);
        }
    }
}