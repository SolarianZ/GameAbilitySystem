using System;

namespace GBG.GameAbilitySystem.Tag
{
    public interface ITagProvider
    {
        event Action OnTagsDirty;


        bool HasAnyTag();

        int GetTagCount(string tag, object source, bool ignoreInactiveTags);

        int GetTagCountIgnoreSource(string tag, bool ignoreInactiveTags);
    }
}
