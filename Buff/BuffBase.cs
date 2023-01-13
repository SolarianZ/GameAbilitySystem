using System;

namespace GBG.GameAbilitySystem.Buff
{
    // 用于在线游戏时，需要保证Buff中的所有状态可以复制，当前版本不满足此需求。
    public abstract class BuffBase : IBuff
    {
        public int InstanceId { get; }

        public int Id => BuffSpec.Id;

        public int FamilyId => BuffSpec.FamilyId;

        public int StackingNumber { get; private set; }

        protected BuffSpec BuffSpec { get; }


        protected BuffBase(BuffSpec buffSpec, int instanceId)
        {
            BuffSpec = buffSpec;
            InstanceId = instanceId;
        }


        #region Lifecycle

        public bool IsExpired() => throw new NotImplementedException();

        void IBuff.Tick(uint deltaTime) => throw new NotImplementedException();

        /// <summary>
        /// Buff是否需要执行逻辑帧更新。
        /// </summary>
        protected virtual bool NeedTick() { return BuffSpec.Duration > 0; }


        void IBuff.OnAttach(IBuffOwner buffOwner, object persistentContext) => throw new NotImplementedException();

        protected virtual void OnAttach() { }

        void IBuff.OnDetach() => throw new NotImplementedException();

        protected virtual void OnDetach() { }


        bool IBuff.Stack()
        {
            if (!IsExpired())
            {
                return false;
            }

            if (!TryStack())
            {
                return false;
            }

            StackingNumber++;

            return true;
        }

        protected virtual bool TryStack()
        {
            return BuffSpec.MaxStackingNumber == 0 || StackingNumber < BuffSpec.MaxStackingNumber;
        }

        bool IBuff.Unstack()
        {
            if (!IsExpired())
            {
                return false;
            }

            if (!TryUnstack())
            {
                return false;
            }

            StackingNumber--;

            return true;
        }

        protected virtual bool TryUnstack()
        {
            return StackingNumber > 1;
        }

        #endregion


        #region Effects

        protected bool ConsumeOneAvailableTime() => throw new NotImplementedException();


        protected abstract bool IsPropertyActive(int propertyId, object instantContext);

        protected abstract bool IsTagActive(string tag, object instantContext);

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

        #endregion
    }
}