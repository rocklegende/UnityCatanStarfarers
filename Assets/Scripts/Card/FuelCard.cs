using System;

[Serializable]
public class FuelCard : ResourceCard
{
    public FuelCard() : base(new FuelResource())
    {
    }
}
