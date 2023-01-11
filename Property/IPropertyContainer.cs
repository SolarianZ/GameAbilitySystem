namespace GBG.GameAbilitySystem.Property
{
    public interface IPropertyContainer : IPropertyProvider
    {
        bool IsPropertiesDirty { get; protected set; }

        IPropertyContainer ParentPropertyContainer { get; set; }


        void SetPropertiesDirty()
        {
            IsPropertiesDirty = true;
            ParentPropertyContainer?.SetPropertiesDirty();
        }

        bool AddPropertyProvider(IPropertyProvider propertyProvider);

        bool RemovePropertyProvider(IPropertyProvider propertyProvider);


        /// <summary>
        /// 添加自定义属性。
        /// </summary>
        /// <param name="property">属性。</param>
        /// <param name="source">属性来源。</param>
        /// <returns>此方法执行完成后，给定属性来源所提供的给定属性的数量。</returns>
        int AddCustomProperty(Property property, object source);

        /// <summary>
        /// 移除来自给定来源的自定义属性。
        /// </summary>
        /// <param name="propertyId">属性。</param>
        /// <param name="source">属性来源。</param>
        /// <param name="count">移除数量。</param>
        /// <returns>此方法执行完成后，实际移除的给定属性的数量。</returns>
        int RemoveCustomProperty(int propertyId, object source, int count = 1);

        /// <summary>
        /// 移除自定义属性，无论其来源为何。
        /// </summary>
        /// <param name="propertyId">属性。</param>
        /// <returns>此方法执行完成后，实际移除的给定属性的数量。</returns>
        int RemoveCustomPropertyIgnoreSource(int propertyId);

        /// <summary>
        /// 移除所有自定义属性。
        /// </summary>
        void RemoveAllCustomProperties();
    }
}
