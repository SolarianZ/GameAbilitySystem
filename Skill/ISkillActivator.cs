namespace GBG.GameAbilitySystem.Skill
{
    public interface ISkillActivator
    {
        void Tick(uint deltaTime);

        void OnSkillEquipped(ISkill skill);

        void OnSkillUnequipped(ISkill skill);

        void Initialize(ISkillOwner skillOwner, object? persistentContext);
    }
}
