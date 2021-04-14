using System;

[Serializable]
public class GoodsCard : ResourceCard
{
    public GoodsCard() : base(new GoodsResource())
    {
    }
}
