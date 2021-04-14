using System;

[Serializable]
public class FoodCard : ResourceCard
{
    public FoodCard() : base(new FoodResource())
    {
    }
}
