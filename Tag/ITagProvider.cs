namespace GBG.GameAbilitySystem.Tag
{
    public interface ITagProvider
    {
        int GetTagCount(string tag, object source, bool ignoreInactiveTags);

        int GetTagCountIgnoreSource(string tag, bool ignoreInactiveTags);
    }
}
