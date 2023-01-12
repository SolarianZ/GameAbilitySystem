using GBG.GameAbilityProperty;
using GBG.GameAbilitySystem.Property;
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
        void AddCustomTag(string tag);

        /// <summary>
        /// 移除来自给定来源的自定义标签。
        /// </summary>
        /// <param name="tag">标签。</param>
        /// <returns>是否有任何标签被移除。</returns>
        bool RemoveCustomTag(string tag);

        /// <summary>
        /// 移除指定名称的自定义标签。
        /// </summary>
        /// <param name="tag">标签。</param>
        /// <returns>是否有任何标签被移除。</returns>
        bool RemoveAllCustomTagsOfName(string tag);

        /// <summary>
        /// 移除所有自定义标签。
        /// </summary>
        void RemoveAllCustomTags();
    }

    public delegate bool CheckTagActiveState(string tag, object instantContext);

    public sealed class TagContainer : ITagContainer, ITagProvider
    {
        public bool IsTagsDirty { get; private set; }

        private CheckTagActiveState? _tagActiveStateChecker;


        public event Action? OnTagsDirty;


        public TagContainer(CheckTagActiveState tagChecker)
        {
            SetTagActiveStateChecker(tagChecker);
        }

        public void SetTagActiveStateChecker(CheckTagActiveState tagChecker)
        {
            _tagActiveStateChecker = tagChecker;
        }

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

        public bool ContainsTag(string tag, object instantContext, bool ignoreInactiveTags)
        {
            if (_customTagTable.TryGetValue(tag, out var tagCount) && tagCount > 0)
            {
                var isCustomTagActive = _tagActiveStateChecker?.Invoke(tag, instantContext) ?? true;
                if (isCustomTagActive)
                {
                    return true;
                }
            }

            foreach (var tagProvider in _subTagProviders)
            {
                if (tagProvider.ContainsTag(tag, instantContext,
                    ignoreInactiveTags))
                {
                    return true;
                }
            }

            return false;
        }

        public int GetTagCount(string tag, object instantContext, bool ignoreInactiveTags)
        {
            var tagCount = 0;
            if (_customTagTable.TryGetValue(tag, out var customTagCount) && tagCount > 0)
            {
                var isCustomTagActive = _tagActiveStateChecker?.Invoke(tag, instantContext) ?? true;
                if (isCustomTagActive)
                {
                    tagCount += customTagCount;
                }
            }

            foreach (var tagProvider in _subTagProviders)
            {
                var subTagCount = tagProvider.GetTagCount(tag, instantContext, ignoreInactiveTags);
                tagCount += subTagCount;
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
        /// 值：此标签个数。
        /// </summary>
        private readonly Dictionary<string, int> _customTagTable
            = new Dictionary<string, int>();

        public void AddCustomTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException(nameof(tag), "Tag is null or empty.");
            }

            if (!_customTagTable.TryGetValue(tag, out var tagCount))
            {
                tagCount = 0;
            }

            _customTagTable[tag] = tagCount + 1;

            SetTagsDirty();
        }

        public bool RemoveCustomTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException(nameof(tag), "Tag is null or empty.");
            }

            if (!_customTagTable.TryGetValue(tag, out var tagCount))
            {
                // 没有tag
                return false;
            }

            if (tagCount == 0)
            {
                // 没有此tag，理论上不会执行到这里
                _customTagTable.Remove(tag);
                return false;
            }

            tagCount--;
            if (tagCount > 0)
            {
                _customTagTable[tag] = tagCount;
            }
            else
            {
                _customTagTable.Remove(tag);
            }

            // 标签已变化
            SetTagsDirty();

            return true;
        }

        public bool RemoveAllCustomTagsOfName(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException(nameof(tag), "Tag is null or empty.");
            }

            if (!_customTagTable.Remove(tag))
            {
                return false;
            }

            SetTagsDirty();

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
            foreach (var tagCount in _customTagTable.Values)
            {
                if (tagCount > 0)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
