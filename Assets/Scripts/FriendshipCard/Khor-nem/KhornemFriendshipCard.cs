using System;
using UnityEngine;

[Serializable]
public abstract class KhornemFriendshipCard : AbstractFriendshipCard
{
    public KhornemFriendshipCard()
    {
    }

    public override string GetTradeStationName()
    {
        return "Khornem";
    }

    public override Sprite GetBackgroundSprite()
    {
        return new Helper().CreateSpriteFromImageName("khornem_trading_bg");
    }

}

[Serializable]
public class KhornemOneBoostOneCannon : KhornemFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.ship.AddBonusForUpgrade(ShipUpgrade.BOOSTER, 1);
        targetPlayer.ship.AddBonusForUpgrade(ShipUpgrade.CANNON, 1);
    }

    public override Sprite GetEffectSprite()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "Get one cannon and one boost added";
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }
}


[Serializable]
public class KhornemTwoBoost : KhornemFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.ship.AddBonusForUpgrade(ShipUpgrade.BOOSTER, 2);
    }

    public override Sprite GetEffectSprite()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "Get two boosts added free of charge";
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }

}

[Serializable]
public class KhornemTwoCannon : KhornemFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.ship.AddBonusForUpgrade(ShipUpgrade.CANNON, 2);
    }

    public override Sprite GetEffectSprite()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "Get two cannons added free of charge";
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }

}

