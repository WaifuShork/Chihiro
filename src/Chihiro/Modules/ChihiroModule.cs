using System;
using System.Threading.Tasks;
using Chihiro.Configurations;
using Chihiro.Implementation;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Serilog;

namespace Chihiro.NET.Modules
{
    public abstract class ChihiroModule : ModuleBase<ChihiroCommandContext>, IAsyncDisposable
    {
        protected ChihiroHostService ChihiroHostService { get; }
        protected Configuration Configuration { get; }
        
        private readonly IServiceScope _scope;

        protected ChihiroModule(IServiceProvider serviceProvider)
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
        
        protected async Task<DiscordMessage> ReplyAsync(string text)
        {
            return await Context.Channel.SendMessageAsync(text);
        }

        protected async Task<DiscordMessage> ReplyAsync(DiscordEmbed embed)
        {
            return await Context.Channel.SendMessageAsync(embed: embed);
        }

        protected async Task<DiscordMessage> ReplyAsync(string text, DiscordEmbed embed)
        {
            return await Context.Channel.SendMessageAsync(text, false, embed);
        }

        protected DiscordEmbedBuilder CreateEmbed(string title)
        {
            return new DiscordEmbedBuilder()
                .WithTitle(title)
                .WithTimestamp(DateTime.Now);
        }
    }
}