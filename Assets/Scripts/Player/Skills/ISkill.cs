using UnityEngine;

namespace DungeonLootRush.Player.Skills
{
    public interface ISkill
    {
        float Cooldown { get; }
        void Activate(Transform caster);
    }
}
