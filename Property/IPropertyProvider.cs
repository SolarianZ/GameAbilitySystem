namespace GBG.GameAbilitySystem.Property
{
    public interface IPropertyProvider
    {
        bool ContainsProperty(int propertyId);

        bool ContainsActiveProperty(int propertyId, object instantContext);

        double GetPropertyValue(int propertyId, object instantContext);

        bool TryGetPropertyValue(int propertyId, object instantContext,
            out double propertyValue, out int propertySpecId);
    }
}
