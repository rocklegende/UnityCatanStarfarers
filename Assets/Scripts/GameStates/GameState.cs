using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState
{
    GameController controller;
    public GameState(GameController controller)
    {
        this.controller = controller;
    }

    public abstract void OnSpacePointClicked(SpacePoint point, GameObject spacePointObject);
    public abstract void OnTokenClicked(Token tokenModel, GameObject tokenGameObject);
    public abstract void OnNextButtonClicked();
    public abstract void OnBackButtonClicked();
    public abstract void OnBuildShipOptionClicked(Token token);
    public abstract void OnBuildUpgradeOptionClicked(Token token);


}