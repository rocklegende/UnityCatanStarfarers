

using System;
using UnityEngine;

public abstract class ChipGroup {

    public abstract string GetTextureName();

    public Sprite GetSprite()
    {
        Texture2D texture = Resources.Load(GetTextureName()) as Texture2D;
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.0f, 0.0f));
        return sprite;
    }
}

public class TriangleChipGroup : ChipGroup
{
    public override string GetTextureName()
    {
        return "chip_group_triangle";
    }
}

public class SquareChipGroup : ChipGroup
{
    public override string GetTextureName()
    {
        return "chip_group_square";
    }
}

public class CircleChipGroup : ChipGroup
{
    public override string GetTextureName()
    {
        return "chip_group_circle";
    }
}