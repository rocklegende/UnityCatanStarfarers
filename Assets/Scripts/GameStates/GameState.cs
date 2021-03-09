﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState
{
    protected GameController controller;
    protected HUDScript hudScript;
    protected MapScript mapScript;
    public GameState(GameController controller)
    {
        this.controller = controller;
        this.hudScript = controller.HUD.GetComponent<HUDScript>();
        this.mapScript = controller.Map.GetComponent<MapScript>();
    }

    public abstract void OnSpacePointClicked(SpacePoint point, GameObject spacePointObject);
    public abstract void OnTokenClicked(Token tokenModel, GameObject tokenGameObject);
    public abstract void OnNextButtonClicked();
    public abstract void OnBackButtonClicked();
    public abstract void OnBuildShipOptionClicked(Token token);
    public abstract void OnBuildUpgradeOptionClicked(Token token);
    public abstract void OnSettleButtonPressed();
    public abstract void OnShipDiceThrown(ShipDiceThrow shipDiceThrow);
    public abstract void OnTokenCanSettle(bool canSettle, Token token);
    public abstract void Setup();


}