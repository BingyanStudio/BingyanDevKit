using UnityEngine;

namespace Bingyan
{
    /// <summary>
    /// 音频工具
    /// </summary>
    public static class AudioUtils
    {
        /// <summary>
        /// 将归一化的音量值转化为实际的分贝
        /// </summary>
        /// <param name="value">音量</param>
        /// <returns>分贝</returns>
        public static float ValueToDb(float value)
            => Mathf.Log10(Mathf.Max(1e-4f, value)) * 20;

        /// <summary>
        /// 将分贝转化为归一化的音量值
        /// </summary>
        /// <param name="db">分贝</param>
        /// <returns>音量值</returns>
        public static float DbToValue(float db)
            => Mathf.Pow(10, db / 20);
    }
}