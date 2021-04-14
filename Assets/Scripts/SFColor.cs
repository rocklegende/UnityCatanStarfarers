using System;
using UnityEngine;

[Serializable]
public class SFColor
{
    private int r;
    private int g;
    private int b;
    private int a = 255;

    public SFColor(int r, int g, int b, int a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public SFColor(int r, int g, int b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = 255;
    }

    public SFColor(Color unityColor)
    {
        this.r = (int)unityColor.r;
        this.g = (int)unityColor.g;
        this.b = (int)unityColor.b;
        this.a = (int)unityColor.a;
    }

    public Color ToUnityColor()
    {
        return new Color((float)r / 255.0F, (float)g / 255.0F, (float)b / 255.0F, (float)a / 255.0F);
    }

    //public static SFColor red()
    //{
    //    return new SFColor(Color.red);
    //}
    //public static Color blue = Color.blue;
    //public static Color black = Color.green;
    //public static Color yellow = Color.yellow;
    //public static Color white = Color.white;
}
