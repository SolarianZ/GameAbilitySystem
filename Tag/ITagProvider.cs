using System;

namespace GBG.GameAbilitySystem.Tag
{
    public interface ITagProvider
    {
        event Action OnTagsDirty;


        bool HasAnyTag();

        bool ContainsTag(string tag, object instantContext, bool ignoreInactiveTags);

        int GetTagCount(string tag, object instantContext, bool ignoreInactiveTags);
    }
}
