using Exiled.API.Interfaces;

namespace PPluginRemastered.Modules.CrazyPills
{
    public class CrazyPillsConfig : IConfig
    {
        public bool IsEnabled { get; set; } = false;
        public bool Debug { get; set; } = false;

        // ---
        public string PickupMessage { get; set; } = "Podniosłeś SCP-5854";
        public float GrenadeVomitInterval { get; set; } = 0.1f;
        public float FlashVomitInterval { get; set; } = 0.1f;
        // Amount of heath done to the player every flash interval. (Used to reduce flash time)
        public int FlashVomitHealth { get; set; } = 5;
        public float BallVomitInterval { get; set; } = 0.2f;
        public List<string> PossibleEffects { get; set; } =
        [
            "explode",
            "mutate",
            "god",
            "paper",
            "upsidedown",
            "flattened",
            "bombvomit",
            "flashvomit",
            "scp268",
            "amnesia",
            "bleeding",
            "corroding",
            "decontaminating",
            "hemorrhage",
            "panic",
            "sinkhole"
        ];
        public float MinDuration { get; set; } = 5f;
        public float MaxDuration { get; set; } = 30f;
    }
}
