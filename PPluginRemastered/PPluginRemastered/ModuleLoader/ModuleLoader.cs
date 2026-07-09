using System.Reflection;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Loader;
using PPluginRemastered.ModuleLoader.Features;

namespace PPluginRemastered.ModuleLoader
{
    public class ModuleLoader
    {
        public static SortedSet<ICoreModule<IConfig>> Modules { get; } = new(new PriorityComparer());
        public static HashSet<ICoreModule<IConfig>> EnabledModules { get; } = [];

        public static void Load()
        {
            Directory.CreateDirectory(Path.Combine(Paths.Configs, $"Core {Server.Port}"));

            Log.Info("Loading modules");

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsAbstract || type.IsInterface)
                {
                    continue;
                }

                if (!type.BaseType.IsGenericType || type.BaseType.GetGenericTypeDefinition() != typeof(CoreModule<>) || type.GetCustomAttributes(typeof(DisabledFeatureAttribute), false).Any())
                {
                    continue;
                }

                ConstructorInfo ctr = type.GetConstructor(Type.EmptyTypes)!;
                ICoreModule<IConfig> module = ctr.Invoke(null) as ICoreModule<IConfig>;

                ConfigManager.LoadConfig(module);
                Modules.Add(module!);
            }

            foreach (ICoreModule<IConfig> module in Modules)
            {
                try
                {
                    if (module.Config.IsEnabled)
                    {
                        module.OnEnabled();
                        EnabledModules.Add(module);
                    }
                    else
                    {
                        module.UnPatch();
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Module {module.Name} threw an exception while enabling:");
                    Log.Error(e);
                }
            }
        }

        public static void UnLoad()
        {
            foreach (ICoreModule<IConfig> module in EnabledModules)
            {
                module.OnDisabled();
            }
        }
    }
}
