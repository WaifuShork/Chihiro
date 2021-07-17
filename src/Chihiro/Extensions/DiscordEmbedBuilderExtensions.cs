using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace Chihiro.Extensions
{
    public static class DiscordEmbedBuilderExtensions
    {
        public static async Task<DiscordMessage> SendAsync(this DiscordEmbedBuilder builder, DiscordChannel channel)
        {
            return await channel.SendMessageAsync(string.Empty, false, builder.Build());
        }
    }
}