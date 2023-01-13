using System;
using System.Collections.Generic;

namespace GBG.GameAbilitySystem.Property
{
    public interface IPropertySpecProvider
    {
        bool ContainsPropertySpec(int propertySpecId);

        bool TryGetPropertySpec(int propertySpecId, out PropertySpec propertySpec);


        int GetPropertyIdCountOfFamily(int propertyFamilyId);

        void GetPropertyIdsOfFamily(int propertyFamilyId, Span<int> result);

        void GetPropertyIdsOfFamily(int propertyFamilyId, List<int> result);

        int[] GetPropertyIdsOfFamily(int propertyFamilyId);
    }
}