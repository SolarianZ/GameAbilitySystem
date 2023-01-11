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
        /// 【主键】属性Id。
        /// </summary>
        public int Id;

        /// <summary>
        /// 【外键】属性描述Id。
        /// </summary>
        public int SpecId;

        /// <summary>
        /// 属性值。
        /// </summary>
        public double Value;


        public override string ToString()
        {
            return $"Prop{{Id={Id}, SpecId={SpecId}, Value={Value:F5}}}";
        }
    }
}