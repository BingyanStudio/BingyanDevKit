using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bingyan
{
    public static class DialogUtils
    {
        /// <summary>
        /// 弹出提示框
        /// </summary>
        /// <param name="title">提示框标题</param>
        /// <param name="content">提示框内容</param>
        /// <param name="yesBtn">确认按钮的文字</param>
        /// <param name="cancelBtn">取消按钮的文字</param>
        /// <param name="isErr">是否是报错的信息，如果是，则会同时在控制台打印信息，以方便栈追踪</param>
        public static bool Show(string title, string content, string yesBtn = "确定", string cancelBtn = "取消", bool isErr = true)
        {
#if UNITY_EDITOR
            if (isErr) Debug.LogError(content);
            return EditorUtility.DisplayDialog(title, content, yesBtn, cancelBtn);
#endif
            return false;
        }
    }
}