using System;

namespace GBG.GameAbilitySystem.Property
{
    // todo: Property Exclusion?

    /// <summary>
    /// 属性描述。
    /// </summary>
    [Serializable]
    public class PropertySpec
    {
        /// <summary>
        /// 【主键】属性描述Id。
        /// </summary>
        public int Id;

        /// <summary>
        /// 属性族Id。同族属性可以在同一属性计算公式中生效。
        /// 多个不同属性可能具有相同属性族Id，这些属性在属性计算公式的不同位置生效。
        /// </summary>
        public int FamilyId;

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


        public static double Clamp(double propertyValue, PropertySpec propertySpec)
        {
            return Math.Clamp(propertyValue, propertySpec.MinValue, propertySpec.MaxValue);
        }
    }
}