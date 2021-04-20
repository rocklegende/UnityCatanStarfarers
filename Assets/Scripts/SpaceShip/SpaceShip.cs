using System;
using System.Collections.Generic;
using UnityEngine;

public enum ShipUpgrade
{
    CANNON,
    FREIGHTPOD,
    BOOSTER
}

[Serializable]
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

    public int UpgradesCountWithoutBonuses()
    {
        return Boosters + FreightPods + Cannons;
    }

    public  List<Token> GetRemovableTokens()
    {
        var list = new List<Token>();
        if (Cannons > 0)
        {
            list.Add(new CannonUpgradeToken());
        }

        if (Boosters > 0)
        {
            list.Add(new BoosterUpgradeToken());
        }

        if (FreightPods > 0)
        {
            list.Add(new FreightPodUpgradeToken());
        }
        return list;
    }

    public List<Token> GetUpgradesThatAreNotFull()
    {
        var list = new List<Token>();
        if (!IsCannonsFull())
        {
            list.Add(new CannonUpgradeToken());
        }

        if (!IsBoostersFull())
        {
            list.Add(new BoosterUpgradeToken());
        }

        if (!IsFreightPodsFull())
        {
            list.Add(new FreightPodUpgradeToken());
        }
        return list;
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
        if (token is BoosterUpgradeToken)
        {
            Boosters -= 1;
            if (Boosters < 0)
            {
                Boosters = 0;
            }
        }
        else if (token is CannonUpgradeToken)
        {
            Cannons -= 1;
            if (Cannons < 0)
            {
                Cannons = 0;
            }
        }
        else if (token is FreightPodUpgradeToken)
        {
            FreightPods -= 1;
            if (FreightPods < 0)
            {
                FreightPods = 0;
            }
        }
        else
        {

        }
        changedCallback();
    }
}
