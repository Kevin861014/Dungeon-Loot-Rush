using DungeonLootRush.Core;

namespace DungeonLootRush.Economy
{
    /// Entry point for the "death exhausted or give up" flow described in GDD section 5:
    /// moves the run's loot into the persistent PlayerProfile and returns to town.
    public static class RunResultSettlement
    {
        public static void SettleAndReturnToTown()
        {
            var result = RunSession.Instance.ToResult();
            GameManager.Instance.Profile.ApplyRunResult(result);
            GameManager.Instance.GoToTown();
        }
    }
}
