using System;
using DungeonLootRush.Core;

namespace DungeonLootRush.Items
{
    /// Town chest per GDD 7.2: timed unlock, or watch an ad to skip the wait.
    public class TownChest : Chest
    {
        public DateTime UnlockTime { get; private set; }

        public bool IsUnlocked => DateTime.UtcNow >= UnlockTime;
        public TimeSpan TimeRemaining => IsUnlocked ? TimeSpan.Zero : UnlockTime - DateTime.UtcNow;

        private void Awake()
        {
            UnlockTime = DateTime.UtcNow + GetWaitDuration(Rarity);
        }

        public static TimeSpan GetWaitDuration(ChestRarity rarity)
        {
            switch (rarity)
            {
                case ChestRarity.Copper: return TimeSpan.FromMinutes(5);
                case ChestRarity.Silver: return TimeSpan.FromMinutes(30);
                case ChestRarity.Gold: return TimeSpan.FromHours(2);
                case ChestRarity.Legendary: return TimeSpan.FromHours(6);
                default: return TimeSpan.Zero;
            }
        }

        public void SkipWaitWithAd()
        {
            AdManager.Instance.ShowRewardedAd(AdPlacement.ChestSpeedup, success =>
            {
                if (success)
                {
                    UnlockTime = DateTime.UtcNow;
                }
            });
        }

        public bool TryOpen(out ChestLootResult loot)
        {
            if (!IsUnlocked)
            {
                loot = default;
                return false;
            }

            loot = Open();
            return true;
        }

        protected override void OnOpened(ChestLootResult loot)
        {
            GameManager.Instance.Profile.ApplyChestLoot(loot);
        }
    }
}
