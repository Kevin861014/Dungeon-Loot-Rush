using UnityEngine;

namespace DungeonLootRush.Dungeon
{
    /// One entry in the room layout template library described in GDD 3.3.
    /// The prefab must contain a RoomInstance with an entry point, exit point,
    /// and 5-8 candidate enemy spawn points.
    [CreateAssetMenu(menuName = "DungeonLootRush/Room Template", fileName = "NewRoomTemplate")]
    public class RoomTemplateData : ScriptableObject
    {
        public string TemplateId;
        public RoomType Type = RoomType.Combat;
        public GameObject RoomPrefab;

        [Tooltip("Minimum candidate spawn points to activate.")]
        public int MinActiveSpawnPoints = 3;

        [Tooltip("Maximum candidate spawn points to activate.")]
        public int MaxActiveSpawnPoints = 5;
    }
}
