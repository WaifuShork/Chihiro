using System;
using System.Threading.Tasks;
using Chihiro.Extensions;
using DSharpPlus.Entities;
using Qmmands;

namespace Chihiro.Modules
{
    public sealed partial class GeneralModule
    {
        [Command("ping", "latency")]
        public async Task PingAsync()
        {
            var timestamp = Context.Message.CreationTimestamp;
            var apiLatency = ChihiroHostService.Latency;
            
            var color = apiLatency switch
            {
                < 200 => DiscordColor.Green,
                > 1000 => DiscordColor.Red,
                _ => DiscordColor.Yellow
            };

            // Pong! :ping_pong:
            var message = await CreateEmbed("Latency")
                .SendAsync(Context.Channel);

            var clientLatency = (message.CreationTimestamp - timestamp).Milliseconds;
            await message.DeleteAsync();
            
            await CreateEmbed("Pong! :ping_pong:")
                .WithColor(color)
                .AddField("API Latency", $"{apiLatency}ms")
                .AddField("Client Latency", $"{clientLatency}ms")
                .AddField("What's the difference",
                            "`API Latency` represents the websocket connection to discord, this is grabbed " +
                                 "directly from DSharpPlus which is as close as it gets to accuracy.\n\n" +
                                 "`Client Latency` represents an edited timestamp difference -- the amount of time that's " +
                                 "passed since a message was sent and edited.")
                .SendAsync(Context.Channel);
        }
    }
}