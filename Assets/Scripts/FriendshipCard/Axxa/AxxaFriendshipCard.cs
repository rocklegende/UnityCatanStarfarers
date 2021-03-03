using System;
using UnityEngine;

public abstract class AxxaFriendshipCard : AbstractFriendshipCard
{
    public AxxaFriendshipCard()
    {
    }

    public override string GetTradeStationName()
    {
        return "Axxa";
    }

    public override Sprite GetBackgroundSprite()
    {
        return new Helper().CreateSpriteFromImageName("axxa_trading_bg");
    }

}

public class AxxaIncreaseFood : AxxaFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.IncreaseFoodBonus(1);
    }

    public override Sprite GetEffectSprite()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "Get one extra food if one of your colonies produces food";
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }
}

public class AxxaIncreaseGoods : AxxaFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.IncreaseGoodsBonus(1);
    }

    public override Sprite GetEffectSprite()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "Get one extra goods if one of your colonies produces food";
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }
}

public class AxxaIncreaseOre : AxxaFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.IncreaseOreBonus(1);
    }

    public override Sprite GetEffectSprite()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "Get one extra ore if one of your colonies produces food";
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }
}

public class AxxaIncreaseFuel : AxxaFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.IncreaseFuelBonus(1);
    }

    public override Sprite GetEffectSprite()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "Get one extra fuel if one of your colonies produces food";
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }
}

public class AxxaIncreaseCarbon : AxxaFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.IncreaseCarbonBonus(1);
    }

    public override Sprite GetEffectSprite()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "Get one extra carbon if one of your colonies produces food";
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }
}