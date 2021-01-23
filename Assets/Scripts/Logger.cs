using System;
using UnityEngine;

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
}
