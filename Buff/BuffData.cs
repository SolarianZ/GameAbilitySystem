using System;
using System.Collections.Generic;

namespace GBG.GameAbilitySystem.Buff
{
    /// <summary>
    /// Buff数据。
    /// </summary>
    [Serializable]
    public class BuffData
    {
        /// <summary>
        /// 【主键】配置Id。0为无效值。
        /// </summary>
        public int Id;

        /// <summary>
        /// 类型Id。0为无效值。同种类型Id的Buff可以产生叠加关系（0除外）。
        /// </summary>
        public int TypeId;

        /// <summary>
        /// 冲突掩码。掩码按位与计算结果不为0的Buff实例相互冲突，发生冲突时，保留<see cref="Priority"/>高者。
        /// </summary>
        public ulong ConflictMask;

        /// <summary>
        /// 优先级。Buff之间发生冲突时，保留优先级高者。
        /// </summary>
        public int Priority;

        /// <summary>
        /// 标签。可用于Buff筛选。
        /// </summary>
        public List<object> Tags;

        /// <summary>
        /// 持续时间（毫秒）。0代表无限持续时间。
        /// </summary>
        public uint Duration;

        /// <summary>
        /// 同种<see cref="TypeId"/>的Buff的叠加方式。
        /// </summary>
        public byte OverlayMode; // enum BuffOverlayMode

        /// <summary>
        /// 自定义键值对参数。
        /// </summary>
        public List<CommonCustomParam> CustomParams;
    }

    // public class BuffTypeDefineData
    // {
    //     public int TypeId;
    // }
}