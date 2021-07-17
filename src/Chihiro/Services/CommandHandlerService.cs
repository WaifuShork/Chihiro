#nullable enable
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Chihiro.Attributes;
using Chihiro.Implementation;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Qmmands;
using Serilog;

namespace Chihiro.Services
{
    [AutoStart]
    public sealed class CommandHandlerService : ChihiroService 
    {
        private readonly CommandService _commandService;
        
        public CommandHandlerService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _commandService = serviceProvider.GetRequiredService<CommandService>();

            ChihiroHostService.Client.MessageCreated += OnMessageCreated;
            
            var assembly = Assembly.GetAssembly(typeof(ChihiroHostService));
            _commandService.AddModules(assembly);
        }

        private async Task OnMessageCreated(DiscordClient sender, MessageCreateEventArgs args)
        {
            if (args.Message.MessageType != MessageType.Default)
            {
                return;
            }
            
            if (args.Message.Author.IsBot)
            {
                return;
            }

            await ExecuteAsync(sender, args);
        }

        private async Task ExecuteAsync(BaseDiscordClient client, MessageCreateEventArgs args)
        {
            if (args.Guild != null)
            {
                var channelPermissions = args.Guild.CurrentMember.PermissionsIn(args.Channel);
                if (!channelPermissions.HasPermission(Permissions.SendMessages))
                {
                    return;
                }
            }

            var prefix = Configuration.Prefix;
            
            if (!CommandUtilities.HasPrefix(args.Message.Content, prefix, StringComparison.InvariantCultureIgnoreCase, out var output))
            {
                if (client.CurrentUser == null)
                {
                    return;
                }

                if (!CommandUtilities.HasPrefix(args.Message.Content, client.CurrentUser.Username,
                        StringComparison.InvariantCultureIgnoreCase, out output)
                    && !args.Message.HasMentionPrefix(client.CurrentUser, out output))
                {
                    return;
                }
            }

            var context = new ChihiroCommandContext(ChihiroHostService, args.Message, prefix);
            var result = await _commandService.ExecuteAsync(output, context);

            if (result.IsSuccessful)
            {
                return;
            }

            await context.Channel.SendMessageAsync(result.ToString());
        }

        
    }
}