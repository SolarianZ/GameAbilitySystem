namespace GBG.GameAbilitySystem.Buff
{
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
        /// 是否已失效。
        /// </summary>
        /// <returns></returns>
        bool IsExpired();

        void Tick(uint deltaTime);

        void OnAttach(IBuffOwner buffOwner, object persistentContext);

        void OnDetach();

        bool Stack();

        bool Unstack();
    }

    // todo: IBuffEffects ?
}