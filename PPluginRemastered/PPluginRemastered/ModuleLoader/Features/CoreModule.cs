using Exiled.API.Features;
using Exiled.API.Interfaces;

namespace PPluginRemastered.ModuleLoader.Features
{
    public abstract class CoreModule<TConfig> : ICoreModule<TConfig> where TConfig : IConfig, new()
    {
        protected CoreModule()
        {
            Name = "some module";
            Priority = 10;
        }

        public virtual string Name { get; }
        public virtual byte Priority { get; }

        public TConfig Config { get; } = new();

        public virtual void OnEnabled()
        {
            Log.Info($"Module [{Name}] has been enabled.");
        }

        public virtual void OnDisabled()
        {
            Log.Info($"Module [{Name}] has been disabled.");
        }

        public virtual void UnPatch()
        {
            // Ignore
        }
    }
}
