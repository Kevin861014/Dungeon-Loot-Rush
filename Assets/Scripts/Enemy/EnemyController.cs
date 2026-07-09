using UnityEngine;
using DungeonLootRush.Combat;

namespace DungeonLootRush.Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Health))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float contactDamage = 10f;
        [SerializeField] private float contactDamageInterval = 1f;

        private Rigidbody2D _rigidbody;
        private Transform _player;
        private float _damageTimer;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                _player = playerObject.transform;
            }
        }

        private void FixedUpdate()
        {
            if (_player == null)
            {
                return;
            }

            Vector2 direction = ((Vector2)_player.position - _rigidbody.position).normalized;
            _rigidbody.velocity = direction * moveSpeed;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            _damageTimer -= Time.deltaTime;
            if (_damageTimer > 0f)
            {
                return;
            }

            var damageable = other.GetComponent<IDamageable>();
            damageable?.ApplyDamage(contactDamage);
            _damageTimer = contactDamageInterval;
        }
    }
}
