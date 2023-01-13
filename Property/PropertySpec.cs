using System;

namespace GBG.GameAbilitySystem.Property
{
    /// <summary>
    /// 属性规则。
    /// 同一个属性规则下，可以有很多属性实例（<see cref="Property"/>）。
    /// </summary>
    [Serializable]
    public class PropertySpec
    {
        /// <summary>
        /// 【主键】属性规则Id。
        /// </summary>
        public int Id;

        /// <summary>
        /// 属性族Id。0为无效值。
        /// 同族属性可以在同一属性计算公式中生效。
        /// 多个不同属性（<see cref="Property"/>）可能具有相同属性族Id，这些属性在属性计算公式的不同位置生效。
        /// </summary>
        public int FamilyId;

        /// <summary>
        /// 属性作用位置。
        /// </summary>
        public byte Position; // 不使用PropertyPosition，以便扩展

        /// <summary>
        /// 最小值。
        /// </summary>
        public double MinValue;

        /// <summary>
        /// 最大值。
        /// </summary>
        public double MaxValue;


        public double Clamp(double propertyValue)
        {
            return Math.Clamp(propertyValue, MinValue, MaxValue);
        }

        public override string ToString()
        {
            return $"PropertySpec(Id={Id}, FamilyId={FamilyId}, Position={Position}, Range=[{MinValue:F5},{MaxValue:F5}])";
        }
    }
}