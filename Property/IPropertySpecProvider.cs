using System;
using System.Collections.Generic;

namespace GBG.GameAbilitySystem.Property
{
    public interface IPropertySpecProvider
    {
        bool ContainsPropertySpec(int propertySpecId);

        PropertySpec GetPropertySpec(int propertySpecId);

        bool TryGetPropertySpec(int propertySpecId, out PropertySpec propertySpec);


        bool RegisterCustomProperty(int propertyId, int propertySpecId);

        bool UnregisterCustomProperty(int propertyId);


        int GetPropertyIdCountOfFamily(int propertyFamilyId);

        void GetPropertyIdsOfFamily(int propertyFamilyId, Span<int> result);

        void GetPropertyIdsOfFamily(int propertyFamilyId, List<int> result);

        int[] GetPropertyIdsOfFamily(int propertyFamilyId);
    }
}