using System.Collections.Generic;
using UnityEngine;

namespace Millarca
{
    public static class PlayerUtils
    {
        public static List<GameObject> GetPlayers(bool rejectLocal = false)
        {
            List<GameObject> plys = [.. PlayerManager.singleton.players];
            if (rejectLocal)
                plys.Remove(PlayerManager.localPlayer);
            return plys;
        }
    }
}
