using GBG.GameAbilitySystem.Skill;

namespace GBG.GameAbilitySystem.Buff
{
    // 用于在线游戏时，需要保证Buff中的所有状态可以复制，当前版本不满足此需求。

    /// <summary>
    /// Buff。
    /// 注意：Buff只能用于提供数据，进而影响目标对象的行为，Buff本身不能产生除提供数据以外的行为。
    /// </summary>
    public interface IBuff
    {
        /// <summary>
        /// Buff规则Id。0为无效值。
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Buff族Id。0为特殊值，视为此Buff没有同族Buff（即不会产生叠加关系）。
        /// 除族Id为0以外的同族Buff可以产生叠加关系。
        /// </summary>
        int FamilyId { get; }

        /// <summary>
        /// Buff实例Id。
        /// </summary>
        int InstanceId { get; }

        /// <summary>
        /// 叠加层数。
        /// </summary>
        int StackingNumber { get; }


        /// <summary>
        /// Buff是否需要执行逻辑帧更新。
        /// </summary>
        bool NeedTick();

        void OnAttach(IBuffOwner buffOwner, object persistentContext);

        void OnDetach();

        void Tick(uint deltaTime);

        bool IsValid();
    }

    // Buff实例可以动态替换Spec吗？

    public abstract class BuffBase //: IBuff
    {
        protected BuffSpec BuffSpec { get; }


        protected BuffBase(BuffSpec buffSpec)
        {
            BuffSpec = buffSpec;
        }

        public virtual bool NeedTick() { return BuffSpec.Duration > 0; }

        protected bool ConsumeOneAvailableTime() => throw new System.NotImplementedException();

        protected bool IsPropertyActive(int propertyId, object instantContext) => throw new System.NotImplementedException();

        protected bool IsTagActive(string tag, object instantContext) => throw new System.NotImplementedException();

        protected bool TryGetCustomLogicParam(string key, out double value)
        {
            if (BuffSpec.CustomLogicParams == null)
            {
                value = default;
                return false;
            }

            foreach (var param in BuffSpec.CustomLogicParams)
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