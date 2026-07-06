using UnityEngine;

namespace DungeonLootRush.Items
{
    public enum ChestRarity
    {
        Copper,
        Silver,
        Gold,
        Legendary
    }

    public struct ChestLootResult
    {
        public int Gold;
        public int UpgradeStones;
        public int BreakthroughMaterials;
        public EquipmentInstance[] EquipmentDrops;
    }

    public abstract class Chest : MonoBehaviour
    {
        public ChestRarity Rarity;
        public EquipmentDatabase EquipmentDatabase;

        private bool _isOpened;

        public ChestLootResult Open()
        {
            if (_isOpened)
            {
                return default;
            }

            _isOpened = true;
            var loot = RollLoot();
            OnOpened(loot);
            return loot;
        }

        protected virtual ChestLootResult RollLoot()
        {
            int rarityTier = (int)Rarity;
            var loot = new ChestLootResult
            {
                Gold = Random.Range(10, 30) * (rarityTier + 1),
                UpgradeStones = Rarity >= ChestRarity.Silver ? Random.Range(1, 4) * rarityTier : 0,
                BreakthroughMaterials = Rarity >= ChestRarity.Gold ? Random.Range(0, 2) : 0
            };

            if (EquipmentDatabase != null && Random.value < 0.2f + rarityTier * 0.15f)
            {
                var data = EquipmentDatabase.GetRandom(Rarity);
                if (data != null)
                {
                    loot.EquipmentDrops = new[] { new EquipmentInstance(data) };
                }
            }

            return loot;
        }

        protected abstract void OnOpened(ChestLootResult loot);
    }
}
