using DungeonLootRush.Economy;

namespace DungeonLootRush.Items
{
    /// In-dungeon chest per GDD 7.1: opens instantly, loot feeds the current RunSession.
    public class DungeonChest : Chest
    {
        protected override void OnOpened(ChestLootResult loot)
        {
            RunSession.Instance.CollectLoot(loot);
        }
    }
}
