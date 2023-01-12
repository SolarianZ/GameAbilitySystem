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

        /// <summary>
        /// 监听事件激活。
        /// </summary>
        Event = 4,


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
}