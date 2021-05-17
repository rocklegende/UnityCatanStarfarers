using System;
using UnityEngine;
using UnityEngine.UI;

public static class GameConstants
{
    public static Font GetFont()
    {
        return (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
    }

    public static bool ShowTextAboveSpacePointButtons = true;

    public static bool isDevelopment = false;

}
