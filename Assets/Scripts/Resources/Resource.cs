using System;
using UnityEngine;
public abstract class Resource
{
    public String Name;
    public Color cardImage;
    public Color planetImage;
    public Resource(String name, Color cardImage, Color planetImage)
    {
        this.Name = name;
        this.cardImage = cardImage;
        this.planetImage = planetImage;
    }
}
