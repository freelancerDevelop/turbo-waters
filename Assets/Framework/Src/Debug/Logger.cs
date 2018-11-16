using UnityEngine;

public static class Logger
{
    public static void ErrorFormat(Object context, string template, params object[] args)
    {
        Error(context, string.Format(template, args));
    }

    public static void ErrorFormat(string template, params object[] args)
    {
        Error(string.Format(template, args));
    }

    public static void Error(object message)
    {
        Debug.LogError(message);
    }

    public static void Error(Object context, object message)
    {
        Debug.LogError(message, context);
    }

    public static void WarningFormat(Object context, string template, params object[] args)
    {
        Warning(context, string.Format(template, args));
    }

    public static void WarningFormat(string template, params object[] args)
    {
        Warning(string.Format(template, args));
    }

    public static void Warning(object message)
    {
        Debug.LogWarning(message);
    }

    public static void Warning(Object context, object message)
    {
        Debug.LogWarning(message, context);
    }

    [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void MessageFormat(Object context, string template, params object[] args)
    {
        Message(context, string.Format(template, args));
    }

    [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void MessageFormat(string template, params object[] args)
    {
        Message(string.Format(template, args));
    }

    [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Message(object message)
    {
        Debug.Log(message);
    }

    [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Message(Object context, object message)
    {
        Debug.Log(message, context);
    }
}
