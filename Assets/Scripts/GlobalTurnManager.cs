using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Linq;



public class GlobalTurnManager
{
    List<Photon.Realtime.Player> turnOrder = new List<Photon.Realtime.Player>();
    int currentStep = 1;
    bool isRandomTurnOrder = false;
    bool skipSetupPhase = true;
    GameController gameController;
    
    public GlobalTurnManager(GameController gameController)
    {
        this.gameController = gameController;
        turnOrder = GenerateTurnOrder();
    }

    enum TurnType
    {
        NORMAL,
        SETUP
    }

    List<Photon.Realtime.Player> GenerateTurnOrder()
    {
        var list = PhotonNetwork.CurrentRoom.Players.Values.ToList();

        if (isRandomTurnOrder)
        {
            list.Shuffle();
        }
        return list;
    }

    public void StartGameLoop()
    {
        
        //2x initial setup step
        //"infinite" game loop
        //game is finished, send message to every Player
        if (skipSetupPhase)
        {
            currentStep += GetNumSetupSteps();
        }
        PlayNextStep();
    }

    public void SkipSetupPhase()
    {
        //currentStep = GetNumSetupSteps();
        //PlayNextStep();
    }

    int GetNumSetupSteps()
    {
        return 2 * turnOrder.Count;
    }

    public void TurnCompletedByPlayer(Photon.Realtime.Player photonPlayer)
    {
        if (photonPlayer != GetCurrentPlayerAtTurn())
        {
            throw new System.ArgumentException("Player who send that he completed the turn was not eactually at turn, this should never happen");
        } else
        {
            PlayNextStep();
        }
    }

    public Photon.Realtime.Player GetCurrentPlayerAtTurn()
    {
        var index = GetCurrentPlayerAtTurnIndex();
        var bla = turnOrder;
        var bla1 = currentStep;
        return turnOrder[index];
    }

    public int GetCurrentPlayerAtTurnIndex()
    {
        var bla = turnOrder;
        var bla1 = currentStep;
        return (currentStep - 1) % turnOrder.Count;
    }

    void PlayNextStep()
    {
        if (currentStep > 1)
        {
            DeactivateTurnForPlayer(GetCurrentPlayerAtTurn());
        }
        var numPlayers = turnOrder.Count;
        

        Debug.Log("Turn order count " + turnOrder.Count);
        Debug.Log("Current step: " + currentStep);
        if (currentStep <= 2 * numPlayers)
        {
            currentStep++;
            ActivateTurnForPlayer(GetCurrentPlayerAtTurn(), TurnType.SETUP);
        } else
        {
            currentStep++;
            ActivateTurnForPlayer(GetCurrentPlayerAtTurn(), TurnType.NORMAL);
        }
        
    }

    void ActivateTurnForPlayer(Photon.Realtime.Player photonPlayer, TurnType turnType)
    {
        PhotonView photonView = PhotonView.Get(gameController);
        if (turnType == TurnType.NORMAL)
        {
            photonView.RPC(RpcMethods.ActivateNormalPlayStep, photonPlayer);
        }

        if (turnType == TurnType.SETUP)
        {
            photonView.RPC(RpcMethods.ActivateSetupPlayStep, photonPlayer);
        }

    }
    void DeactivateTurnForPlayer(Photon.Realtime.Player photonPlayer)
    {
        Debug.Log("Deactivating turn for player" + photonPlayer.NickName);

        PhotonView photonView = PhotonView.Get(gameController);
        photonView.RPC(RpcMethods.DeactivatePlayStep, photonPlayer);        
    }

}
