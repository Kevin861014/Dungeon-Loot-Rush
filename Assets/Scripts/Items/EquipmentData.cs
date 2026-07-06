using UnityEngine;
using DungeonLootRush.Combat;

namespace DungeonLootRush.Items
{
    public enum EquipmentRarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }

    [CreateAssetMenu(menuName = "DungeonLootRush/Equipment Data", fileName = "NewEquipment")]
    public class EquipmentData : ScriptableObject
    {
        public string EquipmentId;
        public string DisplayName;
        public EquipmentRarity Rarity = EquipmentRarity.Common;
        public ProjectileBehavior Affix = ProjectileBehavior.Single;

        [Header("Base Stats (level 0, before upgrades)")]
        public float BaseAttack = 10f;
        public float BaseAttackSpeed = 1f;
        public float BaseDefense = 0f;
    }
}
