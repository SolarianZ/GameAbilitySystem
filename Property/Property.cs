using System;

namespace GBG.GameAbilitySystem.Property
{
    /// <summary>
    /// 属性数据。
    /// </summary>
    [Serializable]
    public class Property
    {
        /// <summary>
        /// 【主键】属性Id。0为无效值。
        /// </summary>
        public int Id;

        /// <summary>
        /// 【外键：<see cref="PropertySpec.Id"/>】属性规则Id。
        /// </summary>
        public int SpecId;

        /// <summary>
        /// 属性值。
        /// </summary>
        public double Value;


        public override string ToString()
        {
            return $"Property(Id={Id}, SpecId={SpecId}, Value={Value:F5})";
        }
    }
}