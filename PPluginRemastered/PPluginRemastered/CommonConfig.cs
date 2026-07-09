using Exiled.API.Interfaces;

namespace PPluginRemastered
{
    public class CommonConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }
    }
}
