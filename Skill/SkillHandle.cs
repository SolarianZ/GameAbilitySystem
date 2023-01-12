namespace GBG.GameAbilitySystem.Skill
{
    // 用于在线游戏时，需要保证技能中的所有状态可以复制，当前版本不满足此需求。

    public readonly struct SkillHandle
    {
        public readonly int SkillId;

        public readonly int SkillFamilyId;

        public readonly int SkillInstanceId;
    }
}