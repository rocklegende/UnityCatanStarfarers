using System;

[Serializable]
public class OreCard : ResourceCard
{
    public OreCard() : base(new OreResource())
    {
    }
}
