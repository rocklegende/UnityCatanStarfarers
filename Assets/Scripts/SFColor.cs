using System;
using UnityEngine;

[Serializable]
public class SFColor
{
    public readonly float r;
    public readonly float g;
    public readonly float b;
    public readonly float a = 1;

    public SFColor(int r, int g, int b, int a)
    {
        this.r = (float)r / 255.0F;
        this.g = (float)g / 255.0F;
        this.b = (float)b / 255.0F;
        this.a = (float)a / 255.0F;
    }

    public SFColor(int r, int g, int b)
    {
        this.r = (float)r / 255.0F;
        this.g = (float)g / 255.0F;
        this.b = (float)b / 255.0F;
        this.a = 1;
    }

    public SFColor(Color unityColor)
    {
        this.r = unityColor.r;
        this.g = unityColor.g;
        this.b = unityColor.b;
        this.a = unityColor.a;
    }

    public Color ToUnityColor()
    {
        return new Color(r, g, b, a);
    }

    public string ConvertToString()
    {
        return string.Format("{0}, {1}, {2}, {3}", r, g, b, a);
    }

}
