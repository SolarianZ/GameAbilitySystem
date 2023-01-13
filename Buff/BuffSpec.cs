using System;
using System.Collections.Generic;

namespace GBG.GameAbilitySystem.Buff
{
    // todo: Buff持续时间属性可能被修正，其他属性也是！

    /// <summary>
    /// Buff规则。
    /// 注意：Buff只能用于提供数据，进而影响目标对象的行为，Buff本身不能产生除提供数据以外的行为。
    /// </summary>
    [Serializable]
    public class BuffSpec
    {
        /// <summary>
        /// 【主键】Buff规则Id。0为无效值。
        /// </summary>
        public int Id;

        /// <summary>
        /// Buff族Id。0为特殊值，视为此Buff没有同族Buff（即不会产生叠加关系）。
        /// 除族Id为0以外的同族Buff可以产生叠加关系。
        /// </summary>
        public int FamilyId;

        /// <summary>
        /// 优先级。Buff之间发生冲突时，保留优先级高者。
        /// </summary>
        public int Priority;

        /// <summary>
        /// 冲突掩码。掩码按位与计算结果不为0的Buff实例相互冲突，发生冲突时，保留<see cref="Priority"/>高者。
        /// </summary>
        public ulong ConflictMask;

        /// <summary>
        /// 持续时间（毫秒）。0代表无限持续时间。
        /// 从Buff被添加到对象开始，经过此时间后，Buff将被移除。
        /// </summary>
        public uint Duration;

        /// <summary>
        /// 可用次数。0代表无限可用次数。
        /// 默认情况下1次逻辑Tick中，Buff只能被消耗1次可用次数，可用次数耗尽后，Buff将被移除。
        /// </summary>
        public uint AvailableTimes;

        /// <summary>
        /// 同族（<see cref="FamilyId"/>）Buff的叠加方式。
        /// </summary>
        public BuffStackingMode StackingMode;

        /// <summary>
        /// 最大叠加次数。
        /// </summary>
        public ushort MaxStackingNumber;

        /// <summary>
        /// Buff标签。此标签不会标记Buff持有者，只用于标记Buff本身。
        /// </summary>
        public List<string> BuffTags;

        /// <summary>
        /// 标签效果。此标签会标记Buff持有者，而非用于标记Buff本身。
        /// </summary>
        public List<string> TagEffects;

        /// <summary>
        /// 提供的属性（<see cref="Property.Property.Id"/>）。
        /// </summary>
        public List<int> PropertyEffects;

        /// <summary>
        /// 赋予的技能（<see cref="Skill.SkillSpec.Id"/>）。
        /// </summary>
        public List<int> SkillGrantEffects;

        /// <summary>
        /// 禁用的技能（<see cref="Skill.SkillSpec.Id"/>）。
        /// 此效果并不移除技能，只是禁用。
        /// 如果要禁用技能族，请将目标技能族中的所有技能Id填入此表。
        /// </summary>
        public List<int> SkillBanEffects;

        /// <summary>
        /// 自定义逻辑键值对参数。
        /// </summary>
        public List<CommonCustomParam> CustomLogicParams; // 生效条件等
    }
}