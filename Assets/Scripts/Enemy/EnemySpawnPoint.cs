using UnityEngine;

namespace DungeonLootRush.Enemy
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [SerializeField] private GameObject[] possibleEnemyPrefabs;

        public GameObject GetRandomEnemyPrefab()
        {
            if (possibleEnemyPrefabs == null || possibleEnemyPrefabs.Length == 0)
            {
                return null;
            }

            return possibleEnemyPrefabs[Random.Range(0, possibleEnemyPrefabs.Length)];
        }
    }
}
