using System;
using System.Threading.Tasks;
using Chihiro.Extensions;
using Chihiro.Implementation;
using Qmmands;
using DSharpPlus.Entities;

namespace Chihiro.Modules
{
    [Name("General")]
    public sealed partial class GeneralModule : ChihiroModuleBase<ChihiroCommandContext>
    {
        public GeneralModule(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        
        
    }
}