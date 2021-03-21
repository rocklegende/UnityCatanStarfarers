using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChipGroup {

    public abstract string GetTextureName();
    public abstract string GetID();

    protected List<DiceChip> diceChips;

    public ChipGroup(List<DiceChip> diceChips)
    {
        this.diceChips = diceChips;
    }

    public bool IsEqualTo(ChipGroup group)
    {
        return GetID() == group.GetID();
    }

    public List<DiceChip> GetChips()
    {
        return diceChips;
    }

    public DiceChip RetrieveChip()
    {
        if (diceChips.Count <= 0)
        {
            throw new ArgumentException("No diceChips left in this group");
        }
        return diceChips.ShuffleAndPop();
    }

    public Sprite GetSprite()
    {
        Texture2D texture = Resources.Load(GetTextureName()) as Texture2D;
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.0f, 0.0f));
        return sprite;
    }
}

public class TriangleChipGroup : ChipGroup
{
    public TriangleChipGroup(List<DiceChip> diceChips) : base(diceChips)
    {
    }

    public override string GetID()
    {
        return "chip_group_triangle";
    }

    public override string GetTextureName()
    {
        return "chip_group_triangle";
    }
}

public class SquareChipGroup : ChipGroup
{
    public SquareChipGroup(List<DiceChip> diceChips) : base(diceChips)
    {
    }

    public override string GetID()
    {
        return "chip_group_square";
    }

    public override string GetTextureName()
    {
        return "chip_group_square";
    }
}

public class CircleChipGroup : ChipGroup
{
    public CircleChipGroup(List<DiceChip> diceChips) : base(diceChips)
    {
    }

    public override string GetID()
    {
        return "chip_group_circle";
    }

    public override string GetTextureName()
    {
        return "chip_group_circle";
    }
}