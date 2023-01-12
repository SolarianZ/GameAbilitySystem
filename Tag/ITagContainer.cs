using System.Collections.Generic;

namespace GBG.GameAbilitySystem.Tag
{
    public interface ITagContainer : ITagProvider
    {
        bool IsTagsDirty { get; protected set; }

        ITagContainer? ParentTagContainer { get; set; }


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
        void AddCustomTag(string tag, object source);

        /// <summary>
        /// 移除来自给定来源的自定义标签。
        /// </summary>
        /// <param name="tag">标签。</param>
        /// <param name="source">标签来源。</param>
        /// <param name="count">移除数量。</param>
        /// <returns>是否有任何标签被移除。</returns>
        bool RemoveCustomTag(string tag, object source, int count = 1);

        /// <summary>
        /// 移除自定义标签，无论其来源为何。
        /// </summary>
        /// <param name="tag">标签。</param>
        /// <returns>是否有任何标签被移除。</returns>
        bool RemoveCustomTagIgnoreSource(string tag);

        /// <summary>
        /// 移除所有自定义标签。
        /// </summary>
        void RemoveAllCustomTags();
    }

    // todo: Tag缓存

    public sealed class TagContainer : ITagContainer, ITagProvider
    {
        public ITagContainer? ParentTagContainer { get; set; }

        public bool IsTagsDirty { get => _isTagsDirty; private set => _isTagsDirty = value; }

        bool ITagContainer.IsTagsDirty { get => _isTagsDirty; set => _isTagsDirty = value; }

        private bool _isTagsDirty;


        public bool HasAnyTag()
        {
            if (HasAnyCustomTag()) return true;
            if (HasAnyTagInProviders()) return true;
            return false;
        }

        public int GetTagCount(string tag, object source, bool ignoreInactiveTags)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new System.ArgumentNullException(nameof(tag), "Tag is null or empty.");
            }

            if (source == null)
            {
                throw new System.ArgumentNullException(nameof(source), "Tag source is null.");
            }

            var tagCount = 0;
            foreach (var provider in _tagProviders)
            {
                tagCount += provider.GetTagCount(tag, source, ignoreInactiveTags);
            }

            if (_tagTable.TryGetValue(tag, out Dictionary<object, int>? counterTable))
            {
                if (counterTable.TryGetValue(source, out var count))
                {
                    tagCount += count;
                }
            }

            return tagCount;
        }

        public int GetTagCountIgnoreSource(string tag, bool ignoreInactiveTags)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new System.ArgumentNullException(nameof(tag), "Tag is null or empty.");
            }

            var tagCount = 0;
            foreach (var provider in _tagProviders)
            {
                tagCount += provider.GetTagCountIgnoreSource(tag, ignoreInactiveTags);
            }

            if (_tagTable.TryGetValue(tag, out Dictionary<object, int>? counterTable))
            {
                foreach (var count in counterTable.Values)
                {
                    tagCount += count;
                }
            }

            return tagCount;
        }


        #region TagProvider

        private readonly List<ITagProvider> _tagProviders = new List<ITagProvider>();


        public bool AddTagProvider(ITagProvider tagProvider)
        {
            if (tagProvider == null)
            {
                throw new System.ArgumentNullException(nameof(tagProvider),
                    "Tag provider is null.");
            }

            if (_tagProviders.Contains(tagProvider))
            {
                return false;
            }

            _tagProviders.Add(tagProvider);
            if (tagProvider.HasAnyTag())
            {
                ((ITagContainer)this).SetTagsDirty();
            }

            return true;
        }

        public bool RemoveTagProvider(ITagProvider tagProvider)
        {
            if (tagProvider == null)
            {
                throw new System.ArgumentNullException(nameof(tagProvider),
                    "Tag provider is null.");
            }

            if (_tagProviders.Remove(tagProvider))
            {
                if (tagProvider.HasAnyTag())
                {
                    ((ITagContainer)this).SetTagsDirty();
                }

                return true;
            }

            return false;
        }


        private bool HasAnyTagInProviders()
        {
            foreach (var provider in _tagProviders)
            {
                if (provider.HasAnyTag())
                {
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region 自定义标签

        /// <summary>
        /// 标签表。
        /// 键：标签；
        /// 值：标签来源和此来源提供的此标签个数。
        /// </summary>
        private readonly Dictionary<string, Dictionary<object, int>> _tagTable = new Dictionary<string, Dictionary<object, int>>();

        public void AddCustomTag(string tag, object source)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new System.ArgumentNullException(nameof(tag), "Tag is null or empty.");
            }

            if (source == null)
            {
                throw new System.ArgumentNullException(nameof(source), "Tag source is null.");
            }

            if (!_tagTable.TryGetValue(tag, out Dictionary<object, int>? counterTable))
            {
                counterTable = new Dictionary<object, int>();
                _tagTable.Add(tag, counterTable);
            }

            if (!counterTable.TryGetValue(source, out var count))
            {
                count = 0;
            }

            counterTable[source] = count + 1;
        }

        public bool RemoveCustomTag(string tag, object source, int count = 1)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new System.ArgumentNullException(nameof(tag), "Tag is null or empty.");
            }

            if (source == null)
            {
                throw new System.ArgumentNullException(nameof(source), "Tag source is null.");
            }

            if (count < 1)
            {
                throw new System.ArgumentOutOfRangeException(nameof(count), "Tag removal count less than 1.");
            }

            if (!_tagTable.TryGetValue(tag, out Dictionary<object, int>? counterTable))
            {
                // 没有tag
                return false;
            }

            if (!counterTable.TryGetValue(source, out var tagCount))
            {
                // 没有来自source的tag
                return false;
            }

            if (tagCount == 0)
            {
                // 没有来自source的tag，理论上不会执行到这里
                CleanEmptyTable();
                return false;
            }

            tagCount -= count;
            if (tagCount > 0)
            {
                counterTable[source] = tagCount;
            }
            else
            {
                CleanEmptyTable();
            }

            // 标签已变化
            ((ITagContainer)this).SetTagsDirty();

            return true;

            void CleanEmptyTable()
            {
                counterTable.Remove(source);
                if (counterTable.Count == 0)
                {
                    _tagTable.Remove(tag);
                }
            }
        }

        public bool RemoveCustomTagIgnoreSource(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new System.ArgumentNullException(nameof(tag), "Tag is null or empty.");
            }

            if (!_tagTable.Remove(tag, out Dictionary<object, int>? counterTable))
            {
                return false;
            }

            // 检查是否有标签变化
            foreach (var count in counterTable.Values)
            {
                if (count > 0)
                {
                    ((ITagContainer)this).SetTagsDirty();
                    break;
                }
            }

            return true;
        }

        public void RemoveAllCustomTags()
        {
            var hasAnyCustomTag = HasAnyCustomTag();
            _tagTable.Clear();

            if (hasAnyCustomTag)
            {
                ((ITagContainer)this).SetTagsDirty();
            }
        }


        private bool HasAnyCustomTag()
        {
            foreach (Dictionary<object, int> counterTable in _tagTable.Values)
            {
                foreach (var count in counterTable.Values)
                {
                    if (count > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion
    }
}
