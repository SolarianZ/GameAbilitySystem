namespace GBG.GameAbilitySystem.Skill
{
    public interface ISkillManager
    {
        bool HasSkill(int skillId);

        bool GiveSkill(SkillSpec skillSpec);

        bool RemoveSkill(int skillInstanceId);

        bool RemoveAllSkills();

        bool BanSkill(int skillId);

        bool UnbanSkill(int skillId);
    }
}
