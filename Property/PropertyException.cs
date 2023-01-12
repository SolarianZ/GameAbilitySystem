using System;

namespace GBG.GameAbilityProperty
{
    public class PropertyException : Exception
    {
        public int PropertyId { get; }

        public PropertyException(int propertyId, string message) : base(message)
        {
            PropertyId = propertyId;
        }
    }
}
