using System;

namespace GBG.GameAbilitySystem.Property
{
    public interface IPropertyProvider
    {
        event Action OnPropertiesDirty;


        bool HasAnyProperty();

        bool ContainsProperty(int propertyId, object instantContext,
            bool ignoreInactiveProperties);

        double GetPropertyValue(int propertyId, object instantContext);

        bool TryGetPropertyValue(int propertyId, object instantContext,
            out double propertyValue, out int propertySpecId);
    }
}
