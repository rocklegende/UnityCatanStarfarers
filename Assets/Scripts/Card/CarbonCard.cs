using System;

[Serializable]
public class CarbonCard : ResourceCard
{
    public CarbonCard() : base(new CarbonResource())
    {
    }
}
