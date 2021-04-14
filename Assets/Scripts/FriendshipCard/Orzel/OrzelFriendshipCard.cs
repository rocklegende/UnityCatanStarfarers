using System;
using UnityEngine;
[Serializable]
public abstract class OrzelFriendshipCard : AbstractFriendshipCard
{
    public OrzelFriendshipCard()
    {
    }

    public override string GetTradeStationName()
    {
        return "Orzel";
    }

    public override Sprite GetBackgroundSprite()
    {
        return new Helper().CreateSpriteFromImageName("orzel_trading_bg");
    }


}

[Serializable]
public class OrzelFriendshipCardFood : OrzelFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.ChangeFoodTradingRatio(2);
    }

    public override Sprite GetEffectSprite()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "2:1, food";
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class OrzelFriendshipCardGoods : OrzelFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.ChangeGoodsTradingRatio(1);
    }

    public override Sprite GetEffectSprite()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "1:1, goods";
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class OrzelFriendshipCardOre : OrzelFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.ChangeOreTradingRatio(2);
    }

    public override Sprite GetEffectSprite()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "Change ore trading ratio to 2:1";
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class OrzelFriendshipCardCarbon : OrzelFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.ChangeCarbonTradingRatio(2);
    }

    public override Sprite GetEffectSprite()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "2:1, carbon";
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class OrzelFriendshipCardFuel : OrzelFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.ChangeFuelTradingRatio(2);
    }

    public override Sprite GetEffectSprite()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "2:1, fuel";
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }
}