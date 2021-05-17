using System;
using UnityEngine;
using UnityEngine.TestTools;

public static class Logger
{
    static bool enabled = true;

    public static void log(object message)
    {
        if (enabled)
        {
            Debug.Log(message);
        }
        
    }

    public static void log(object message, UnityEngine.Object context)
    {
        if (enabled)
        {
            Debug.Log(message, context);
        }
        
    }

    public static void LogError(object message)
    {
        Debug.Log("ERROR: " + message);
    }
}
