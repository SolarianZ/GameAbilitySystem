using System;

namespace GBG.GameAbilitySystem.Skill
{
    public delegate bool ProbabilityCalculator(double chance);

    // 用于在线游戏时，需要保证技能中的所有状态可以复制，当前版本不满足此需求。
    public abstract class SkillBase : ISkill
    {
        public int InstanceId { get; }

        public int Id => SkillSpec.Id;

        public int FamilyId => SkillSpec.FamilyId;

        public ushort Level => SkillSpec.Level;

        public bool IsBanned { get; private set; }

        public SkillState SkillState { get; private set; }

        protected SkillSpec SkillSpec { get; }

        ///// <summary>
        ///// 当帧更新时间超出技能当前状态剩余时间时，是否将超出的时间用于处理技能的下一个状态。
        ///// </summary>
        //protected virtual bool EnableTickTimeShift => true;


        public event SkillActivatedCallback? OnSkillActivated;

        public event Action<SkillHandle>? OnSkillCanceled;

        public event Action<SkillHandle>? OnSkillEnded;


        protected ProbabilityCalculator ProbabilityCalculator { get; }


        protected SkillBase(SkillSpec skillSpec, int instanceId,
            ProbabilityCalculator probabilityCalculator)
        {
            if (skillSpec == null)
            {
                throw new ArgumentNullException(nameof(skillSpec),
                    "Skill spec is null.");
            }

            if (probabilityCalculator == null)
            {
                throw new ArgumentNullException(nameof(probabilityCalculator),
                    "Probability calculator is null.");
            }

            SkillSpec = skillSpec;
            InstanceId = instanceId;
            ProbabilityCalculator = probabilityCalculator;
        }


        #region Lifecycle

        protected ISkillOwner SkillOwner { get; private set; }

        protected object? PersistentContext { get; private set; }


        void ISkill.Tick(uint deltaTime)
        {
            if (!NeedTick())
            {
                return;
            }

            OnPreTick(deltaTime);

            var lastState = SkillState;
            switch (SkillState)
            {
                case SkillState.Idle:
                {
                    ProcessIdleTime(deltaTime);
                    break;
                }

                case SkillState.Cooldown:
                {
                    ProcessCooldownTime(deltaTime);

                    if (CooldownTimeRemaining == 0)
                    {
                        SkillState = SkillState.Idle;
                    }

                    break;
                }

                case SkillState.Active:
                {
                    ProcessActiveStageTime(deltaTime);

                    if (CooldownTimeRemaining == 0)
                    {
                        TryEndSkill();
                    }

                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(SkillState),
                        $"Unknown skill state: {SkillState}.");
            }

            OnPostTick(deltaTime);

            if (lastState != SkillState)
            {
                // todo: Skill state changed
            }
        }

        /// <summary>
        /// 技能是否需要执行逻辑帧更新。
        /// </summary>
        protected virtual bool NeedTick()
        {
            switch (SkillSpec.ActivationMode)
            {
                case SkillActivationMode.Manual:
                case SkillActivationMode.Immediate:
                case SkillActivationMode.Event:
                    return GetCooldownDuration() > 0 || GetActiveStageDuration() > 0;

                case SkillActivationMode.Periodic:
                case SkillActivationMode.PeriodicDelay:
                    return true;

                default:
                    return true;
            }
        }

        protected virtual void OnPreTick(uint deltaTime) { }

        protected virtual void OnPostTick(uint deltaTime) { }


        void ISkill.OnEquip(ISkillOwner skillOwner, object persistentContext)
        {
            if (SkillOwner != null)
            {
                throw new InvalidOperationException(
                    $"Skill has been already equipped, Skill.Id={Id}, Skill.InstanceId={InstanceId}.");
            }

            if (skillOwner == null)
            {
                throw new ArgumentNullException(nameof(skillOwner),
                    $"Skill owner is null, Skill.Id={Id}, Skill.InstanceId={InstanceId}.");
            }

            SkillOwner = skillOwner;
            PersistentContext = persistentContext;

            SkillState = SkillState.Idle;

            OnEquip();
        }

        protected virtual void OnEquip() { }

        void ISkill.OnUnequip()
        {
            if (SkillOwner == null)
            {
                throw new InvalidOperationException(
                    $"Cannot unequip an unequipped skill, Skill.Id={Id}, Skill.InstanceId={InstanceId}.");
            }

            OnUnequip();

            SkillOwner = null!;
            PersistentContext = null;

            SkillState = SkillState.Idle;
        }

