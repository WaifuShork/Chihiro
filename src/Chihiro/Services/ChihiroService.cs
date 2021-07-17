using System;
using Chihiro.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace Chihiro.Services
{
    public class ChihiroService
    {
        protected ChihiroService(IServiceProvider serviceProvider)
        {
            ChihiroHostService = serviceProvider.GetRequiredService<ChihiroHostService>();
            Configuration = new Configuration("appsettings.json").LoadAsync().GetAwaiter().GetResult();
        }

        protected ChihiroHostService ChihiroHostService { get; }
        protected Configuration Configuration { get; }
    }
}