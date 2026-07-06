using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DungeonLootRush.Items;

namespace DungeonLootRush.Economy
{
    public struct RunSessionResult
    {
        public int Gold;
        public int UpgradeStones;
        public int BreakthroughMaterials;
        public EquipmentInstance[] EquipmentDrops;
    }

    /// Tracks everything collected during a single dungeon run (GDD section 5).
    /// Cleared each time a new run starts; only settled into PlayerProfile on death/give-up.
    public class RunSession : MonoBehaviour
    {
        public const int MaxRevives = 3;

        public static RunSession Instance { get; private set; }

        public int Gold { get; private set; }
        public int UpgradeStones { get; private set; }
        public int BreakthroughMaterials { get; private set; }
        public List<EquipmentInstance> EquipmentDrops { get; } = new List<EquipmentInstance>();
        public int RevivesUsed { get; private set; }
        public bool CanRevive => RevivesUsed < MaxRevives;

        private void Awake()
        {
            Instance = this;
        }

        public void CollectLoot(ChestLootResult loot)
        {
            Gold += loot.Gold;
            UpgradeStones += loot.UpgradeStones;
            BreakthroughMaterials += loot.BreakthroughMaterials;
            if (loot.EquipmentDrops != null)
            {
                EquipmentDrops.AddRange(loot.EquipmentDrops);
            }
        }

        public void AddMerchantGold(int amount)
        {
            Gold += amount;
        }

        public bool TrySpendGold(int amount)
        {
            if (Gold < amount)
            {
                return false;
            }

            Gold -= amount;
            return true;
        }

        public void RegisterRevive()
        {
            RevivesUsed++;
        }

        public RunSessionResult ToResult()
        {
            return new RunSessionResult
            {
                Gold = Gold,
                UpgradeStones = UpgradeStones,
                BreakthroughMaterials = BreakthroughMaterials,
                EquipmentDrops = EquipmentDrops.ToArray()
            };
        }
    }
}
