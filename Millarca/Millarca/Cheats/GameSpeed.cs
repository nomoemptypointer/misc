using UnityEngine;

namespace Millarca.Cheats
{
    internal class GameSpeed(string label = "Time Scale") : MillarcaCheat(label)
    {
        // Internal
        public float speedMultiplier = Time.timeScale;
        public readonly float speedMultiplierPrev = Time.timeScale;
        public const float step = 0.01f;
        public override string LabelSuffix
        {
            get => $"{speedMultiplier:F2}x";
        }

        public override void HandleEnter()
        {
            speedMultiplier = speedMultiplierPrev;
        }

        public override void HandleLeft()
        {
            // Decrease speed
            speedMultiplier -= step;
            if (speedMultiplier < 0f) speedMultiplier = 0f; // Prevent negative time scale
            Time.timeScale = speedMultiplier;
        }

        public override void HandleRight()
        {
            // Increase speed
            speedMultiplier += step;
            Time.timeScale = speedMultiplier;
        }
    }
}
