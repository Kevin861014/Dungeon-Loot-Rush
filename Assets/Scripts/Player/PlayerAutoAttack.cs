using System.Collections.Generic;
using UnityEngine;
using DungeonLootRush.Combat;
using DungeonLootRush.Enemy;
using DungeonLootRush.Items;

namespace DungeonLootRush.Player
{
    /// Fully automatic attack loop (GDD 4.1): finds the nearest enemy, fires the
    /// equipped weapon's projectile pattern based on its affix (ProjectileBehavior).
    public class PlayerAutoAttack : MonoBehaviour
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float attackRange = 6f;
        [SerializeField] private LayerMask enemyMask;

        private EquipmentInstance _equippedWeapon;
        private EquipmentDatabase _equipmentDatabase;
        private float _cooldownTimer;

        public void EquipWeapon(EquipmentInstance weapon, EquipmentDatabase database)
        {
            _equippedWeapon = weapon;
            _equipmentDatabase = database;
        }

        private void Update()
        {
            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer > 0f || _equippedWeapon == null)
            {
                return;
            }

            var target = FindNearestEnemy();
            if (target == null)
            {
                return;
            }

            Fire(target.transform);
            _cooldownTimer = 1f / Mathf.Max(0.1f, _equippedWeapon.GetAttackSpeed(_equipmentDatabase));
        }

        private EnemyController FindNearestEnemy()
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyMask);
            EnemyController nearest = null;
            float bestDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                var enemy = hit.GetComponent<EnemyController>();
                if (enemy == null)
                {
                    continue;
                }

                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    nearest = enemy;
                }
            }

            return nearest;
        }

        private void Fire(Transform target)
        {
            var data = _equippedWeapon.ResolveData(_equipmentDatabase);
            float damage = _equippedWeapon.GetAttack(_equipmentDatabase);
            Vector2 baseDirection = (target.position - firePoint.position).normalized;

            foreach (var direction in GetFireDirections(data.Affix, baseDirection))
            {
                var projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                projectile.Damage = damage;
                projectile.Behavior = data.Affix;
                projectile.Launch(direction, data.Affix == ProjectileBehavior.Homing ? target : null);
            }
        }

        private static IEnumerable<Vector2> GetFireDirections(ProjectileBehavior affix, Vector2 baseDirection)
        {
            if (affix != ProjectileBehavior.Fan)
            {
                yield return baseDirection;
                yield break;
            }

            const int pelletCount = 3;
            const float spreadDegrees = 30f;
            float step = spreadDegrees / (pelletCount - 1);
            float start = -spreadDegrees / 2f;

            for (int i = 0; i < pelletCount; i++)
            {
                yield return Quaternion.Euler(0, 0, start + step * i) * baseDirection;
            }
        }
    }
}
