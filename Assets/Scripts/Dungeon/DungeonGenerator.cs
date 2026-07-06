using System.Collections.Generic;
using UnityEngine;

namespace DungeonLootRush.Dungeon
{
    /// Implements the "template library + content randomization" approach from
    /// GDD 3.3: room skeletons are pre-authored, only which template and which
    /// enemies get picked is randomized.
    public class DungeonGenerator : MonoBehaviour
    {
        [SerializeField] private List<RoomTemplateData> combatTemplates;
        [SerializeField] private List<RoomTemplateData> merchantTemplates;
        [SerializeField] private List<RoomTemplateData> extraChestTemplates;
        [SerializeField] private RoomTemplateData bossTemplate;

        [Range(0f, 1f)] [SerializeField] private float merchantRoomChance = 0.25f;
        [Range(0f, 1f)] [SerializeField] private float extraChestRoomChance = 0.35f;

        public RoomInstance SpawnStage(int roomsBeforeBoss, Transform parent)
        {
            RoomInstance previousRoom = null;

            for (int i = 0; i < roomsBeforeBoss; i++)
            {
                previousRoom = SpawnRoom(PickTemplateForSlot(), parent, previousRoom);
            }

            return SpawnRoom(bossTemplate, parent, previousRoom);
        }

        private RoomTemplateData PickTemplateForSlot()
        {
            if (extraChestTemplates.Count > 0 && Random.value < extraChestRoomChance)
            {
                return extraChestTemplates[Random.Range(0, extraChestTemplates.Count)];
            }

            if (merchantTemplates.Count > 0 && Random.value < merchantRoomChance)
            {
                return merchantTemplates[Random.Range(0, merchantTemplates.Count)];
            }

            return combatTemplates[Random.Range(0, combatTemplates.Count)];
        }

        private RoomInstance SpawnRoom(RoomTemplateData template, Transform parent, RoomInstance previousRoom)
        {
            var instance = Instantiate(template.RoomPrefab, parent).GetComponent<RoomInstance>();

            if (previousRoom != null)
            {
                instance.transform.position = previousRoom.ExitPoint.position - instance.EntryPoint.localPosition;
            }

            if (template.Type == RoomType.Combat)
            {
                instance.PopulateEnemies(template.MinActiveSpawnPoints, template.MaxActiveSpawnPoints);
            }

            return instance;
        }
    }
}
