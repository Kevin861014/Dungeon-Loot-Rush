using UnityEngine;
using DungeonLootRush.Combat;

namespace DungeonLootRush.Enemy
{
    public class EnemyRangedAttack : MonoBehaviour
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private float fireInterval = 2f;
        [SerializeField] private float range = 5f;
        [SerializeField] private float damage = 8f;

        private Transform _player;
        private float _timer;

        private void Start()
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                _player = playerObject.transform;
            }
        }

        private void Update()
        {
            if (_player == null)
            {
                return;
            }

            _timer -= Time.deltaTime;
            float distance = Vector2.Distance(transform.position, _player.position);
            if (_timer > 0f || distance > range)
            {
                return;
            }

            var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.Damage = damage;
            projectile.Behavior = ProjectileBehavior.Single;
            projectile.Launch(((Vector2)_player.position - (Vector2)transform.position).normalized);

            _timer = fireInterval;
        }
    }
}
