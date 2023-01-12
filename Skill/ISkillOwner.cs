namespace GBG.GameAbilitySystem.Skill
{
    public interface ISkillOwner
    {
        bool MeetSkillCosts(object skillCosts);

        bool CommitMeetSkillCosts(object skillCosts);
    }
}
