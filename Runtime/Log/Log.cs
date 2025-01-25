using System.Runtime.CompilerServices;
using UnityEngine;

namespace Bingyan
{
        public static class Log
        {
                [MethodImpl(MethodImplOptions.AggressiveInlining), HideInCallstack]
                public static void I(string tag, object message)
                {
#if UNITY_EDITOR
                        Debug.Log($"<b><color=#{ColorUtility.ToHtmlStringRGB(Color.HSVToRGB(((float)tag.GetHashCode() - int.MinValue) / ((float)int.MaxValue - int.MinValue), 1, 1))}>[{tag}]</color></b> {message}");
#endif
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining), HideInCallstack]
                public static void I(object message)
                {
#if UNITY_EDITOR
                        Debug.Log($"<b><color=#09DAFF>[I]</color></b> {message}");
#endif
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining), HideInCallstack]
                public static void W(string tag, object message)
                {
#if UNITY_EDITOR
                        Debug.LogWarning($"<b><color=#FFB509>[{tag}]</color></b> <color=#FFDC7D>{message}</color>");
#endif
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining), HideInCallstack]
                public static void W(object message)
                {
#if UNITY_EDITOR
                        Debug.LogWarning($"<b><color=#FFB509>[W]</color></b> <color=#FFDC7D>{message}</color>");
#endif
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining), HideInCallstack]
                public static void E(string tag, object message)
                {
#if UNITY_EDITOR
                        Debug.LogError($"<b><color=#F83939>[{tag}]</color></b> <color=#FF7D7E>{message}</color>");
#endif
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining), HideInCallstack]
                public static void E(object message)
                {
#if UNITY_EDITOR
                        Debug.LogError($"<b><color=#F83939>[E]</color></b> <color=#FF7D7E>{message}</color>");
#endif
                }
        }
}