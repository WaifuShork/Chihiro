using System.Threading.Tasks;
using Chihiro.Commons;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Chihiro.Attributes
{
    /// <summary>
    /// This attribute ensures that the context is either Guild or DM.
    /// Could be used for instance for the prefix command, as the command should only be executed in a guild.
    /// </summary>
    public class ContextAttribute : CheckBaseAttribute
    {
        private readonly ContextType _contextType;

        public ContextAttribute(ContextType contextType)
        {
            _contextType = contextType;
        }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            return _contextType == ContextType.Guild 
                ? Task.FromResult(ctx.Channel.Guild == null) 
                : Task.FromResult(ctx.Channel.Type == ChannelType.Private);
        }
    }
}