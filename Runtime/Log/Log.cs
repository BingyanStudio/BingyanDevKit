#define LOG_TRACE
#define LOG_INFO
#define LOG_WARNING
#define LOG_ERROR

using System.Runtime.CompilerServices;
using UnityEngine;

namespace Bingyan
{
        public static class Log
        {
                [MethodImpl(MethodImplOptions.AggressiveInlining), HideInCallstack]
                public static void T(string tag, object message)
                {
#if UNITY_EDITOR && LOG_TRACE
                        Debug.Log($"<b><color=#{ColorUtility.ToHtmlStringRGB(Color.HSVToRGB(((float)tag.GetHashCode() - int.MinValue) / ((float)int.MaxValue - int.MinValue), 1, 1))}>[{tag}]</color></b> <color=#888888>{message}</color>");
#endif
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining), HideInCallstack]
                public static void T(object message)
                {
#if UNITY_EDITOR && LOG_TRACE
                        Debug.Log($"<b><color=#888888>[T]</color></b> <color=#888888>{message}</color>");
#endif
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining), HideInCallstack]
                public static void I(string tag, object message)
                {
#if UNITY_EDITOR && LOG_INFO
                        Debug.Log($"<b><color=#{ColorUtility.ToHtmlStringRGB(Color.HSVToRGB(((float)tag.GetHashCode() - int.MinValue) / ((float)int.MaxValue - int.MinValue), 1, 1))}>[{tag}]</color></b> {message}");
#endif
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining), HideInCallstack]
                public static void I(object message)
                {
#if UNITY_EDITOR && LOG_INFO
                        Debug.Log($"<b><color=#09DAFF>[I]</color></b> {message}");
#endif
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining), HideInCallstack]
                public static void W(string tag, object message)
                {
#if UNITY_EDITOR && LOG_WARNING
                        Debug.LogWarning($"<b><color=#FFB509>[{tag}]</color></b> <color=#FFDC7D>{message}</color>");
#endif
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining), HideInCallstack]
                public static void W(object message)
                {
#if UNITY_EDITOR && LOG_WARNING
                        Debug.LogWarning($"<b><color=#FFB509>[W]</color></b> <color=#FFDC7D>{message}</color>");
#endif
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining), HideInCallstack]
                public static void E(string tag, object message)
                {
#if UNITY_EDITOR && LOG_ERROR
                        Debug.LogError($"<b><color=#F83939>[{tag}]</color></b> <color=#FF7D7E>{message}</color>");
#endif
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining), HideInCallstack]
                public static void E(object message)
                {
#if UNITY_EDITOR && LOG_ERROR
                        Debug.LogError($"<b><color=#F83939>[E]</color></b> <color=#FF7D7E>{message}</color>");
#endif
                }
        }
}