using System;
using System.Collections.Generic;

namespace GBG.GameAbilitySystem.Skill
{
    // todo: 技能冷却时间属性可能被修正，其他属性也是！

    /// <summary>
    /// 技能规则。
    /// </summary>
    [Serializable]
    public class SkillSpec
    {
        /// <summary>
        /// 【主键】技能规则Id。0为无效值。
        /// </summary>
        public int Id;

        /// <summary>
        /// 技能族Id。0为特殊值，视为此技能没有其他等级或状态。
        /// 除族Id为0以外的同族技能视为同一个技能的不同等级或状态。
        /// </summary>
        public int FamilyId;

        /// <summary>
        /// 技能等级。
        /// </summary>
        public ushort Level;

        /// <summary>
        /// 冲突掩码。掩码按位与计算结果不为0的技能相互冲突，不能同时装配。
        /// </summary>
        public ulong ConflictMask;

        /// <summary>
        /// 激活方式。
        /// </summary>
        public byte ActivationMode; // enum SkillActivationMode

        /// <summary>
        /// 自动激活周期（毫秒）。
        /// 仅在激活方式为定期自动激活时生效。
        /// 自动激活计时从技能冷却完成并且技能激活状态结束时开始。
        /// </summary>
        public uint ActivationPeriod;

        /// <summary>
        /// 技能激活时，产生技能效果的几率。
        /// 0.0代表不会激活，1.0代表必定激活。
        /// 技能激活后，即使生效几率判定失败，也会进入冷却流程和更新自动触发状态。
        /// </summary>
        public double ChanceToTakeEffect; // [0.0, 1.0]

        /// <summary>
        /// 技能激活后，激活状态的持续时间。
        /// 技能实现类中，可以根据需求调整实际的持续时间。
        /// </summary>
        public uint DefaultActiveDuration;

        /// <summary>
        /// 冷却时间（毫秒）。
        /// 冷却计时从技能激活状态结束时开始。
        /// </summary>
        public uint Cooldown;

        /// <summary>
        /// 技能标签。此标签不会标记技能持有者，只用于标记技能本身。
        /// </summary>
        public List<string> SkillTags;

        /// <summary>
        /// 技能开销键值对参数。
        /// </summary>
        public List<CommonCustomParam> CostParams;

        /// <summary>
        /// 自定义逻辑键值对参数。
        /// </summary>
        public List<CommonCustomParam> CustomLogicParams; // InstantEffects，BuffEffects
    }
}