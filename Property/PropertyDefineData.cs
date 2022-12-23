using System;

namespace GBG.GameAbilitySystem.Property
{
    /// <summary>
    /// 属性定义数据。
    /// </summary>
    [Serializable]
    public class PropertyDefineData
    {
        /// <summary>
        /// 属性类型Id。用以区分不同属性。
        /// </summary>
        public int TypeId;

        /// <summary>
        /// 属性作用位置。
        /// </summary>
        public PropertyPosition Position;

        /// <summary>
        /// 最小值。
        /// </summary>
        public double MinValue;

        /// <summary>
        /// 最大值。
        /// </summary>
        public double MaxValue;


        public static double Clamp(double propertyValue, PropertyDefineData propertyDefineData)
        {
            return Math.Clamp(propertyValue, propertyDefineData.MinValue, propertyDefineData.MaxValue);
        }
    }
}