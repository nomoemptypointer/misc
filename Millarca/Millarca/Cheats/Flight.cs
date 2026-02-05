using UnityEngine;

namespace Millarca.Cheats
{
    internal class Flight : MillarcaCheat
    {
        public static bool Enabled = false;
        public GameObject player;
        public FirstPersonController controller;

        internal Flight(string label = "Flight") : base(label)
        {
            while (PlayerManager.localPlayer != null)
            {
                player = PlayerManager.localPlayer;
                controller = GameObject.FindObjectOfType<FirstPersonController>();
            }
        }

        public override string LabelSuffix
        {
            get => $"{Enabled}";
        }

        public override void HandleEnter()
        {
            if (Enabled)
                Enabled = false;
            else
                Enabled = true;
            controller.noclip = Enabled;
        }

        public override void HandleLeft()
        {

        }

        public override void HandleRight()
        {

        }
    }
}
