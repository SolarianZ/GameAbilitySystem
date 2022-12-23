using System;

namespace GBG.GameAbilitySystem.Skill
{
    /// <summary>
    /// 技能显示层数据。
    /// </summary>
    [Serializable]
    public class SkillViewData
    {
        // Ln: Localization
        // Fmt: Format

        /// <summary>
        /// 【主键】技能Id。
        /// </summary>
        public int Id;

        /// <summary>
        /// 本地化名称键。
        /// </summary>
        public string LnName;

        /// <summary>
        /// 本地化格式化简介键。
        /// </summary>
        public string LnFmtIntro;
    }
}