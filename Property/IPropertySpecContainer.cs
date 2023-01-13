using System;
using System.Collections.Generic;

namespace GBG.GameAbilitySystem.Property
{
    public interface IPropertySpecContainer : IPropertySpecProvider
    {
        void RegisterPropertySpec(PropertySpec propertySpec);

        bool UnregisterPropertySpec(int propertySpecId);


        void RegisterPropertyRelation(int propertySpecId, int propertyId);

        bool UnregisterPropertyRelation(int propertySpecId, int propertyId);
    }

    public class PropertySpecContainer : IPropertySpecContainer, IPropertySpecProvider
    {
        /// <summary>
        /// 属性族关联表。
        /// 键：属性族Id（<see cref="PropertySpec.FamilyId"/>）；
        /// 值：族内属性规则Id（<see cref="PropertySpec.Id"/>）表。
        /// </summary>
        // PropertySpec.FamilyId : PropertySpec.Id[]
        private readonly Dictionary<int, List<int>> _familyRelationTable = new Dictionary<int, List<int>>();

        /// <summary>
        /// 属性规则关联表。
        /// 键：属性规则Id（<see cref="PropertySpec.Id"/>）；
        /// 值：使用此规则的属性Id（<see cref="Property.Id"/>）表。
        /// </summary>
        // PropertySpec.Id : Property.Id[]
        private readonly Dictionary<int, List<int>> _specRelationTable = new Dictionary<int, List<int>>();

        /// <summary>
        /// 属性规则注册表。
        /// 键：属性规则Id（<see cref="PropertySpec.Id"/>）；
        /// 值：属性规则。
        /// </summary>
        // PropertySpec.Id : PropertySpec
        private readonly Dictionary<int, PropertySpec> _specTable = new Dictionary<int, PropertySpec>();


        #region Provider

        public bool ContainsPropertySpec(int propertySpecId)
        {
            return _specTable.ContainsKey(propertySpecId);
        }

        public bool TryGetPropertySpec(int propertySpecId, out PropertySpec propertySpec)
        {
            return _specTable.TryGetValue(propertySpecId, out propertySpec);
        }


        public int GetPropertyIdCountOfFamily(int propertyFamilyId)
        {
            if (!_familyRelationTable.TryGetValue(propertyFamilyId, out List<int> specIds))
            {
                return 0;
            }

            var propIdCount = 0;
            foreach (var specId in specIds)
            {
                List<int> propIds = _specRelationTable[specId];
                propIdCount += propIds.Count;
            }

            return propIdCount;
        }

        public void GetPropertyIdsOfFamily(int propertyFamilyId, Span<int> result)
        {
            if (!_familyRelationTable.TryGetValue(propertyFamilyId, out List<int> specIds))
            {
                if (result.Length != 0)
                {
                    ThrowCapacityInconsistentException();
                }

                return;
            }

            var index = 0;
            foreach (var specId in specIds)
            {
                List<int> propIds = _specRelationTable[specId];
                foreach (var propId in propIds)
                {
                    if (index == result.Length)
                    {
                        ThrowCapacityInconsistentException();
                    }

                    result[index++] = propId;
                }
            }

            if (index != result.Length)
            {
                ThrowCapacityInconsistentException();
            }

            void ThrowCapacityInconsistentException()
            {
                throw new ArgumentException(
                    $"The capacity of result span is inconsistent with the actual quantity of Property.Id.",
                    nameof(result)
                );
            }
        }

        public void GetPropertyIdsOfFamily(int propertyFamilyId, List<int> result)
        {
            result.Clear();

            if (!_familyRelationTable.TryGetValue(propertyFamilyId, out List<int> specIds))
            {
                return;
            }

            foreach (var specId in specIds)
            {
                List<int> propIds = _specRelationTable[specId];
                result.AddRange(propIds);
            }
        }

        public int[] GetPropertyIdsOfFamily(int propertyFamilyId)
        {
            if (!_familyRelationTable.TryGetValue(propertyFamilyId, out List<int> specIds))
            {
                return new int[0];
            }

            var propIdCount = GetPropertyIdCountOfFamily(propertyFamilyId);
            var result = new int[propIdCount];
            var index = 0;
            foreach (var specId in specIds)
            {
                List<int> propIds = _specRelationTable[specId];
                foreach (var propId in propIds)
                {
                    result[index++] = propId;
                }
            }

            return result;
        }

        #endregion


        #region Container

        public void RegisterPropertySpec(PropertySpec propertySpec)
        {
            if (!_specTable.TryAdd(propertySpec.Id, propertySpec))
            {
                return;
            }

            if (!_familyRelationTable.TryGetValue(propertySpec.FamilyId, out List<int> specIds))
            {
                specIds = new List<int>();
                _familyRelationTable.Add(propertySpec.FamilyId, specIds);
            }

            if (!_specRelationTable.ContainsKey(propertySpec.Id))
            {
                _specRelationTable.Add(propertySpec.Id, new List<int>());
            }

            if (!specIds.Contains(propertySpec.Id))
            {
                specIds.Add(propertySpec.Id);
            }
        }

        public bool UnregisterPropertySpec(int propertySpecId)
        {
            if (!_specTable.Remove(propertySpecId, out var propertySpec))
            {
                return false;
            }

            List<int> specIds = _familyRelationTable[propertySpec.FamilyId];
            specIds.Remove(propertySpec.Id);
            if (specIds.Count == 0)
            {
                _familyRelationTable.Remove(propertySpec.FamilyId);
            }

            _specRelationTable.Remove(propertySpec.Id);

            return true;
        }


        public void RegisterPropertyRelation(int propertySpecId, int propertyId)
        {
            if (!_specRelationTable.TryGetValue(propertySpecId, out List<int> propIds))
            {
                throw new ArgumentOutOfRangeException(nameof(propertySpecId),
                    $"Property spec unregistered, PropertySpec.Id={propertySpecId}.");
            }

            if (!propIds.Contains(propertyId))
            {
                propIds.Add(propertyId);
            }
        }

        public bool UnregisterPropertyRelation(int propertySpecId, int propertyId)
        {
            if (!_specRelationTable.TryGetValue(propertySpecId, out List<int> propIds))
            {
                return false;
            }

            return propIds.Remove(propertyId);
        }

        #endregion
    }
}