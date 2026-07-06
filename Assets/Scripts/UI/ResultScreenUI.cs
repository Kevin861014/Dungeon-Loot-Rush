using UnityEngine;
using UnityEngine.UI;
using DungeonLootRush.Economy;

namespace DungeonLootRush.UI
{
    public class ResultScreenUI : MonoBehaviour
    {
        [SerializeField] private Text goldText;
        [SerializeField] private Text materialsText;
        [SerializeField] private Text equipmentCountText;

        public void Show(RunSessionResult result)
        {
            gameObject.SetActive(true);
            goldText.text = $"Gold: {result.Gold}";
            materialsText.text = $"Stones: {result.UpgradeStones}  Breakthrough: {result.BreakthroughMaterials}";
            equipmentCountText.text = $"Equipment Found: {result.EquipmentDrops?.Length ?? 0}";
        }
    }
}
