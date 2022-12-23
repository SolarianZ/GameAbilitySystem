﻿using System;

namespace GBG.GameAbilitySystem.Buff
{
    /// <summary>
    /// Buff显示层数据。
    /// </summary>
    [Serializable]
    public class BuffViewData
    {
        // Ln: Localization
        // Fmt: Format

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