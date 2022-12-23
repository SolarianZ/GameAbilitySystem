using System;
using System.Collections.Generic;

namespace GBG.GameAbilitySystem.Property
{
    public static class PropertyHelper
    {
        public static IPropertyStaticDataProvider? PropertyStaticDataProvider { get; set; }

        public static PropertyModifier GetPropertyModifier(this PropertyData propertyData)
        {
            if (PropertyStaticDataProvider == null)
            {
                throw new NullReferenceException("Property static data provider is null.");
            }

            if (!PropertyStaticDataProvider.TryGetPropertyDefineData(propertyData.TypeId,
                    out var propertyDefineData))
            {
                throw new ArgumentOutOfRangeException(nameof(propertyData),
                    $"Property define data of property type id '{propertyData.TypeId}' does not exist.");
            }

            switch (propertyDefineData.Position)
            {
                case PropertyPosition.InnerAddCoef:
                    return new PropertyModifier { InnerAddCoef = propertyData.Value };

                case PropertyPosition.MulCoef:
                    return new PropertyModifier { MulCoef = propertyData.Value };

                case PropertyPosition.OuterAddCoef:
                    return new PropertyModifier { OuterAddCoef = propertyData.Value };

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static double ApplyPropertyModifier(double baseValue, PropertyModifier modifier)
        {
            // Result = (Base + ΣInnerAddCoef) * (1 + ΣMulCoef) + ΣOuterAddCoef
            var result = (baseValue + modifier.InnerAddCoef) * (1 + modifier.MulCoef) + modifier.OuterAddCoef;

            return result;
        }

        public static double ApplyPropertyModifiers(double baseValue, IEnumerable<PropertyModifier> modifiers)
        {
            var innerAddCoefSum = 0.0;
            var mulCoefSum = 0.0;
            var outerAddCoefSum = 0.0;
            foreach (var mod in modifiers)
            {
                innerAddCoefSum += mod.InnerAddCoef;
                mulCoefSum += mod.MulCoef;
                outerAddCoefSum += mod.OuterAddCoef;
            }

            // Result = (Base + ΣInnerAddCoef) * (1 + ΣMulCoef) + ΣOuterAddCoef
            var result = (baseValue + innerAddCoefSum) * (1 + mulCoefSum) + outerAddCoefSum;

            return result;
        }

        public static double ClampPropertyById(int propertyId, double propertyValue)
        {
            if (PropertyStaticDataProvider == null)
            {
                throw new NullReferenceException("Property static data provider is null.");
            }

            if (!PropertyStaticDataProvider.TryGetPropertyData(propertyId, out var propertyData))
            {
                throw new ArgumentOutOfRangeException(nameof(propertyData),
                    $"Property data of property id '{propertyId}' does not exist.");
            }

            return ClampPropertyByTypeId(propertyData.TypeId, propertyValue);
        }

        public static double ClampPropertyByTypeId(int propertyTypeId, double propertyValue)
        {
            if (PropertyStaticDataProvider == null)
            {
                throw new NullReferenceException("Property static data provider is null.");
            }

            if (!PropertyStaticDataProvider.TryGetPropertyDefineData(propertyTypeId, out var propertyDefineData))
            {
                throw new ArgumentOutOfRangeException(nameof(propertyTypeId),
                    $"Property define data of property type id '{propertyTypeId}' does not exist.");
            }

            return PropertyDefineData.Clamp(propertyValue, propertyDefineData);
        }
    }
}