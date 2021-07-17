using System;
using System.Threading.Tasks;
using Chihiro.Extensions;
using Qmmands;
using Chihiro.NET.Modules;
using DSharpPlus.Entities;

namespace Chihiro.Modules
{
    [Name("General")]
    public class GeneralModule : ChihiroModule
    {
        public GeneralModule(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        
        [Command("ping", "latency")]
        public async Task PingAsync()
        {
            var latency = ChihiroHostService.Latency;

            var color = latency switch
            {
                < 200 => DiscordColor.Green,
                > 1000 => DiscordColor.Red,
                _ => DiscordColor.Yellow
            };

            await CreateEmbed("Pong! :ping_pong:")
                .WithDescription($"Chihiro currently has a latency of `{latency}ms`.")
                .WithColor(color)
                .SendAsync(Context.Channel);
        }
    }
}