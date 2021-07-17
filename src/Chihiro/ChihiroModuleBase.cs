using System;
using System.Threading.Tasks;
using Chihiro.Configurations;
using Chihiro.Implementation;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Serilog;

namespace Chihiro.Modules
{
    public abstract class ChihiroModuleBase<T> : ModuleBase<T>, IAsyncDisposable where T : ChihiroCommandContext
    {
        protected ChihiroHostService ChihiroHostService { get; }
        protected Configuration Configuration { get; }
        
        private readonly IServiceScope _scope;

        protected ChihiroModuleBase(IServiceProvider serviceProvider)
        {
            ChihiroHostService = serviceProvider.GetRequiredService<ChihiroHostService>();
            Configuration = serviceProvider.GetRequiredService<Configuration>();

            _scope = serviceProvider.CreateScope();
        }
        
        public async ValueTask DisposeAsync()
        {
            if (_scope is IAsyncDisposable asyncDisposableScope)
            {
                await asyncDisposableScope.DisposeAsync();
            }
            else
            {
                _scope.Dispose();
            }
            
            Log.Debug($"Module: {Context.Command.Module.Name}, Command: {Context.Command.Name}, scope disposed");
        }

        protected DiscordEmbedBuilder CreateEmbed(string title)
        {
            return new DiscordEmbedBuilder()
                .WithTitle(title)
                .WithTimestamp(DateTime.Now);
        }
    }
}