using Exiled.API.Features;
using MEC;

namespace PPluginRemastered.Modules.CrazyPills
{
    public class CPUtil
    {
        private static void SpawnGrenadeOnPlayer(Player player, GrenadeType grenadeType, float timer, float velocity = 1f)
        {
            bool fullForce = velocity >= 1;
            player.ThrowGrenade(grenadeType, fullForce);
        }

        private IEnumerator<float> GrenadeVomitTime(Player player, float randomTimer)
        {
            for (var i = 0; i < randomTimer * 10.0 && player.IsAlive; ++i)
            {
                yield return Timing.WaitForSeconds(plugin.Config.GrenadeVomitInterval);
                SpawnGrenadeOnPlayer(player, GrenadeType.FragGrenade, 5f);
            }
        }

        private IEnumerator<float> FlashVomitTime(Player player, float randomTimer)
        {
            for (var i = 0; i < randomTimer * 10.0 && player.IsAlive; ++i)
            {
                yield return Timing.WaitForSeconds(plugin.Config.FlashVomitInterval);
                player.Hurt(1);
                SpawnGrenadeOnPlayer(player, GrenadeType.Flashbang, 5f);
            }
        }

        private IEnumerator<float> BallVomitTime(Player player, float randomTimer)
        {
            for (var i = 0; i < randomTimer * 10.0 && player.IsAlive; ++i)
            {
                yield return Timing.WaitForSeconds(plugin.Config.BallVomitInterval);
                player.Hurt(1);
                SpawnGrenadeOnPlayer(player, GrenadeType.Scp018, 5f);
            }
        }
    }
}
