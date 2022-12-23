using System;

namespace GBG.GameAbilitySystem.Property
{
    /// <summary>
    /// 属性数据。
    /// </summary>
    [Serializable]
    public class PropertyData
    {
        /// <summary>
        /// 【主键】数据Id。
        /// </summary>
        public int Id;

        /// <summary>
        /// 属性类型Id。用以区分不同属性。
        /// </summary>
        public int TypeId;

        /// <summary>
        /// 属性值。
        /// </summary>
        public double Value;
    }
}