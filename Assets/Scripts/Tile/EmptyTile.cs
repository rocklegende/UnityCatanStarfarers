using System;
using UnityEngine;

public class EmptyTile : Tile_ { 
    public EmptyTile()
    {
    }

    public override Color? GetColor()
    {
        return Color.white;
    }
}
