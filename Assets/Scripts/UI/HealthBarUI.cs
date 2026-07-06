using UnityEngine;
using UnityEngine.UI;
using DungeonLootRush.Combat;

namespace DungeonLootRush.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private Image fillImage;

        private void OnEnable()
        {
            if (health == null)
            {
                return;
            }

            health.OnHealthChanged += HandleHealthChanged;
            HandleHealthChanged(health.CurrentHealth, health.MaxHealth);
        }

        private void OnDisable()
        {
            if (health != null)
            {
                health.OnHealthChanged -= HandleHealthChanged;
            }
        }

        private void HandleHealthChanged(float current, float max)
        {
            fillImage.fillAmount = max > 0f ? current / max : 0f;
        }
    }
}
