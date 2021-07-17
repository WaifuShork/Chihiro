using System.Threading.Tasks;
using Chihiro.Commons;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Chihiro.Attributes
{
    public class RequireContextAttribute : CheckBaseAttribute
    {
        private readonly ContextType _contextType;

        public RequireContextAttribute(ContextType contextType)
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