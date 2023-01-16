using System;

namespace GBG.GameAbilitySystem.Skill
{
    public delegate void SkillActivatedCallback(SkillHandle skillHandle, bool tookEffect);
    public enum SkillState : byte
    {
        Idle = 0,

        Cooldown = 1,

        Active = 2,

        //Banned,
    }

    /// <summary>
    /// 技能。
    /// </summary>
    public interface ISkill
    {
        /// <summary>
        /// 技能实例Id。
        /// </summary>
        int InstanceId { get; }

        /// <summary>
        /// 【主键】技能规则Id。0为无效值。
        /// </summary>
        int Id { get; }

        /// <summary>
        /// 技能族Id。0为特殊值，视为此技能没有其他等级或状态。
        /// 除族Id为0以外的同族技能视为同一个技能的不同等级或状态。
        /// </summary>
        int FamilyId { get; }

        /// <summary>
        /// 技能等级。
        /// </summary>
        ushort Level { get; }

        /// <summary>
        /// 激活方式。
        /// </summary>
        byte ActivationMode { get; }

        /// <summary>
        /// 技能是否被禁用。
        /// </summary>
        bool IsBanned { get; }

        /// <summary>
        /// 已激活次数。
        /// </summary>
        int ActivatedTimes { get; }

        /// <summary>
        /// 技能状态。
        /// </summary>
        SkillState SkillState { get; }


        event SkillActivatedCallback OnSkillActivated;

        event Action<SkillHandle> OnSkillCanceled;

        event Action<SkillHandle> OnSkillEnded;

        event Action<SkillHandle> OnSkillBanned;

        event Action<SkillHandle> OnSkillUnbanned;


        void Tick(uint deltaTime);

        void OnEquip(ISkillOwner skillOwner, object persistentContext);

        void OnUnequip();

        bool TryBan();

        bool TryUnban();


        uint GetActivationPeriod();

        uint GetIdleDuration();

        uint GetCooldownTimeRemaining();

        /// <summary>
        /// 获取技能激活阶段的剩余时间。
        /// 技能的一个激活阶段时间耗尽后，可能进入另一个激活阶段，也可能结束技能。
        /// </summary>
        /// <returns></returns>
        uint GetActiveStageTimeRemaining();


        /// <summary>
        /// 计算技能激活开销。
        /// </summary>
        /// <param name="costs">技能激活开销。</param>
        /// <returns>激活技能是否有开销。</returns>
        bool CalculateSkillActivationCosts(out object costs);

        bool CanActivateSkill();

        bool TryActivateSkill();

        bool CanCancelSkill();

        bool TryCancelSkill();
    }
}