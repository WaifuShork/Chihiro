#nullable enable
using System;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Chihiro.Implementation
{
    public class ChihiroCommandContext : Qmmands.CommandContext
    {
        /// <summary>
        /// Gets the logged-in client.
        /// </summary>
        public readonly DiscordClient Client;

        /// <summary>
        /// Gets the guild where the command was executed, null if context is a DM.
        /// </summary>
        public readonly DiscordGuild? Guild;

        /// <summary>
        /// Gets the channel where the command was executed.
        /// </summary>
        public readonly DiscordChannel Channel;

        /// <summary>
        /// Gets the user that executed the command. If the command was executed in a guild, then this will contain the command's member.
        /// </summary>
        public DiscordUser User { get; set; }

        /// <summary>
        /// Gets the current logged-in user.
        /// </summary>
        public DiscordMember? CurrentMember => Guild?.CurrentMember;

        /// <summary>
        /// Gets the user's message that executed the command.
        /// </summary>
        public readonly DiscordMessage Message;

        /// <summary>
        /// Gets the prefix of the server or the default one.
        /// </summary>
        public readonly string Prefix;

        public ChihiroCommandContext(IServiceProvider serviceProvider, DiscordMessage message, string prefix)
            : base(serviceProvider)
        {
            var bot = serviceProvider.GetRequiredService<ChihiroHostService>();
            Guild = message.Channel.Guild;

            Client = bot.Client;
            Channel = message.Channel;
            User = message.Author;
            Message = message;
            Prefix = prefix;
        }
    }
}