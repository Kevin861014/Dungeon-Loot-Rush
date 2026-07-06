using UnityEngine;
using UnityEngine.UI;
using DungeonLootRush.Player;

namespace DungeonLootRush.UI
{
    public class SkillButtonUI : MonoBehaviour
    {
        [SerializeField] private SkillController skillController;
        [SerializeField] private int slotIndex;
        [SerializeField] private Image cooldownOverlay;
        [SerializeField] private Button button;

        private void Awake()
        {
            if (button != null)
            {
                button.onClick.AddListener(() => skillController.TryActivate(slotIndex));
            }
        }

        private void Update()
        {
            if (skillController == null || cooldownOverlay == null)
            {
                return;
            }

            float remaining = skillController.GetCooldownRemaining(slotIndex);
            float total = skillController.GetSkillCooldownDuration(slotIndex);
            cooldownOverlay.fillAmount = total > 0f ? remaining / total : 0f;
        }
    }
}
