using System;
using System.Collections.Generic;

namespace GBG.GameAbilitySystem.Property
{
    public static class PropertyHelper
    {
        public static PropertyModifier GetPropertyModifier(this Property property,
            IPropertySpecProvider propertySpecProvider)
        {
            if (!propertySpecProvider.TryGetPropertySpec(property.SpecId,
                    out var propertySpec))
            {
                throw new ArgumentOutOfRangeException(nameof(property),
                    $"Property spec not found, property.Id = {property.Id}, property.SpecId = {property.SpecId}.");
            }

            switch (propertySpec.Position)
            {
                case PropertyPosition.InnerAddCoef:
                    return new PropertyModifier { InnerAddCoef = property.Value };

                case PropertyPosition.MulCoef:
                    return new PropertyModifier { MulCoef = property.Value };

                case PropertyPosition.OuterAddCoef:
                    return new PropertyModifier { OuterAddCoef = property.Value };

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

        public static double ClampPropertyById(int propertyId, double propertyValue,
            IPropertyProvider propertyProvider, IPropertySpecProvider propertySpecProvider)
        {
            if (!propertyProvider.TryGetProperty(propertyId, out var property))
            {
                throw new ArgumentOutOfRangeException(nameof(propertyId),
                    $"Property not found, property.Id = {property.Id}.");
            }

            return ClampPropertyBySpecId(property.SpecId, propertyValue, propertySpecProvider);
        }

        public static double ClampPropertyBySpecId(int propertySpecId, double propertyValue,
            IPropertySpecProvider propertySpecProvider)
        {
            if (!propertySpecProvider.TryGetPropertySpec(propertySpecId, out var propertySpec))
            {
                throw new ArgumentOutOfRangeException(nameof(propertySpecId),
                    $"Property spec not found, property.SpecId = {propertySpecId}.");
            }

            return PropertySpec.Clamp(propertyValue, propertySpec);
        }
    }
}