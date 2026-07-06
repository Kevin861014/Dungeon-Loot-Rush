using System;
using UnityEngine;

namespace DungeonLootRush.Core
{
    public enum AdPlacement
    {
        DeathRevive,
        ChestSpeedup
    }

    public interface IAdProvider
    {
        void ShowRewardedAd(AdPlacement placement, Action<bool> onComplete);
    }

    /// Stub ad provider (GDD sections 8 & 10). Always "succeeds" so the rest of the
    /// game can be built and tested; swap the body for a LevelPlay/AppLovin MAX call
    /// when the real SDK is integrated.
    public class AdManager : MonoBehaviour, IAdProvider
    {
        public static AdManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ShowRewardedAd(AdPlacement placement, Action<bool> onComplete)
        {
            Debug.Log($"[AdManager] Stub rewarded ad for {placement}.");
            onComplete?.Invoke(true);
        }
    }
}
