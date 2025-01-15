using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Bingyan
{
    public static class CSV
    {
        private static readonly HashSet<string> falseStrs = new()
        {
            "0", "否", "无", "false"
        };

        private static readonly Dictionary<Type, Func<string, object>> defaultParsers = new()
        {
            { typeof(string), s=>s },
            { typeof(int), s=>{
                    if(!int.TryParse(s, out var i))throw new Exception($"无法将 {s} 转换为整数");
                    return i;
                }
            },
            { typeof(float), s=>{
                    if(!float.TryParse(s, out var i))throw new Exception($"无法将 {s} 转换为小数");
                    return i;
                }
            },
            { typeof(bool), s=> !falseStrs.Contains(s.ToLower().Trim()) },
        };

        public static List<T> Parse<T>(TextAsset source,
                                    char lineSplit = '\n',
                                    char fieldSplit = ',',
                                    params (Type, Func<string, object>)[] customParsers)
            => Parse<T>(source.text, lineSplit, fieldSplit, customParsers);

        public static List<T> Parse<T>(string source,
                                    char lineSplit = '\n',
                                    char fieldSplit = ',',
                                    params (Type, Func<string, object>)[] customParsers)
        {
            var parsers = new Dictionary<Type, Func<string, object>>(defaultParsers);
            foreach (var item in customParsers)
                parsers[item.Item1] = item.Item2;

            var lines = source.Split(lineSplit);
            foreach (var h in lines.Where(i => i.Length > 0).GroupBy(i => i))
                if (h.Count() > 1) Debug.LogWarning($"表头中出现多个 {h.Key}");

            var headers = new List<string>(lines[0].Trim().Split(fieldSplit));

            var colFieldMap = new Dictionary<int, FieldInfo>();
            var colParserMap = new Dictionary<int, Func<string, object>>();

            int maxIdx = -1;

            foreach (var field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                        .Where(i => i.GetCustomAttribute<CSVIgnoreAttribute>() == null))
            {
                var headerAttr = field.GetCustomAttribute<CSVAttribute>();
                var header = headerAttr == null ? field.Name : headerAttr.Header;

                int idx = headers.IndexOf(header);
                if (idx == -1)
                {
                    Debug.LogWarning($"变量 {header} 未在表格头中找到!\n如果这是期望的行为，请为其添加 [CSVIgnore]");
                    continue;
                }

                if (!parsers.TryGetValue(field.FieldType, out var parser))
                {
                    Debug.LogError($"未找到 {field.FieldType} 类型的解析器，需要提供一个！");
                    continue;
                }

                colFieldMap[idx] = field;
                colParserMap[idx] = parser;

                if (idx > maxIdx) maxIdx = idx;
            }

            var results = new List<T>();

            lines[1..].ForEachIndex(
                (idx, line) =>
                {
                    line = line.Trim();
                    if (line.Length == 0) return;

                    var ps = line.Split(fieldSplit);
                    if (ps.Length < maxIdx + 1)
                    {
                        Debug.LogWarning($"第 {idx + 1} 行缺少数据，跳过！");
                        return;
                    }

                    var obj = Activator.CreateInstance<T>();
                    foreach (var colFieldPair in colFieldMap)
                    {
                        object val = null;

                        try
                        {
                            val = colParserMap[colFieldPair.Key](ps[colFieldPair.Key]);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"第 {idx + 1} 行第 {colFieldPair.Key + 1} 列解析失败:\n{e.Message}");
                        }

                        colFieldPair.Value.SetValue(obj, val);
                    }

                    results.Add(obj);
                }
            );

            return results;
        }
    }
}