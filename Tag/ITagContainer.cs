using GBG.GameAbilitySystem.Tag;
using System;
using System.Collections.Generic;

namespace GBG.GameAbilityTag
{
    public interface ITagContainer : ITagProvider
    {
        bool IsTagsDirty { get; }


        void SetTagsDirty();

        void CollectTags();

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

    public sealed class TagContainer : ITagContainer, ITagProvider
    {
        public bool IsTagsDirty { get; private set; }

        public event Action? OnTagsDirty;


        public void SetTagsDirty()
        {
            if (IsTagsDirty)
            {
                return;
            }

            IsTagsDirty = true;
            OnTagsDirty?.Invoke();
        }

        // todo: Tag缓存
        public void CollectTags() => throw new NotImplementedException();

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
                throw new ArgumentNullException(nameof(tag), "Tag is null or empty.");
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "Tag source is null.");
            }

            var tagCount = 0;
            foreach (var provider in _subTagProviders)
            {
                tagCount += provider.GetTagCount(tag, source, ignoreInactiveTags);
            }

            if (_customTagTable.TryGetValue(tag, out Dictionary<object, int> counterTable))
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
                throw new ArgumentNullException(nameof(tag), "Tag is null or empty.");
            }

            var tagCount = 0;
            foreach (var provider in _subTagProviders)
            {
                tagCount += provider.GetTagCountIgnoreSource(tag, ignoreInactiveTags);
            }

            if (_customTagTable.TryGetValue(tag, out Dictionary<object, int> counterTable))
            {
                foreach (var count in counterTable.Values)
                {
                    tagCount += count;
                }
            }

            return tagCount;
        }


        #region 子标签提供器

        private readonly List<ITagProvider> _subTagProviders = new List<ITagProvider>();


        public bool AddTagProvider(ITagProvider tagProvider)
        {
            if (tagProvider == null)
            {
                throw new ArgumentNullException(nameof(tagProvider),
                    "Tag provider is null.");
            }

            if (_subTagProviders.Contains(tagProvider))
            {
                return false;
            }

            _subTagProviders.Add(tagProvider);
            tagProvider.OnTagsDirty += SetTagsDirty;

            if (tagProvider.HasAnyTag())
            {
                SetTagsDirty();
            }

            return true;
        }

        public bool RemoveTagProvider(ITagProvider tagProvider)
        {
            if (tagProvider == null)
            {
                throw new ArgumentNullException(nameof(tagProvider),
                    "Tag provider is null.");
            }

            if (_subTagProviders.Remove(tagProvider))
            {
                tagProvider.OnTagsDirty -= SetTagsDirty;

                if (tagProvider.HasAnyTag())
                {
                    SetTagsDirty();
                }

                return true;
            }

            return false;
        }


        private bool HasAnyTagInProviders()
        {
            foreach (var provider in _subTagProviders)
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
        /// 自定义标签表。
        /// 键：标签；
        /// 值：标签来源和此来源提供的此标签个数。
        /// </summary>
        private readonly Dictionary<string, Dictionary<object, int>> _customTagTable
            = new Dictionary<string, Dictionary<object, int>>();

        public void AddCustomTag(string tag, object source)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException(nameof(tag), "Tag is null or empty.");
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "Tag source is null.");
            }

            if (!_customTagTable.TryGetValue(tag, out Dictionary<object, int> counterTable))
            {
                counterTable = new Dictionary<object, int>();
                _customTagTable.Add(tag, counterTable);
            }

            if (!counterTable.TryGetValue(source, out var count))
            {
                count = 0;
            }

            counterTable[source] = count + 1;

            SetTagsDirty();
        }

        public bool RemoveCustomTag(string tag, object source, int count = 1)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException(nameof(tag), "Tag is null or empty.");
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "Tag source is null.");
            }

            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Tag removal count less than 1.");
            }

            if (!_customTagTable.TryGetValue(tag, out Dictionary<object, int> counterTable))
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
            SetTagsDirty();

            return true;

            void CleanEmptyTable()
            {
                counterTable.Remove(source);
                if (counterTable.Count == 0)
                {
                    _customTagTable.Remove(tag);
                }
            }
        }

        public bool RemoveCustomTagIgnoreSource(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException(nameof(tag), "Tag is null or empty.");
            }

            if (!_customTagTable.Remove(tag, out Dictionary<object, int> counterTable))
            {
                return false;
            }

            // 检查是否有标签变化
            foreach (var count in counterTable.Values)
            {
                if (count > 0)
                {
                    SetTagsDirty();
                    break;
                }
            }

            return true;
        }

        public void RemoveAllCustomTags()
        {
            var hasAnyCustomTag = HasAnyCustomTag();
            _customTagTable.Clear();

            if (hasAnyCustomTag)
            {
                SetTagsDirty();
            }
        }


        private bool HasAnyCustomTag()
        {
            foreach (Dictionary<object, int> counterTable in _customTagTable.Values)
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
