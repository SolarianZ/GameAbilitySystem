namespace GBG.GameAbilitySystem.Property
{
    public interface IPropertyProvider
    {
        bool ContainsProperty(int propertyId);

        Property GetProperty(int propertyId);

        bool TryGetProperty(int propertyId, out Property property);
    }
}
