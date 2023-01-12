using System;

namespace GBG.GameAbilitySystem.Buff
{
    /// <summary>
    /// Buff显示规则。
    /// </summary>
    [Serializable]
    public class BuffViewSpec
    {
        // Ln: Localization
        // Fmt: Format

        /// <summary>
        /// 【主键|外键】Buff显示规则Id。应与对应Buff的<see cref="BuffSpec.Id"/>保持相同。
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