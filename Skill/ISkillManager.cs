namespace GBG.GameAbilitySystem.Skill
{
    public interface ISkillManager
    {
        //todo: bool HasSkill();

        bool GiveSkill(SkillSpec skillSpec);

        bool RemoveSkill(int skillInstanceId);

        bool RemoveAllSkills();
    }
}
