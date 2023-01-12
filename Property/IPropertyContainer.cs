using GBG.GameAbilitySystem.Property;
using System;
using System.Collections.Generic;

namespace GBG.GameAbilityProperty
{
    public interface IPropertyContainer : IPropertyProvider
    {
        bool IsPropertiesDirty { get; }


        void SetPropertiesDirty();

        void CollectProperties();

        bool AddPropertyProvider(IPropertyProvider propertyProvider);

        bool RemovePropertyProvider(IPropertyProvider propertyProvider);


        /// <summary>
        /// 添加自定义属性。
        /// </summary>
        /// <param name="property">属性。</param>
        /// <returns>是否添加成功</returns>
        bool AddCustomProperty(Property property);

        /// <summary>
        /// 移除自定义属性。
        /// </summary>
        /// <param name="property">属性。</param>
        /// <returns>是否移除成功。</returns>
        bool RemoveCustomProperty(Property property);

        /// <summary>
        /// 移除指定Id的自定义属性。
        /// </summary>
        /// <param name="propertyId">属性。</param>
        /// <returns>是否有任何属性被移除。</returns>
        bool RemoveAllCustomPropertiesOfPropertyId(int propertyId);

        /// <summary>
        /// 移除所有自定义属性。
        /// </summary>
        void RemoveAllCustomProperties();
    }

    public sealed class PropertyContainer : IPropertyContainer, IPropertyProvider
    {
        public bool IsPropertiesDirty { get; private set; }


        public event Action? OnPropertiesDirty;


        public void SetPropertiesDirty()
        {
            if (IsPropertiesDirty)
            {
                return;
            }

            IsPropertiesDirty = true;
            OnPropertiesDirty?.Invoke();
        }

        // todo: Property缓存
        public void CollectProperties() => throw new NotImplementedException();

        public bool HasAnyProperty()
        {
            if (HasAnyCustomProperty()) return true;
            if (HasAnyPropertyInProviders()) return true;
            return false;
        }

        public bool ContainsProperty(int propertyId, object instantContext,
            bool ignoreInactiveProperties)
        {
            if (_customPropertyTable.ContainsKey(propertyId))
            {
                return true;
            }

            foreach (var propertyProvider in _subPropertyProviders)
            {
                if (propertyProvider.ContainsProperty(propertyId, instantContext,
                    ignoreInactiveProperties))
                {
                    return true;
                }
            }

            return false;
        }

        public double GetPropertyValue(int propertyId, object instantContext)
        {
            var propertyValue = 0.0;

            if (_customPropertyTable.TryGetValue(propertyId, out List<Property> properties))
            {
                foreach (var property in properties)
                {
                    propertyValue += property.Value;
                }
            }

            foreach (var propertyProvider in _subPropertyProviders)
            {
                var subPropValue = propertyProvider.GetPropertyValue(propertyId, instantContext);
                propertyValue += subPropValue;
            }

            return propertyValue;
        }

        public bool TryGetPropertyValue(int propertyId, object instantContext,
            out double propertyValue, out int propertySpecId)
        {
            var hasProperty = false;
            propertyValue = 0.0;
            propertySpecId = 0;

            if (_customPropertyTable.TryGetValue(propertyId, out List<Property> properties))
            {
                foreach (var property in properties)
                {
                    propertyValue += property.Value;

                    if (propertySpecId == 0)
                    {
                        propertySpecId = property.SpecId;
                    }
                    else if (propertySpecId != property.SpecId)
                    {
                        throw new PropertyException(propertyId,
                            $"Property spec id inconsistent, property.Id={propertyId}.");
                    }
                }

                hasProperty = properties.Count > 0;
            }

            foreach (var propertyProvider in _subPropertyProviders)
            {
                if (propertyProvider.TryGetPropertyValue(propertyId, instantContext,
                    out var tempPropertyValue, out var tempPropertySpecId))
                {
                    propertyValue += tempPropertyValue;

                    if (!hasProperty)
                    {
                        propertySpecId = tempPropertySpecId;
                        hasProperty = true;
                    }
                    else if (propertySpecId != tempPropertySpecId)
                    {
                        throw new PropertyException(propertyId,
                            $"Property spec id inconsistent, property.Id={propertyId}.");
                    }

                }
            }

            return hasProperty;
        }


