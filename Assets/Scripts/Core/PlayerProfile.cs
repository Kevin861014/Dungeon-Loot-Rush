using System;
using System.Collections.Generic;
using UnityEngine;
using DungeonLootRush.Items;
using DungeonLootRush.Economy;

namespace DungeonLootRush.Core
{
    [Serializable]
    public class TalentEntry
    {
        public string TalentId;
        public int Level;
    }

    /// Persistent, cross-run player data (GDD section 6): town currency, owned
    /// equipment and talents. Saved/loaded as JSON via PlayerPrefs.
    [Serializable]
    public class PlayerProfile
    {
        private const string SaveKey = "DungeonLootRush.PlayerProfile";

        public int Gold;
        public int UpgradeStones;
        public int BreakthroughMaterials;
        public List<EquipmentInstance> Equipment = new List<EquipmentInstance>();
        public List<TalentEntry> Talents = new List<TalentEntry>();

        public static PlayerProfile Load()
        {
            string json = PlayerPrefs.GetString(SaveKey, string.Empty);
            if (string.IsNullOrEmpty(json))
            {
                return new PlayerProfile();
            }

            return JsonUtility.FromJson<PlayerProfile>(json) ?? new PlayerProfile();
        }

        public void Save()
        {
            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(this));
            PlayerPrefs.Save();
        }

        public void ApplyRunResult(RunSessionResult result)
        {
            Gold += result.Gold;
            UpgradeStones += result.UpgradeStones;
            BreakthroughMaterials += result.BreakthroughMaterials;
            if (result.EquipmentDrops != null)
            {
                Equipment.AddRange(result.EquipmentDrops);
            }

            Save();
        }

        public void ApplyChestLoot(ChestLootResult loot)
        {
            Gold += loot.Gold;
            UpgradeStones += loot.UpgradeStones;
            BreakthroughMaterials += loot.BreakthroughMaterials;
            if (loot.EquipmentDrops != null)
            {
                Equipment.AddRange(loot.EquipmentDrops);
            }

            Save();
        }
    }
}
