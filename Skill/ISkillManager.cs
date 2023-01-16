namespace GBG.GameAbilitySystem.Skill
{
    public interface ISkillManager
    {
        // Tick顺序：
        //     0. 【外部】给予和移除技能
        //     1. 技能
        //     2. 技能激活器
        void Tick(uint deltaTime);

        bool HasSkill(int skillId);

        bool GiveSkill(ISkill skill);

        bool RemoveSkill(int skillId);

        void RemoveAllSkills();

        bool BanSkill(int skillId);

        bool UnbanSkill(int skillId);
    }
}
