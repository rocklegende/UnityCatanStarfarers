using System;
using UnityEngine;
public abstract class Resource
{
    public String Name;
    public Color tileColor;
    public string cardImageName;
    public Resource(String name, Color tileColor, string cardImageName)
    {
        this.Name = name;
        this.tileColor = tileColor;
        this.cardImageName = cardImageName;
    }
}
