using System;
using UnityEngine;

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

public class KhornemOneBoostOneCannon : KhornemFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.BuildUpgradeWithoutCost(new BoosterUpgradeToken());
        targetPlayer.BuildUpgradeWithoutCost(new CannonUpgradeToken());
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

public class KhornemTwoBoost : KhornemFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.BuildUpgradeWithoutCost(new BoosterUpgradeToken());
        targetPlayer.BuildUpgradeWithoutCost(new BoosterUpgradeToken());
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

public class KhornemTwoCannon : KhornemFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.BuildUpgradeWithoutCost(new CannonUpgradeToken());
        targetPlayer.BuildUpgradeWithoutCost(new CannonUpgradeToken());
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

