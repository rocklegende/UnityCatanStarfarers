using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ColorCreator
{
    public static Color Create(int r, int g, int b, int a)
    {
        return new Color((float)r / 255.0F, (float)g / 255.0F, (float)b / 255.0F, (float)a / 255.0F);
    }
}
