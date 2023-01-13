using System;

namespace GBG.GameAbilitySystem.Property
{
    public static class PropertyHelper
    {
        public static double CalculatePropertyFamilyValue(int propertyFamilyId, object instantContext,
            IPropertyProvider propertyProvider, IPropertySpecProvider propertySpecProvider)
        {
            var propertyIdCount = propertySpecProvider.GetPropertyIdCountOfFamily(propertyFamilyId);
            Span<int> propertyIds = stackalloc int[propertyIdCount];
            propertySpecProvider.GetPropertyIdsOfFamily(propertyFamilyId, propertyIds);

            var propertyModifier = new PropertyModifier();
            foreach (var propertyId in propertyIds)
            {
                if (!propertyProvider.TryGetPropertyValue(propertyId, instantContext,
                    out var propertyValue, out var propertySpecId))
                {
                    throw new ArgumentOutOfRangeException(nameof(propertyId),
                        $"Property not found, property.Id={propertyId}.");
                }

                if (!propertySpecProvider.TryGetPropertySpec(propertySpecId, out var propertySpec))
                {
                    throw new ArgumentOutOfRangeException(nameof(propertySpecId),
                        $"Property spec not found, property.Id={propertyId}, property.SpecId={propertySpecId}.");
                }

                //propertyModifier += CreatePropertyModifier(propertyValue, propertySpec.Position);
                propertyModifier.AddPropertyModifierValue(propertyValue, propertySpec.Position);
            }

            var result = propertyModifier.GetValue();
            return result;
        }

        public static void AddPropertyModifierValue(this PropertyModifier propertyModifier,
            double propertyValue, byte propertyPosition)
        {
            switch ((PropertyPosition)propertyPosition)
            {
                case PropertyPosition.BaseAddCoef:
                    propertyModifier.BaseAddCoef += propertyValue;
                    break;

                case PropertyPosition.MulCoef:
                    propertyModifier.MulCoef += propertyValue;
                    break;

                case PropertyPosition.OuterAddCoef:
                    propertyModifier.OuterAddCoef += propertyValue;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(propertyPosition),
                        $"Unknown property position: {propertyPosition}.");
            }
        }

        public static double ClampPropertyValueBySpecId(double propertyValue, int propertySpecId,
            IPropertySpecProvider propertySpecProvider)
        {
            if (!propertySpecProvider.TryGetPropertySpec(propertySpecId, out var propertySpec))
            {
                throw new ArgumentOutOfRangeException(nameof(propertySpecId),
                    $"Property spec not found, property.SpecId={propertySpecId}.");
            }

            return propertySpec.Clamp(propertyValue);
        }
    }
}