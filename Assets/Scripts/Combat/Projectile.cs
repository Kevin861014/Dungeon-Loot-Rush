using UnityEngine;

namespace DungeonLootRush.Combat
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        public float Speed = 10f;
        public float Damage = 10f;
        public float Lifetime = 3f;
        public ProjectileBehavior Behavior = ProjectileBehavior.Single;
        public LayerMask HitMask;
        public int MaxBounces = 3;
        public int MaxPierces = 3;

        private Rigidbody2D _rigidbody;
        private Vector2 _direction;
        private Transform _homingTarget;
        private int _remainingBounces;
        private int _remainingPierces;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _remainingBounces = MaxBounces;
            _remainingPierces = MaxPierces;
        }

        public void Launch(Vector2 direction, Transform homingTarget = null)
        {
            _direction = direction.normalized;
            _homingTarget = homingTarget;
            _rigidbody.linearVelocity = _direction * Speed;
            Destroy(gameObject, Lifetime);
        }

        private void FixedUpdate()
        {
            if (Behavior == ProjectileBehavior.Homing && _homingTarget != null)
            {
                _direction = ((Vector2)_homingTarget.position - _rigidbody.position).normalized;
                _rigidbody.linearVelocity = _direction * Speed;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & HitMask) == 0)
            {
                return;
            }

            var damageable = other.GetComponent<IDamageable>();
            damageable?.ApplyDamage(Damage);

            if (Behavior == ProjectileBehavior.Pierce && _remainingPierces > 0)
            {
                _remainingPierces--;
                return;
            }

            Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (Behavior != ProjectileBehavior.Bounce || _remainingBounces <= 0)
            {
                Destroy(gameObject);
                return;
            }

            _remainingBounces--;
            _direction = Vector2.Reflect(_direction, collision.contacts[0].normal);
            _rigidbody.linearVelocity = _direction * Speed;
        }
    }
}
