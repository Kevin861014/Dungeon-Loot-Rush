using System.Linq;
using UnityEngine;
using DungeonLootRush.Enemy;

namespace DungeonLootRush.Dungeon
{
    public class RoomInstance : MonoBehaviour
    {
        [SerializeField] private Transform entryPoint;
        [SerializeField] private Transform exitPoint;
        [SerializeField] private EnemySpawnPoint[] candidateSpawnPoints;

        public Transform EntryPoint => entryPoint;
        public Transform ExitPoint => exitPoint;

        /// Picks a random subset of the candidate points (GDD 3.3: 3-5 out of 5-8)
        /// so the same room template produces different fights each run.
        public void PopulateEnemies(int minCount, int maxCount)
        {
            if (candidateSpawnPoints == null || candidateSpawnPoints.Length == 0)
            {
                return;
            }

            int count = Mathf.Clamp(Random.Range(minCount, maxCount + 1), 0, candidateSpawnPoints.Length);
            var chosen = candidateSpawnPoints.OrderBy(_ => Random.value).Take(count);

            foreach (var spawnPoint in chosen)
            {
                var prefab = spawnPoint.GetRandomEnemyPrefab();
                if (prefab != null)
                {
                    Instantiate(prefab, spawnPoint.transform.position, Quaternion.identity, transform);
                }
            }
        }
    }
}
