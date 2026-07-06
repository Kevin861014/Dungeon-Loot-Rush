using DungeonLootRush.Combat;
using DungeonLootRush.Core;
using DungeonLootRush.Economy;
using UnityEngine;

namespace DungeonLootRush.Player
{
    /// Death/revive flow (GDD 4.3): up to 3 ad-revives per run, otherwise settle.
    /// Room/enemy state is intentionally left untouched on revive.
    public class PlayerRevive : MonoBehaviour
    {
        [SerializeField] [Range(0f, 1f)] private float reviveHealthRatio = 0.5f;

        public void HandlePlayerDeath(Health health)
        {
            if (!RunSession.Instance.CanRevive)
            {
                RunResultSettlement.SettleAndReturnToTown();
                return;
            }

            AdManager.Instance.ShowRewardedAd(AdPlacement.DeathRevive, success =>
            {
                if (success)
                {
                    RunSession.Instance.RegisterRevive();
                    health.Revive(reviveHealthRatio);
                }
                else
                {
                    RunResultSettlement.SettleAndReturnToTown();
                }
            });
        }
    }
}
