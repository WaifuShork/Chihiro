using System;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Chihiro.Attributes;
using Chihiro.Implementation;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace Chihiro.Services
{
    [AutoStart]
    public sealed class CommandHandlerService : ChihiroService 
    {
        private readonly CommandService _commandService;
        
        public CommandHandlerService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            RegisterEvents();
            
            var assembly = Assembly.GetAssembly(typeof(ChihiroHostService));
            
            _commandService = serviceProvider.GetRequiredService<CommandService>();
            _commandService.AddModules(assembly);
        }

        private void RegisterEvents()
        {
            ChihiroHostService.Client.MessageCreated += async (sender, args) => await OnMessageCreatedAsync(sender, args);
        }

        private async Task OnMessageCreatedAsync(BaseDiscordClient sender, MessageCreateEventArgs args)
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
            
            if (!CommandUtilities.HasPrefix(args.Message.Content, prefix, StringComparison.InvariantCultureIgnoreCase, out var output) && 
                !args.Message.HasMentionPrefix(client.CurrentUser, out output))
            {
                if (client.CurrentUser == null)
                {
                    return;
                }
            }

            var context = new ChihiroCommandContext(ChihiroHostService, args.Message, prefix);
            var result = await _commandService.ExecuteAsync(output, context);

            if (result == null)
            {
                await context.Channel.SendMessageAsync("Fatal error... this is embarrassing...");
                return;
            }
            
            if (result.IsSuccessful)
            {
                return;
            }

            await context.Channel.SendMessageAsync(result.ToString());
        }
    }
}