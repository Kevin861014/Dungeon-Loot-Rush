using System;
using UnityEngine;
using UnityEngine.UI;
using DungeonLootRush.Items;

namespace DungeonLootRush.UI
{
    public class ChestTimerUI : MonoBehaviour
    {
        [SerializeField] private TownChest chest;
        [SerializeField] private Text timerText;
        [SerializeField] private Button watchAdButton;
        [SerializeField] private Button openButton;

        private void Awake()
        {
            if (watchAdButton != null)
            {
                watchAdButton.onClick.AddListener(() => chest.SkipWaitWithAd());
            }

            if (openButton != null)
            {
                openButton.onClick.AddListener(HandleOpen);
            }
        }

        private void Update()
        {
            if (chest == null)
            {
                return;
            }

            bool unlocked = chest.IsUnlocked;
            timerText.text = unlocked ? "Ready!" : FormatTime(chest.TimeRemaining);
            watchAdButton.gameObject.SetActive(!unlocked);
            openButton.gameObject.SetActive(unlocked);
        }

        private void HandleOpen()
        {
            if (chest.TryOpen(out _))
            {
                gameObject.SetActive(false);
            }
        }

        private static string FormatTime(TimeSpan time)
        {
            return time.TotalHours >= 1
                ? $"{(int)time.TotalHours:00}:{time.Minutes:00}:{time.Seconds:00}"
                : $"{time.Minutes:00}:{time.Seconds:00}";
        }
    }
}
