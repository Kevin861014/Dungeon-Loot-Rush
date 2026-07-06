using System;
using UnityEngine;

namespace DungeonLootRush.Combat
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 100f;

        public float CurrentHealth { get; private set; }
        public float MaxHealth => maxHealth;
        public bool IsAlive => CurrentHealth > 0f;

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void SetMaxHealth(float value, bool refill)
        {
            maxHealth = value;
            if (refill)
            {
                CurrentHealth = maxHealth;
            }

            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void ApplyDamage(float amount)
        {
            if (!IsAlive || amount <= 0f)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

            if (CurrentHealth <= 0f)
            {
                OnDeath?.Invoke();
            }
        }

        public void Heal(float amount)
        {
            if (!IsAlive || amount <= 0f)
            {
                return;
            }

            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void Revive(float ratio)
        {
            CurrentHealth = Mathf.Clamp(maxHealth * ratio, 1f, maxHealth);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }
    }
}
