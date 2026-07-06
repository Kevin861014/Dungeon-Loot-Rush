using UnityEngine;
using DungeonLootRush.Combat;

namespace DungeonLootRush.Player.Skills
{
    /// First concrete active skill for the MVP skill button (GDD 4.1): damages
    /// every enemy in a radius around the player, then goes on cooldown.
    [CreateAssetMenu(menuName = "DungeonLootRush/Skills/AoE Burst", fileName = "AoeBurstSkill")]
    public class AoeBurstSkill : ScriptableObject, ISkill
    {
        public float cooldown = 5f;
        public float radius = 3f;
        public float damage = 30f;
        public LayerMask enemyMask;

        public float Cooldown => cooldown;

        public void Activate(Transform caster)
        {
            foreach (var hit in Physics2D.OverlapCircleAll(caster.position, radius, enemyMask))
            {
                var damageable = hit.GetComponent<IDamageable>();
                damageable?.ApplyDamage(damage);
            }
        }
    }
}
