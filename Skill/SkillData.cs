using System;
using System.Collections.Generic;

namespace GBG.GameAbilitySystem.Skill
{
    /// <summary>
    /// 技能数据。
    /// </summary>
    [Serializable]
    public class SkillData
    {
        /// <summary>
        /// 【主键】配置Id。0为无效值。
        /// </summary>
        public int Id;

        /// <summary>
        /// 类型Id。0为无效值。同种类型Id的技能视为同一个技能的不同等级或状态（0除外）。
        /// </summary>
        public int TypeId;

        /// <summary>
        /// 冲突掩码。掩码按位与计算结果不为0的技能相互冲突，不能同时装配。
        /// </summary>
        public ulong ConflictMask;

        /// <summary>
        /// 冷却时间（毫秒）。
        /// </summary>
        public uint Cooldown;

        /// <summary>
        /// 触发方式。
        /// </summary>
        public byte TriggerMode; // BuffTriggerMode

        /// <summary>
        /// 触发方式参数。
        /// </summary>
        public List<uint> TriggerModeParams;

        /// <summary>
        /// 自定义键值对参数。
        /// </summary>
        public List<CommonCustomParam> CustomParams; // InstantEffects，BuffEffects
    }
}