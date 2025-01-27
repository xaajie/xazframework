using System.Diagnostics;
using UnityEngine;

public static class Logger
{
    private static string getMessage(object[] messages)
    {
        string text = "";
        foreach (object arg in messages)
        {
            text = text + " " + arg;
        }
        if (!string.IsNullOrEmpty(text))
        {
            return text.Substring(1);
        }
        return "";
    }

    [Conditional("USE_LOG")]
    [Conditional("UNITY_EDITOR")]
    public static void Print(params object[] messages)
    {
        UnityEngine.Debug.Log(getMessage(messages));
    }

    [Conditional("USE_LOG")]
    [Conditional("UNITY_EDITOR")]
    public static void Error(params object[] messages)
    {
        UnityEngine.Debug.LogError(getMessage(messages));
    }

    [Conditional("USE_LOG")]
    [Conditional("UNITY_EDITOR")]
    public static void Exception(params object[] messages)
    {
        UnityEngine.Debug.LogException(new UnityException(getMessage(messages)));
    }

    [Conditional("UNITY_EDITOR")]
    [Conditional("USE_LOG")]
    public static void Warning(params object[] messages)
    {
        UnityEngine.Debug.LogWarning(getMessage(messages));
    }
}
