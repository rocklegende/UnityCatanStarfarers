using System;
using UnityEngine;

public enum ShipUpgrade
{
    CANNON,
    FREIGHTPOD,
    BOOSTER
}

public class SpaceShip
{
    public int Cannons = 0;
    public int CannonsMaxCapacity = 6;
    public int CannonsBonus = 0;

    public int FreightPods = 0;
    public int FreightPodsMaxCapacity = 5;
    public int FreightPodsBonus = 0;

    public int Boosters = 0;
    public int BoostersMaxCapacity = 6;
    public int BoostersBonus = 0;

    Action changedCallback;

    public SpaceShip(Action changedCallback)
    {
        this.changedCallback = changedCallback;
    }

    public void Add(Token token)
    {
        if (token is BoosterUpgradeToken)
        {
            Boosters += 1;
        } else if (token is CannonUpgradeToken)
        {
            Cannons += 1;
        } else if (token is FreightPodUpgradeToken)
        {
            FreightPods += 1;
        } else
        {
            
        }
        changedCallback();
    }

    public int GetStrength(ShipUpgrade upgradeType)
    {
        switch (upgradeType)
        {
            case ShipUpgrade.CANNON:
                return Cannons + CannonsBonus;
            case ShipUpgrade.BOOSTER:
                return Boosters + BoostersBonus;
            case ShipUpgrade.FREIGHTPOD:
                return FreightPods + FreightPodsBonus;
        }
        return 0;
    }

    public void AddBonusForUpgrade(ShipUpgrade upgradeType, int amount)
    {
        if (amount > 2 && amount < 1)
        {
            throw new ArgumentException("Shouldnt be possible to add more than 2 bonus upgrades");
        };
        switch (upgradeType)
        {
            case ShipUpgrade.CANNON:
                CannonsBonus += amount;
                break;
            case ShipUpgrade.BOOSTER:
                BoostersBonus += amount;
                break;
            case ShipUpgrade.FREIGHTPOD:
                FreightPodsBonus += amount;
                break;
        }
        changedCallback();
    }

    public bool IsUpgradeRackFull(ShipUpgrade upgradeType)
    {
        switch (upgradeType)
        {
            case ShipUpgrade.CANNON:
                return Cannons >= CannonsMaxCapacity;
            case ShipUpgrade.BOOSTER:
                return Boosters >= BoostersMaxCapacity;
            case ShipUpgrade.FREIGHTPOD:
                return FreightPods >= FreightPodsMaxCapacity;
        }
        return false;
    }

    public bool IsCannonsFull()
    {
        return IsUpgradeRackFull(ShipUpgrade.CANNON);
    }

    public bool IsBoostersFull()
    {
        return IsUpgradeRackFull(ShipUpgrade.BOOSTER);
    }

    public bool IsFreightPodsFull()
    {
        return IsUpgradeRackFull(ShipUpgrade.FREIGHTPOD);
    }

    public void Remove(Token token)
    {
        //TODO
    }
}
