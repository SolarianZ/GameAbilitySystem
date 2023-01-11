namespace GBG.GameAbilitySystem.Skill
{
    // todo: Skill Cost
    // todo: Skill Exclusion
    // todo: Skill Family
    // todo: Skill Level (Dynamic Level?)
    // todo: RemoveAfterActivation
    // todo: ActivateOnceOnGranted

    public interface ISkill
    {
        SkillSpec SkillSpec { get; }

        int InstanceId { get; }

        bool IsActive { get; }

        bool CanActivateSkill { get; }

        bool CanCancelSkill { get; }


        // todo: event Action OnSkillActivated;
        // todo: event Action OnSkillCanceled;
        // todo: event Action OnSkillEnded;


        uint GetCooldownTimeRemaining();

        bool TryActivateSkill();

        bool TryCancelSkill();

        // todo: void Tick(uint deltaTimeMs);
    }
}