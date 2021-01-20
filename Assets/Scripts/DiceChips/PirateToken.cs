using System;
using UnityEngine;



public abstract class PirateTokenBeatCondition
{
    public PirateTokenBeatCondition()
    {
        
    }

    public abstract bool SpaceShipFullfillsCondition(SpaceShip ship);
    public abstract string GetTextureName();
}

public class FreightPodsBeatCondition : PirateTokenBeatCondition
{

    int minNumFreightPods;
    public FreightPodsBeatCondition(int minNumFreightPods) : base()
    {
        this.minNumFreightPods = minNumFreightPods;
    }

    public override bool SpaceShipFullfillsCondition(SpaceShip ship)
    {
        return ship.FreightPods >= minNumFreightPods;
    }

    public override string GetTextureName()
    {
        return String.Format("pirate_token_freightpod_{0}", minNumFreightPods);
    }
}

public class CannonBeatCondition : PirateTokenBeatCondition
{

    int minCannons;
    public CannonBeatCondition(int minCannons) : base()
    {
        this.minCannons = minCannons;
    }

    public override bool SpaceShipFullfillsCondition(SpaceShip ship)
    {
        return ship.Cannons >= minCannons;
    }

    public override string GetTextureName()
    {
        return String.Format("pirate_token_cannon_{0}", minCannons);
    }
}



public class PirateToken : DiceChip
{
    bool isBeaten = false;
    PirateTokenBeatCondition condition;

    public PirateToken(PirateTokenBeatCondition condition, ChipGroup chipGroup) : base(new int[] { 0, 0 }, chipGroup)
    {
        this.condition = condition;
    }

    public void AttemptBeatingIt(SpaceShip spaceShip)
    {
        if (condition.SpaceShipFullfillsCondition(spaceShip))
        {
            isBeaten = true;

            //app.Notify("Beaten", this)
        }
    }

    public override string GetTextureName()
    {
        return condition.GetTextureName();
    }


}
