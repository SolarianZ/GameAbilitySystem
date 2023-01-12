namespace GBG.GameAbilitySystem.Tag
{
    public interface ITagContainer : ITagProvider
    {
        bool IsTagsDirty { get; protected set; }

        ITagContainer ParentTagContainer { get; set; }


        void SetTagsDirty()
        {
            IsTagsDirty = true;
            ParentTagContainer?.SetTagsDirty();
        }

        bool AddTagProvider(ITagProvider tagProvider);

        bool RemoveTagProvider(ITagProvider tagProvider);


        /// <summary>
        /// 添加自定义标签。
        /// </summary>
        /// <param name="tag">标签。</param>
        /// <param name="source">标签来源。</param>
        /// <returns>此方法执行完成后，给定标签来源所提供的给定标签的数量。</returns>
        int AddCustomTag(string tag, object source);

        /// <summary>
        /// 移除来自给定来源的自定义标签。
        /// </summary>
        /// <param name="tag">标签。</param>
        /// <param name="source">标签来源。</param>
        /// <param name="count">移除数量。</param>
        /// <returns>此方法执行完成后，实际移除的给定标签的数量。</returns>
        int RemoveCustomTag(string tag, object source, int count = 1);

        /// <summary>
        /// 移除自定义标签，无论其来源为何。
        /// </summary>
        /// <param name="tag">标签。</param>
        /// <returns>此方法执行完成后，实际移除的给定标签的数量。</returns>
        int RemoveCustomTagIgnoreSource(string tag);

        /// <summary>
        /// 移除所有自定义标签。
        /// </summary>
        void RemoveAllCustomTags();
    }

    // { Tag : { TagSource : TagCount } }
    // Dictionary<string, Dictionary<object, int>> _tagTable;
}
