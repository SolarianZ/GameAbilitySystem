namespace GBG.GameAbilitySystem.Skill
{
    public interface ISkill
    {
        SkillData Data { get; }

        int InstanceId { get; }
    }
}