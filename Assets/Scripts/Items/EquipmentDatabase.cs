using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonLootRush.Items
{
    [CreateAssetMenu(menuName = "DungeonLootRush/Equipment Database", fileName = "EquipmentDatabase")]
    public class EquipmentDatabase : ScriptableObject
    {
        public List<EquipmentData> AllEquipment = new List<EquipmentData>();

        public EquipmentData GetById(string equipmentId)
        {
            return AllEquipment.FirstOrDefault(e => e.EquipmentId == equipmentId);
        }

        public EquipmentData GetRandom(ChestRarity chestRarity)
        {
            var pool = AllEquipment.Where(e => (int)e.Rarity <= (int)chestRarity + 1).ToList();
            return pool.Count == 0 ? null : pool[Random.Range(0, pool.Count)];
        }
    }
}
