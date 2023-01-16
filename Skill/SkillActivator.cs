using System.Collections.Generic;

namespace GBG.GameAbilitySystem.Skill
{
    public enum SkillActivationMode : byte
    {
        /// <summary>
        /// 手动激活。
        /// </summary>
        Manual = 0,

        /// <summary>
        /// 装配后立即激活。
        /// </summary>
        Immediate = 1,

        /// <summary>
        /// 定期自动激活，在装备技能时立即激活一次。
        /// </summary>
        Periodic = 2,

        /// <summary>
        /// 定期自动激活，在装备技能后等待一次定时激活周期再激活。
        /// </summary>
        PeriodicDelay = 3,

        ///// <summary>
        ///// 监听事件激活。
        ///// </summary>
        //Event = 4,


        // 自定义激活方式
        Custom0 = 246,
        Custom1 = 247,
        Custom2 = 248,
        Custom3 = 249,
        Custom4 = 250,
        Custom5 = 251,
        Custom6 = 252,
        Custom7 = 253,
        Custom8 = 254,
        Custom9 = 255,
    }

    public class SkillActivator : ISkillActivator
    {
        protected ISkillOwner SkillOwner { get; private set; }

        protected object? PersistentContext { get; private set; }

        private readonly List<ISkill> _pendingSkills = new List<ISkill>();


        public void Initialize(ISkillOwner skillOwner, object? persistentContext)
        {
            SkillOwner = skillOwner;
            PersistentContext = persistentContext;
        }

        public void Tick(uint deltaTime)
        {
            for (int i = 0; i < _pendingSkills.Count; i++)
            {
                ISkill skill = _pendingSkills[i];
                var activationMode = (SkillActivationMode)skill.ActivationMode;
                switch (activationMode)
                {
                    // 立即激活
                    case SkillActivationMode.Immediate:
                    {
                        if (skill.TryActivateSkill())
                        {
                            _pendingSkills.RemoveAt(i);
                            i--;
                        }

                        break;
                    }

                    // 定期激活
                    case SkillActivationMode.Periodic:
                    {
                        if (skill.SkillState == SkillState.Idle)
                        {
                            var skillIdleDuration = skill.GetIdleDuration();
                            var skillActivationPeriod = skill.GetActivationPeriod();
                            if (skill.ActivatedTimes == 0 || skillIdleDuration >= skillActivationPeriod)
                            {
                                skill.TryActivateSkill();
                            }
                        }

                        break;
                    }

                    // 定期激活，延迟首次激活
                    case SkillActivationMode.PeriodicDelay:
                    {
                        if (skill.SkillState == SkillState.Idle)
                        {
                            var skillIdleDuration = skill.GetIdleDuration();
                            var skillActivationPeriod = skill.GetActivationPeriod();
                            if (skillIdleDuration >= skillActivationPeriod)
                            {
                                skill.TryActivateSkill();
                            }
                        }

                        break;
                    }

                    default:
                    {
                        OnTickCustomActivationMode(skill, deltaTime);

                        break;
                    }
                }
            }
        }

        public void OnSkillEquipped(ISkill skill)
        {
            var activationMode = (SkillActivationMode)skill.ActivationMode;
            var pending = ShouldAddSkillToPendingList(activationMode);
            if (pending)
            {
                _pendingSkills.Add(skill);
            }
        }

        public void OnSkillUnequipped(ISkill skill)
        {
            var activationMode = (SkillActivationMode)skill.ActivationMode;
            var pending = ShouldAddSkillToPendingList(activationMode);
            if (pending)
            {
                _pendingSkills.Remove(skill);
            }
        }


        protected virtual void OnTickCustomActivationMode(ISkill skill, uint deltaTime)
        {
            throw new System.NotImplementedException("Please override this method and do not call base implemention.");
        }

        protected virtual bool ShouldAddSkillToPendingList(SkillActivationMode activationMode)
        {
            switch (activationMode)
            {
                case SkillActivationMode.Manual:
                    return false;

                case SkillActivationMode.Immediate:
                case SkillActivationMode.Periodic:
                case SkillActivationMode.PeriodicDelay:
                    return true;

                default:
                    throw new System.NotImplementedException($"Unknown skill activation mode: {activationMode}");
            }
        }
    }
}
