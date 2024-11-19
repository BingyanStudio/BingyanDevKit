using System;

namespace Bingyan
{
    /// <summary>
    /// 直接作用于 object 的拓展方法
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 将值应用于对象，并返回应用后的副本
        /// </summary>
        /// <param name="mapper">变换方法</param>
        /// <returns>变换后的对象</returns>
        public static T Apply<T>(this T obj, Func<T, T> mapper)
        {
            return mapper.Invoke(obj);
        }
    }
}