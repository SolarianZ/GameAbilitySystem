using System;
using System.Collections.Generic;

namespace GBG.GameAbilitySystem.Property
{
    /// <summary>
    /// 属性修改器。
    /// 计算公式：Result = Σ<see cref="BaseAddCoef"/> * (1 + Σ<see cref="MulCoef"/>) + Σ<see cref="OuterAddCoef"/>。
    /// </summary>
    public struct PropertyModifier
    {
        public double BaseAddCoef;

        public double MulCoef;

        public double OuterAddCoef;

        public double GetValue()
        {
            // Result = ΣBaseAddCoef * (1 + ΣMulCoef) + ΣOuterAddCoef
            var result = BaseAddCoef * (1 + MulCoef) + OuterAddCoef;
            return result;
        }

        public override string ToString()
        {
            return $"PropertyModifier=Σ{BaseAddCoef:F5}*(1+Σ{MulCoef:F5})+Σ{OuterAddCoef:F5}.";
        }


        public static double GetValue(IEnumerable<PropertyModifier> modifiers)
        {
            var modifier = new PropertyModifier();
            foreach (var mod in modifiers)
            {
                modifier += mod;
            }

            var result = modifier.GetValue();
            return result;
        }

        public static double GetValue(List<PropertyModifier>.Enumerator modifiers)
        {
            var modifier = new PropertyModifier();
            while (modifiers.MoveNext())
            {
                modifier += modifiers.Current;
            }

            var result = modifier.GetValue();
            return result;
        }

        public static PropertyModifier CreatePropertyModifier(double propertyValue,
            PropertyPosition propertyPosition)
        {
            switch (propertyPosition)
            {
                case PropertyPosition.BaseAddCoef:
                    return new PropertyModifier { BaseAddCoef = propertyValue };

                case PropertyPosition.MulCoef:
                    return new PropertyModifier { MulCoef = propertyValue };

                case PropertyPosition.OuterAddCoef:
                    return new PropertyModifier { OuterAddCoef = propertyValue };

                default:
                    throw new ArgumentOutOfRangeException(nameof(propertyPosition),
                        $"Unknown property position: {propertyPosition}.");
            }
        }


        public static PropertyModifier operator +(PropertyModifier l, PropertyModifier r)
        {
            return new PropertyModifier
            {
                BaseAddCoef = l.BaseAddCoef + r.BaseAddCoef,
                MulCoef = l.MulCoef + r.MulCoef,
                OuterAddCoef = l.OuterAddCoef + r.OuterAddCoef,
            };
        }

        public static PropertyModifier operator -(PropertyModifier l, PropertyModifier r)
        {
            return new PropertyModifier
            {
                BaseAddCoef = l.BaseAddCoef - r.BaseAddCoef,
                MulCoef = l.MulCoef - r.MulCoef,
                OuterAddCoef = l.OuterAddCoef - r.OuterAddCoef,
            };
        }
    }
}