        #region 子属性提供器

        private readonly List<IPropertyProvider> _subPropertyProviders = new List<IPropertyProvider>();


        public bool AddPropertyProvider(IPropertyProvider propertyProvider)
        {
            if (propertyProvider == null)
            {
                throw new ArgumentNullException(nameof(propertyProvider),
                    "Property provider is null.");
            }

            if (_subPropertyProviders.Contains(propertyProvider))
            {
                return false;
            }

            _subPropertyProviders.Add(propertyProvider);
            propertyProvider.OnPropertiesDirty += SetPropertiesDirty;

            if (propertyProvider.HasAnyProperty())
            {
                SetPropertiesDirty();
            }

            return true;
        }

        public bool RemovePropertyProvider(IPropertyProvider propertyProvider)
        {
            if (propertyProvider == null)
            {
                throw new ArgumentNullException(nameof(propertyProvider),
                    "Property provider is null.");
            }

            if (_subPropertyProviders.Remove(propertyProvider))
            {
                propertyProvider.OnPropertiesDirty -= SetPropertiesDirty;

                if (propertyProvider.HasAnyProperty())
                {
                    SetPropertiesDirty();
                }

                return true;
            }

            return false;
        }


        private bool HasAnyPropertyInProviders()
        {
            foreach (var provider in _subPropertyProviders)
            {
                if (provider.HasAnyProperty())
                {
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region 自定义属性

        /// <summary>
        /// 自定义属性表。
        /// 键：属性Id（<see cref="Property.Id"/>）；
        /// 值：相同属性Id的属性列表。
        /// </summary>
        private readonly Dictionary<int, List<Property>> _customPropertyTable
            = new Dictionary<int, List<Property>>();


        public bool AddCustomProperty(Property property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property), "Property is null.");
            }

            if (!_customPropertyTable.TryGetValue(property.Id, out List<Property> properties))
            {
                properties = new List<Property>();
                _customPropertyTable.Add(property.Id, properties);
            }

            if (properties.Contains(property))
            {
                return false;
            }

            properties.Add(property);
            property.OnValueChanged += OnCustomPropertyValueChanged;

            SetPropertiesDirty();

            return true;
        }

        public bool RemoveCustomProperty(Property property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property), "Property is null.");
            }

            if (!_customPropertyTable.TryGetValue(property.Id, out List<Property> properties))
            {
                // 没有property
                return false;
            }

            var index = properties.IndexOf(property);
            if (index == -1)
            {
                // 没有property
                return false;
            }

            properties.RemoveAt(index);
            property.OnValueChanged -= OnCustomPropertyValueChanged;

            // 清理空表
            if (properties.Count == 0)
            {
                _customPropertyTable.Remove(property.Id);
            }

            // 标签已变化
            SetPropertiesDirty();

            return true;
        }

        public bool RemoveAllCustomPropertiesOfPropertyId(int propertyId)
        {
            if (!_customPropertyTable.Remove(propertyId, out List<Property>? properties))
            {
                return false;
            }

            if (properties.Count > 0)
            {
                SetPropertiesDirty();
            }

            return true;
        }

        public void RemoveAllCustomProperties()
        {
            var hasAnyCustomProperty = HasAnyCustomProperty();
            _customPropertyTable.Clear();

            if (hasAnyCustomProperty)
            {
                SetPropertiesDirty();
            }
        }


        private bool HasAnyCustomProperty()
        {
            foreach (List<Property> properties in _customPropertyTable.Values)
            {
                if (properties.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnCustomPropertyValueChanged(Property property)
        {
            SetPropertiesDirty();
        }

        #endregion
    }
}
