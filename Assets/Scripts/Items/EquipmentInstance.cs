using System;

namespace DungeonLootRush.Items
{
    /// Runtime + save-data wrapper around an EquipmentData asset.
    /// Stores only the id (not the asset reference) so it survives JSON persistence.
    [Serializable]
    public class EquipmentInstance
    {
        public string EquipmentId;
        public int UpgradeLevel;

        [NonSerialized] private EquipmentData _cachedData;

        public EquipmentInstance()
        {
        }

        public EquipmentInstance(EquipmentData data, int upgradeLevel = 0)
        {
            EquipmentId = data.EquipmentId;
            UpgradeLevel = upgradeLevel;
            _cachedData = data;
        }

        public EquipmentData ResolveData(EquipmentDatabase database)
        {
            if (_cachedData == null)
            {
                _cachedData = database.GetById(EquipmentId);
            }

            return _cachedData;
        }

        public float GetAttack(EquipmentDatabase database)
        {
            return EquipmentUpgradeSystem.GetUpgradedValue(ResolveData(database).BaseAttack, UpgradeLevel);
        }

        public float GetAttackSpeed(EquipmentDatabase database)
        {
            return EquipmentUpgradeSystem.GetUpgradedValue(ResolveData(database).BaseAttackSpeed, UpgradeLevel, growthPerLevel: 0.01f);
        }
    }
}
