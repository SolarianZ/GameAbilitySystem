namespace GBG.GameAbilitySystem.Property
{
    public interface IPropertyStaticDataProvider
    {
        #region Property Data

        bool ContainsPropertyData(int propertyId);

        PropertyData GetPropertyData(int propertyId);

        bool TryGetPropertyData(int propertyId, out PropertyData propertyData);

        #endregion


        #region Property Define Data

        bool ContainsPropertyDefineData(int propertyTypeId);

        PropertyDefineData GetPropertyDefineData(int propertyTypeId);

        bool TryGetPropertyDefineData(int propertyId, out PropertyDefineData propertyDefineData);

        #endregion
    }
}