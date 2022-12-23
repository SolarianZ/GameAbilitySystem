namespace GBG.GameAbilitySystem.Property
{
    /// <summary>
    /// 属性修改器。
    /// 计算公式：Result = (Base + ΣInnerAddCoef) * (1 + ΣMulCoef) + ΣOuterAddCoef。
    /// </summary>
    public struct PropertyModifier
    {
        public double InnerAddCoef;

        public double MulCoef;

        public double OuterAddCoef;


        public static PropertyModifier operator +(PropertyModifier l, PropertyModifier r)
        {
            return new PropertyModifier
            {
                InnerAddCoef = l.InnerAddCoef + r.InnerAddCoef,
                MulCoef = l.MulCoef + r.MulCoef,
                OuterAddCoef = l.OuterAddCoef + r.OuterAddCoef,
            };
        }

        public static PropertyModifier operator -(PropertyModifier l, PropertyModifier r)
        {
            return new PropertyModifier
            {
                InnerAddCoef = l.InnerAddCoef - r.InnerAddCoef,
                MulCoef = l.MulCoef - r.MulCoef,
                OuterAddCoef = l.OuterAddCoef - r.OuterAddCoef,
            };
        }
    }
}