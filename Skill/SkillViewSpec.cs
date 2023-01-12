using System;

namespace GBG.GameAbilitySystem.Skill
{
    /// <summary>
    /// 技能显示规则。
    /// </summary>
    [Serializable]
    public class SkillViewSpec
    {
        // Ln: Localization
        // Fmt: Format

        /// <summary>
        /// 【主键|外键】技能显示规则Id。应与对应技能的<see cref="SkillSpec.Id"/>保持相同。
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