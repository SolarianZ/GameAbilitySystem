namespace GBG.GameAbilitySystem.Tag
{
    public interface ITagProvider
    {
        int GetTagCount(string tag, object source);

        int GetTagCountIgnoreSource(string tag);
    }
}
