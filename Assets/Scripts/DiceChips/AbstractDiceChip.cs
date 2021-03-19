using System;
using System.Linq;
using UnityEngine;

public abstract class DiceChip
{
    int[] values;
    public bool isFaceUp = false; //faceup = we can see the dice value, facedown = we see the group symbol

    public DiceChip(int[] values)
    {
        this.values = values;
        this.isFaceUp = false;
    }

    public abstract string GetTextureName();

    //public Sprite GetSprite()
    //{
    //    string textureName = this.isFaceUp ? GetTextureName() : chipGroup.GetTextureName();

    //    Texture2D texture = Resources.Load(textureName) as Texture2D;
    //    Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.0f, 0.0f));
    //    return sprite;

    //}

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

    public void SetFaceUp()
    {
        isFaceUp = true;
    }

    public void SetFaceDown()
    {
        isFaceUp = false;
    }
    

    public bool fulfillsDiceThrow(int diceNumber)
    {
        return this.values.Contains(diceNumber);
    }
}
