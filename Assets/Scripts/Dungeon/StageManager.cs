using UnityEngine;

namespace DungeonLootRush.Dungeon
{
    /// Drives floor progression per GDD 3.1: 4 regular rooms + 1 boss room per
    /// stage, stages increase indefinitely in difficulty/loot quality.
    public class StageManager : MonoBehaviour
    {
        private const int RoomsPerStageBeforeBoss = 4;

        [SerializeField] private DungeonGenerator generator;
        [SerializeField] private Transform dungeonRoot;

        public int CurrentStage { get; private set; } = 1;

        private void Start()
        {
            GenerateStage();
        }

        public void GenerateStage()
        {
            generator.SpawnStage(RoomsPerStageBeforeBoss, dungeonRoot);
        }

        public void AdvanceToNextStage()
        {
            CurrentStage++;

            foreach (Transform child in dungeonRoot)
            {
                Destroy(child.gameObject);
            }

            GenerateStage();
        }
    }
}
