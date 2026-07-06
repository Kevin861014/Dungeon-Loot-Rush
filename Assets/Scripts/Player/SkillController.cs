using System.Collections.Generic;
using UnityEngine;
using DungeonLootRush.Player.Skills;

namespace DungeonLootRush.Player
{
    /// Holds the 1-2 skill button slots from GDD 4.1. Skills are ScriptableObjects
    /// implementing ISkill so new skills can be added without touching this class.
    public class SkillController : MonoBehaviour
    {
        [SerializeField] private List<ScriptableObject> skillSlots = new List<ScriptableObject>();

        private readonly Dictionary<int, float> _cooldownTimers = new Dictionary<int, float>();

        private void Update()
        {
            var slots = new List<int>(_cooldownTimers.Keys);
            foreach (var slot in slots)
            {
                _cooldownTimers[slot] = Mathf.Max(0f, _cooldownTimers[slot] - Time.deltaTime);
            }
        }

        public float GetCooldownRemaining(int slotIndex)
        {
            return _cooldownTimers.TryGetValue(slotIndex, out var value) ? value : 0f;
        }

        public float GetSkillCooldownDuration(int slotIndex)
        {
            return slotIndex >= 0 && slotIndex < skillSlots.Count && skillSlots[slotIndex] is ISkill skill
                ? skill.Cooldown
                : 0f;
        }

        public bool TryActivate(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= skillSlots.Count || GetCooldownRemaining(slotIndex) > 0f)
            {
                return false;
            }

            if (!(skillSlots[slotIndex] is ISkill skill))
            {
                return false;
            }

            skill.Activate(transform);
            _cooldownTimers[slotIndex] = skill.Cooldown;
            return true;
        }
    }
}
