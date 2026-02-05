using MelonLoader;
using HarmonyLib;
using UnityEngine;

namespace Millarca
{
    public class MillarcaMain : MelonMod
    {
        internal static Menu Menu { get; private set; }
        

        public override void OnInitializeMelon()
        {
            Menu = new Menu();
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
                Menu.showMenu = !Menu.showMenu;

            if (!Menu.showMenu) return;

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Menu.selectedIndex = (Menu.selectedIndex - 1 + Menu.menuFields.Length) % (Menu.menuFields.Length == 0 ? 1 : Menu.menuFields.Length);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Menu.selectedIndex = (Menu.selectedIndex + 1) % (Menu.menuFields.Length == 0 ? 1 : Menu.menuFields.Length);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Menu.HandleLeft();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Menu.HandleRight();
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Menu.HandleEnter();
            }
        }

        public override void OnGUI()
        {
            if (!Menu.showMenu) return;

            Menu.windowRect = GUI.Window(
                123456,
                Menu.windowRect,
                Menu.DrawWindow,
                "Millarca"
            );
        }
    }
}
