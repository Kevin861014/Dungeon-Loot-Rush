using UnityEngine;

namespace DungeonLootRush.Items
{
    public enum UpgradeCostType
    {
        GoldOnly,
        GoldAndStone,
        BreakthroughMaterial
    }

    public struct UpgradeCost
    {
        public UpgradeCostType Type;
        public int Gold;
        public int UpgradeStones;
        public int BreakthroughMaterials;
        public float SuccessChance;
    }

    /// Implements the tiered strengthen curve from GDD 6.1: 1-10 gold only,
    /// 11-30 gold+stone, 31+ breakthrough material with failure risk.
    public static class EquipmentUpgradeSystem
    {
        public const int MaxLevel = 99;
        private const int TierOneMax = 10;
        private const int TierTwoMax = 30;

        public static UpgradeCost GetUpgradeCost(int currentLevel)
        {
            int nextLevel = currentLevel + 1;

            if (nextLevel <= TierOneMax)
            {
                return new UpgradeCost
                {
                    Type = UpgradeCostType.GoldOnly,
                    Gold = 50 * nextLevel,
                    SuccessChance = 1f
                };
            }

            if (nextLevel <= TierTwoMax)
            {
                return new UpgradeCost
                {
                    Type = UpgradeCostType.GoldAndStone,
                    Gold = 100 * nextLevel,
                    UpgradeStones = nextLevel - TierOneMax,
                    SuccessChance = 1f
                };
            }

            return new UpgradeCost
            {
                Type = UpgradeCostType.BreakthroughMaterial,
                Gold = 200 * nextLevel,
                BreakthroughMaterials = 1 + (nextLevel - TierTwoMax) / 5,
                SuccessChance = Mathf.Clamp01(1f - (nextLevel - TierTwoMax) * 0.02f)
            };
        }

        public static float GetUpgradedValue(float baseValue, int level, float growthPerLevel = 0.05f)
        {
            return baseValue * (1f + level * growthPerLevel);
        }
    }
}
