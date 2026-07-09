using UnityEngine;
using DungeonLootRush.Combat;

namespace DungeonLootRush.Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Health))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float aggroRange = 8f;
        [SerializeField] private float contactDamage = 10f;
        [SerializeField] private float contactDamageInterval = 1f;

        private Rigidbody2D _rigidbody;
        private Transform _player;
        private float _damageTimer;

        public bool IsMoving { get; private set; }
        public Vector2 FacingDirection { get; private set; } = Vector2.down;

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
            if (_player == null || Vector2.Distance(_rigidbody.position, _player.position) > aggroRange)
            {
                _rigidbody.velocity = Vector2.zero;
                IsMoving = false;
                return;
            }

            Vector2 direction = ((Vector2)_player.position - _rigidbody.position).normalized;
            _rigidbody.velocity = direction * moveSpeed;
            FacingDirection = direction;
            IsMoving = true;
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
