using System;
using UnityEngine;

public abstract class Card
{
    private string imageName;

    public Card(string imageName)
    {
        this.imageName = imageName;
    }
}
