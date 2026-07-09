using PPluginRemastered.ModuleLoader.Features;

namespace PPluginRemastered.Modules.CrazyPills
{
    public class CrazyPillsMain : CoreModule<CrazyPillsConfig>
    {
        public override string Name { get; } = "CrazyPills";
        public static CrazyPillsMain Instance { get; private set; }
        public static CrazyPillsConfig ModuleConfig { get; set; }
    }
}
