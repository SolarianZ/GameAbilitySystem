using System;

namespace GBG.GameAbilitySystem.Skill
{
    public delegate void SkillActivatedCallback(SkillHandle skillHandle, bool tookEffect);

    /// <summary>
    /// 技能。
    /// </summary>
    public interface ISkill
    {
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
        /// 技能实例Id。
        /// </summary>
        int InstanceId { get; }

        /// <summary>
        /// 技能等级。
        /// </summary>
        ushort Level { get; }

        bool IsActive { get; }

        bool CanActivate { get; }

        bool CanCancel { get; }


        event SkillActivatedCallback OnSkillActivated;

        event Action<SkillHandle> OnSkillCanceled;

        event Action<SkillHandle> OnSkillEnded;


        /// <summary>
        /// 技能是否需要执行逻辑帧更新。
        /// </summary>
        bool NeedTick();

        void OnEquip(ISkillOwner skillOwner, object persistentContext);

        void OnUnequip();

        void Tick(uint deltaTime);

        bool IsValid();

        uint GetCooldownTimeRemaining();

        uint GetActiveTimeRemaining();

        bool TryActivateSkill();

        bool TryCancelSkill();
    }

    // 技能实例可以动态替换Spec吗？

    public abstract class SkillBase //: ISkill
    {
        protected SkillSpec SkillSpec { get; }

        protected SkillBase(SkillSpec skillSpec)
        {
            SkillSpec = skillSpec;
        }

        public bool NeedTick()
        {
            switch (SkillSpec.ActivationMode)
            {
                case SkillActivationMode.Manual:
                case SkillActivationMode.Immediate:
                case SkillActivationMode.Event:
                    return GetCooldownDuration() > 0 || GetActiveDuration() > 0;

                case SkillActivationMode.Periodic:
                case SkillActivationMode.PeriodicDelay:
                    return true;

                default:
                    return CustomLogicNeedTick();
            }
        }

        protected virtual bool CustomLogicNeedTick() { return true; }


        protected virtual uint GetCooldownDuration() { return SkillSpec.Cooldown; }

        protected virtual uint GetActiveDuration() { return SkillSpec.ActiveDuration; }


        protected bool TryGetCustomLogicParam(string key, out double value)
        {
            if (SkillSpec.CustomLogicParams == null)
            {
                value = default;
                return false;
            }

            foreach (var param in SkillSpec.CustomLogicParams)
            {
                if (param.Key.Equals(key))
                {
                    value = param.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}