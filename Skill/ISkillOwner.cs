namespace GBG.GameAbilitySystem.Skill
{
    public interface ISkillOwner
    {
        bool MeetSkillActivationCosts(object costs);

        bool CommitSkillActivationCosts(object costs);
    }
}
