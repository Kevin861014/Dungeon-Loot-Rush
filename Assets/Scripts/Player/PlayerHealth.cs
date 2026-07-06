using UnityEngine;
using DungeonLootRush.Combat;

namespace DungeonLootRush.Player
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(PlayerRevive))]
    public class PlayerHealth : MonoBehaviour
    {
        private Health _health;
        private PlayerRevive _revive;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _revive = GetComponent<PlayerRevive>();
        }

        private void OnEnable()
        {
            _health.OnDeath += HandleDeath;
        }

        private void OnDisable()
        {
            _health.OnDeath -= HandleDeath;
        }

        private void HandleDeath()
        {
            _revive.HandlePlayerDeath(_health);
        }
    }
}
