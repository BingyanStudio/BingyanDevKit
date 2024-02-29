using System;
using System.Collections.Generic;

public static class IEnumerableExtensions
{
    /// <summary>
    /// 类似于 <see cref="List.ForEach(Action{T})", 但可以用于所有集合/>
    /// </summary>
    /// <param name="collection">集合</param>
    /// <param name="callback">回调</param>
    /// <typeparam name="T">元素的类型</typeparam>
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> callback)
    {
        foreach (var item in collection) callback.Invoke(item);
    }

    /// <summary>
    /// 遍历所有元素，并传入各个元素与其对应的下标
    /// </summary>
    /// <param name="list">列表</param>
    /// <param name="callback">回调</param>
    /// <typeparam name="T">元素类型</typeparam>
    public static void ForEachIndex<T>(this IList<T> list, Action<int, T> callback)
    {
        for (int i = 0; i < list.Count; i++)
            callback?.Invoke(i, list[i]);
    }
}