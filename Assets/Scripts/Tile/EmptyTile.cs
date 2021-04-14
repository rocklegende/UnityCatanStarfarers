using System;
using UnityEngine;

public interface SFSerializable
{
    byte[] Serialize();
    object Deserialize(byte[] bytes);
}

[Serializable]
public class EmptyTile : Tile_ { 
    public EmptyTile()
    {
    }

    public override Color? GetColor()
    {
        return new Color(54.0F / 255.0F, 54.0F / 255.0F, 54.0F / 255.0F);
    }

    public override bool blocksTraffic()
    {
        return false;
    }
}
