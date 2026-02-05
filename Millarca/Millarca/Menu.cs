using Millarca.Cheats;
using UnityEngine;

namespace Millarca
{
    internal class Menu
    {
        public bool showMenu = true;
        public Rect windowRect = new(10, 10, 260, 160);
        public int selectedIndex = 0;

        internal MillarcaCheat[] menuFields = [new PlayerESP(), new GameSpeed(), new Flight()];

        internal void DrawWindow(int id)
        {
            GUILayout.BeginVertical();

            if (menuFields.Length == 0)
            {
                GUILayout.Label("No menu items defined.");
            }
            else
            {
                for (int i = 0; i < menuFields.Length; i++)
                {
                    string label = menuFields[i].Label;
                    string prefix = (i == selectedIndex) ? "> " : "  ";
                    string suffix = menuFields[i].LabelSuffix;
                    string final;
                    final = prefix + label;
                    if (!string.IsNullOrEmpty(suffix))
                        final += $" [{suffix}]";

                    GUILayout.Label(final);
                }
            }

            GUILayout.EndVertical();
        }

        internal void HandleLeft()
        {
            if (menuFields.Length > 0)
                menuFields[selectedIndex].HandleLeft();
        }

        internal void HandleRight()
        {
            if (menuFields.Length > 0)
                menuFields[selectedIndex].HandleRight();
        }

        internal void HandleEnter()
        {
            if (menuFields.Length > 0)
                menuFields[selectedIndex].HandleEnter();
        }
    }
}
