namespace GBG.GameAbilitySystem.Skill
{
    public enum SkillTriggerMode : byte
    {
        /// <summary>
        /// 手动触发。
        /// </summary>
        Manual = 0,

        /// <summary>
        /// 装配后立即触发。
        /// </summary>
        Immediate = 1,

        /// <summary>
        /// 定期自动触发。
        /// </summary>
        Periodic = 2,

        /// <summary>
        /// 监听事件触发。
        /// </summary>
        Event = 3,

        /// <summary>
        /// 自定义。
        /// </summary>
        Custom = 255,
    }
}