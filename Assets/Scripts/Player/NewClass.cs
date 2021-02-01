using System;
public class SpacePortToken : Token
{
    public SpacePortToken(): base("spaceport_token", false, new Cost(new Resource[] { new CarbonResource(), new CarbonResource(), new CarbonResource(), new FoodResource(), new FoodResource() }))
    {
    }

    public override int BaseVictoryPoints()
    {
        return 1;
    }

    public override int ResourceProduce()
    {
        return 1;
    }

}

public class ColonyBaseToken : Token
{
    public ColonyBaseToken() : base("colonybase_token", true, new Cost(new Resource[] { new FuelResource(), new OreResource(), new CarbonResource(), new FoodResource() }))
    {
    }

    public override int BaseVictoryPoints()
    {
        return 1;
    }

    public override int ResourceProduce()
    {
        return 1;
    }
}

public class TradeBaseToken : Token
{
    public TradeBaseToken() : base("tradebase_token", true, new Cost(new Resource[] { new OreResource(), new FuelResource(), new GoodsResource(), new GoodsResource() }))
    {
    }

    public override int BaseVictoryPoints()
    {
        return 1;
    }

    public override int ResourceProduce()
    {
        return 0;
    }
}

public class ShipToken : Token
{
    public ShipToken() : base("ship_token", false, new Cost(new Resource[] { }))
    {
    }

    public override int BaseVictoryPoints()
    {
        return 0;
    }

    public override int ResourceProduce()
    {
        return 0;
    }
}

public class BoosterUpgradeToken : Token
{
    public BoosterUpgradeToken() : base("booster_upgrade", false, new Cost(new Resource[] { new FuelResource(), new FuelResource() }))
    {

    }

    public override int BaseVictoryPoints()
    {
        return 0;
    }

    public override int ResourceProduce()
    {
        return 0;
    }
}

public class CannonUpgradeToken : Token
{
    public CannonUpgradeToken() : base("cannon_upgrade", false, new Cost(new Resource[] { new CarbonResource(), new CarbonResource() }))
    {

    }

    public override int BaseVictoryPoints()
    {
        return 0;
    }

    public override int ResourceProduce()
    {
        return 0;
    }
}

public class FreightPodUpgradeToken : Token
{
    public FreightPodUpgradeToken() : base("freight_pod_upgrade", false, new Cost(new Resource[] { new OreResource(), new OreResource() }))
    {

    }

    public override int BaseVictoryPoints()
    {
        return 0;
    }

    public override int ResourceProduce()
    {
        return 0;
    }
}

