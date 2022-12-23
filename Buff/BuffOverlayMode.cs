namespace GBG.GameAbilitySystem.Buff
{
    /// <summary>
    /// Buff叠加方式。
    /// </summary>
    public enum BuffOverlayMode : byte
    {
        /// <summary>
        /// 禁止叠加，忽略后来者。
        /// </summary>
        Ignore = 0,

        /// <summary>
        /// 禁止叠加，刷新已有实例。
        /// </summary>
        Refresh = 1,

        /// <summary>
        /// 禁止叠加，使用新实例替换已有实例。
        /// </summary>
        Replace = 2,

        /// <summary>
        /// 升阶（通常伴随Buff数值非线性增长或行为改变）。
        /// </summary>
        Upgrade = 3,

        /// <summary>
        /// 数值线性堆叠，各实例间无干扰。
        /// </summary>
        LinearStackIndependently = 4,

        /// <summary>
        /// 数值线性堆叠，各实例间共享最早的起始时间。
        /// </summary>
        LinearStackShareEarliestStartTime = 5,

        /// <summary>
        /// 自定义。
        /// </summary>
        Custom = 255,
    }
}