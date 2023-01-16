namespace GBG.GameAbilitySystem.Skill
{
    public readonly struct SkillHandle
    {
        public readonly int SkillInstanceId;

        public readonly int SkillId;

        public readonly int SkillFamilyId;

        //public readonly int SkillLevel;

        public SkillHandle(int skillInstanceId, int skillId, int skillFamilyId)
        {
            SkillInstanceId = skillInstanceId;
            SkillId = skillId;
            SkillFamilyId = skillFamilyId;
        }
    }
}