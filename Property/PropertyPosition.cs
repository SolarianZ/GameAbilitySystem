namespace GBG.GameAbilitySystem.Property
{
    /// <summary>
    /// 属性作用位置。
    /// 计算公式：Result = (Base + ΣInnerAddCoef) * (1 + ΣMulCoef) + ΣOuterAddCoef。
    /// </summary>
    public enum PropertyPosition
    {
        InnerAddCoef,

        MulCoef,

        OuterAddCoef,
    }
}