        protected virtual void OnUnequip() { }

        void ISkill.Ban()
        {
            if (IsBanned)
            {
                return;
            }

            IsBanned = true;
            OnBanned();
        }

        protected virtual void OnBanned() { }

        void ISkill.Unban()
        {
            if (!IsBanned)
            {
                return;
            }

            IsBanned = false;
            OnUnbanned();
        }

        protected virtual void OnUnbanned() { }

        #endregion


        #region Activation

        public abstract bool CanActivateSkill(); // => SkillState == SkillState.Idle;

        public bool TryActivateSkill()
        {
            if (!CanActivateSkill())
            {
                return false;
            }

            var chanceToTakeEffect = GetChanceToTakeEffect();
            if (ProbabilityCalculator(chanceToTakeEffect))
            {
                SkillState = SkillState.Cooldown;
                CooldownTimeRemaining = GetCooldownDuration();
                return false;
            }

            if (!ActiveSkill())
            {
                SkillState = SkillState.Cooldown;
                CooldownTimeRemaining = GetCooldownDuration();
                return false;
            }

            SkillState = SkillState.Active;
            ActiveStageTimeRemaining = GetActiveStageDuration();
            return true;
        }

        /// <summary>
        /// 激活技能。
        /// 永远不要在派生类中调用此方法，若要激活技能，请使用<see cref="TryActivateSkill"/>方法。
        /// </summary>
        /// <returns>是否成功激活。</returns>
        protected abstract bool ActiveSkill();


        public abstract bool CanCancelSkill(); // => SkillState == SkillState.Active;

        public virtual bool TryCancelSkill()
        {
            if (!CanCancelSkill())
            {
                return false;
            }

            if (!CancelSkill())
            {
                return false;
            }

            SkillState = SkillState.Cooldown;
            CooldownTimeRemaining = GetCooldownDuration();
            return true;
        }

        /// <summary>
        /// 取消激活中的技能。
        /// 不同于<see cref="EndSkill"/>方法，取消意味着技能没有正常执行完整个激活周期。
        /// 永远不要在派生类中调用此方法，若要取消激活中的技能，请使用<see cref="TryCancelSkill"/>方法。
        /// </summary>
        /// <returns>是否成功取消。</returns>
        protected abstract bool CancelSkill();


        protected bool TryEndSkill()
        {
            if (SkillState != SkillState.Active)
            {
                return false;
            }

            EndSkill();
            SkillState = SkillState.Cooldown;
            CooldownTimeRemaining = GetCooldownDuration();

            return true;
        }

        /// <summary>
        /// 完结激活中的技能。
        /// 不同于<see cref="CancelSkill"/>方法，完结意味着技能正常执行完整个激活周期。
        /// 永远不要在派生类中调用此方法，若要完结激活中的技能，请使用<see cref="EndSkill"/>方法。
        /// </summary>
        protected abstract void EndSkill();


        protected virtual bool HasAnyCostOnActivateSkill() { return true; }

        protected virtual double GetChanceToTakeEffect() { return SkillSpec.ChanceToTakeEffect; }

        #endregion


        #region Timing

        protected uint CooldownTimeRemaining { get; set; }

        protected uint ActiveStageTimeRemaining { get; set; }

        private uint _idleDuration;


        public uint GetIdleDuration() { return _idleDuration; }

        private void ProcessIdleTime(uint deltaTime)
        {
            _idleDuration += deltaTime;
        }


        uint ISkill.GetCooldownTimeRemaining() { return CooldownTimeRemaining; }

        protected virtual uint GetCooldownDuration() { return SkillSpec.Cooldown; }

        protected virtual void ProcessCooldownTime(uint deltaTime)
        {
            if (CooldownTimeRemaining > deltaTime)
            {
                CooldownTimeRemaining -= deltaTime;
                return;
            }

            CooldownTimeRemaining = 0;
        }


        uint ISkill.GetActiveStageTimeRemaining() { return ActiveStageTimeRemaining; }

        protected virtual uint GetActiveStageDuration() { return SkillSpec.DefaultActiveDuration; }

        protected virtual void ProcessActiveStageTime(uint deltaTime)
        {
            if (ActiveStageTimeRemaining > deltaTime)
            {
                ActiveStageTimeRemaining -= deltaTime;
                return;
            }

            ActiveStageTimeRemaining = 0;
        }

        #endregion


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