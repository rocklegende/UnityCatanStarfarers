using System;
using UnityEngine;

[Serializable]
public abstract class Resource
{
    public String Name;
    public SFColor tileColor;
    public string cardImageName;
    public Resource(String name, SFColor tileColor, string cardImageName)
    {
        this.Name = name;
        this.tileColor = tileColor;
        this.cardImageName = cardImageName;
    }
}
