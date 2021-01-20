using System;
using System.Linq;
using UnityEngine;

public abstract class DiceChip
{
    int[] values;
    public bool isFaceUp = false;
    public ChipGroup chipGroup;

    public DiceChip(int[] values, ChipGroup chipGroup)
    {
        this.values = values;
        this.isFaceUp = false;
        this.chipGroup = chipGroup;
    }

    public abstract string GetTextureName();

    public Sprite GetSprite()
    {
        string textureName = this.isFaceUp ? GetTextureName() : chipGroup.GetTextureName();

        Texture2D texture = Resources.Load(textureName) as Texture2D;
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.0f, 0.0f));
        return sprite;

    }

    public void Flip()
    {
        if (isFaceUp)
        {
            isFaceUp = false;
        } else
        {
            isFaceUp = true;
        }
    }
    

    public bool fulfillsDiceThrow(int diceNumber)
    {
        return this.values.Contains(diceNumber);
    }
}
