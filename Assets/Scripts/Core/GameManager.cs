using UnityEngine;
using UnityEngine.SceneManagement;

namespace DungeonLootRush.Core
{
    public enum GameScene
    {
        Boot,
        Town,
        Dungeon
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public PlayerProfile Profile { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Profile = PlayerProfile.Load();
        }

        public void GoToTown()
        {
            Profile.Save();
            SceneManager.LoadScene(GameScene.Town.ToString());
        }

        public void StartDungeonRun()
        {
            SceneManager.LoadScene(GameScene.Dungeon.ToString());
        }

        private void OnApplicationQuit()
        {
            Profile.Save();
        }
    }
}
