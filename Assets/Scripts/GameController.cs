using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using com.onebuckgames.UnityStarFarers;


public interface IGameController
{
    List<Player> GetPlayers();
    Player GetMainPlayer();
    MapScript GetMapScript();
    HUDScript GetHUDScript();
}

//public enum RemoteClientActionType
//{
//    GIVE_RESOURCE,
//    GIVEUP_UPGRADE,
//    SEVEN_ROLL_DISCARD,
//    TRADE_OFFER
//}

[System.Serializable]
public class RemoteActionCallbackData
{
    public readonly Player player;
    public readonly object data;

    public RemoteActionCallbackData(Player player, object data = null)
    {
        this.player = player;
        this.data = data;
    }
}

namespace com.onebuckgames.UnityStarFarers
{
    [System.Serializable]
    public abstract class RemoteClientAction
    {
        public readonly object[] parameters;
        public readonly Player sendFromPlayer;
        public readonly bool isBlockingInteraction;

        public RemoteClientAction(object[] parameters, Player sendFromPlayer, bool isBlockingInteraction)
        {
            this.parameters = parameters;
            this.sendFromPlayer = sendFromPlayer;
            this.isBlockingInteraction = isBlockingInteraction;
        }
    }

    [System.Serializable]
    public class DiscardRemoteClientAction : RemoteClientAction
    {
        public DiscardRemoteClientAction(Player sendFromPlayer) : base(null, sendFromPlayer, true)
        {

        }
    }

    [System.Serializable]
    public class TradeOfferRemoteClientAction : RemoteClientAction
    {
        public readonly TradeOffer tradeOffer;
        public TradeOfferRemoteClientAction(TradeOffer tradeOffer, Player sendFromPlayer) : base(null, sendFromPlayer, false)
        {
            this.tradeOffer = tradeOffer;
        }
    }

    [System.Serializable]
    public class GiveResourceRemoteClientAction : RemoteClientAction
    {
        public readonly int numResources;
        public GiveResourceRemoteClientAction(int numResources, Player sendFromPlayer) : base(null, sendFromPlayer, true)
        {
            this.numResources = numResources;
        }
    }

    [System.Serializable]
    public class GiveUpgradeRemoteClientAction : RemoteClientAction
    {
        public readonly int numUpgradesToDiscard;
        public GiveUpgradeRemoteClientAction(int numUpgradesToDiscard, Player sendFromPlayer) : base(null, sendFromPlayer, true)
        {
            this.numUpgradesToDiscard = numUpgradesToDiscard;
        }
    }
}

[System.Serializable]
public class GameStartInformation
{
    public Map map { get; }
    public GameStartInformation(Map map)
    {
        this.map = map;
    }
}

public static class RpcMethods
{
    public static string OtherPlayerCancelledTrade = "OtherPlayerCancelledTrade";
    public static string ActivateNormalPlayStep = "ActivateNormalPlayStep";
    public static string ActivateSetupPlayStep = "ActivateSetupPlayStep";
    public static string DeactivatePlayStep = "DeactivatePlayStep";
}

public class RpcCall
{
    public readonly string methodName;
    public readonly Photon.Pun.RpcTarget target;
    
    public RpcCall(string methodName, Photon.Pun.RpcTarget target)
    {
        this.methodName = methodName;
        this.target = target;
    }
}


public class GameController : SFController, IGameController, Observer
{
    public GameObject HUD;
    public GameObject Map;
    public Map mapModel
    {
        get
        {
            if (currentGameStateInfo == null)
            {
                return null;
            }
            return currentGameStateInfo.mapmodel;
        }
    }

    private GameState _state;
    public GameState State {
        get { return _state; }
        set { _state = value; OnStateChanged(); }
    }

    public SFGameClient_ gameClient;
    public string mainPlayerGuid;
    public GameObject photonServiceObj;
    public List<Player> players
    {
        get 
        {
            if (currentGameStateInfo == null)
            {
                return null;
            }
            return currentGameStateInfo.players;
        }
    }

    public Player mainPlayer {
        get
        {
            if (mainPlayerGuid != null && players != null)
            {

                return players.Find(p => p.name == mainPlayerGuid);
            }
            return null;
        }
    }
    public PayoutHandler payoutHandler;

    public EncounterCardHandler encounterCardHandler;
    public DrawPileHandler drawPileHandler;
    public DebugStartState debugStartState;
    public Photon.Realtime.Player recentPhotonResponsePlayer;
    public int numPlayersUpdated = 0;
    public int numMapUpdated = 0;
    public List<RpcCall> recentRpcCalls = new List<RpcCall>();
    public RemoteActionDispatcher dispatcher;
    public string stateText;
    private SFGameStateInfo currentGameStateInfo;
    private bool turnWasReceived = false;

    /// <summary>
    /// Run GameController in testMode
    /// </summary>
    private bool testMode = false;
    private System.Action<RemoteActionCallbackData> dispatcherSomeoneFullfilledActionCallback;

    void Awake()
    {
        var photonService = photonServiceObj.GetComponent<PhotonService>(); 
        gameClient = new SFGameClient_();
        gameClient.photonService = photonService;
        photonService.sfGameClient = gameClient;
        gameClient.gameController = this;
        gameClient.Start();
    }

    void Update()
    {
        
    }

    void SaveGameState()
    {
        //We made changes to the game state, upload them so others get it
        //var currentState = GetCurrentState();
        gameClient.SaveGameStateToProps(currentGameStateInfo);
    }

    //SFGameStateInfo GetCurrentState()
    //{
    //    //var info = new SFGameStateInfo();
    //    //info.players = players;
    //    //info.mapmodel = mapModel;
    //    //info.state = stateText;
    //    //info.turnNumber = currentGameStateInfo.turnNumber;
    //    //info.playerToActTurnOrderPosition = currentGameStateInfo.playerToActTurnOrderPosition;
    //    //return info;
    //}

    public bool IsMyTurn
    {
        get
        {
            if (currentGameStateInfo == null)
            {
                return false;
            }
            //Debug.Log(PlayerIdToMakeThisTurn + "'s turn. You are: " + this.LocalPlayer.ID);
            return this.currentGameStateInfo.playerToActTurnOrderPosition == mainPlayer.TurnOrderPosition;
        }
    }

    public void OnGameStateChangedRemotely(SFGameStateInfo newGameState) //interface method that will be called from the SFGameClient
    {
        // others changed the state, update our ui
        Debug.Log("OnGameStateChangedRemotely");
        LoadGameState(newGameState);
    }

    public void Initialize(SFGameStateInfo info, string mainPlayerGuid)
    {
        currentGameStateInfo = info;
        this.mainPlayerGuid = mainPlayerGuid;
        mapModel.RegisterObserver(this);
        foreach (var group in mapModel.tileGroups)
        {
            group.RegisterObserver(mapModel);
        }
        this._state = new BuildAndTradeState(this);
        Debug.Log("Initializing GameController");
        foreach (var player in players)
        {
            Debug.Log(player.guid);
        }
        Debug.Log("main player guid" + mainPlayerGuid);
        Init(info);
        encounterCardHandler.encounterCardStack = new DefaultEncounterCardStack(this);
        dispatcher = new DefaultRemoteActionDispatcher(this);
    }

    void Start()
    {
        

    }

    void OnStateChanged()
    {
        RunRPC("RemoteClientChangedState", RpcTarget.Others, State.GetType().Name);
    }

    

    public void ActivateAllInteraction(bool isActivated)
    {
        GetHUDScript().ActivateAllInteraction(isActivated);
        GetMapScript().ActivateAllInteraction(isActivated);
    }

    [PunRPC]
    void RemoteClientChangedState(string newStateName)
    {
        GetHUDScript().SetStateText(newStateName);
    }


    //[PunRPC]
    //void GameStartInformationGenerated(byte[] gameStartInformationAsBytes)
    //{
    //    var gameStartInformation = (GameStartInformation)SFFormatter.Deserialize(gameStartInformationAsBytes);

    //    //setup the observers, because after serialization we always lose the observers
    //    mapModel = gameStartInformation.map;
    //    mapModel.RegisterObserver(this);
    //    foreach(var group in mapModel.tileGroups)
    //    {
    //        group.RegisterObserver(mapModel);
    //    }
    //    this._state = new BuildAndTradeState(this);
    //    Init();

    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        globalTurnManager = new GlobalTurnManager(this);
    //        globalTurnManager.StartGameLoop();
    //    }
    //}

    /// <summary>
    /// Initialize the gamecontroller without Photon. Useful for test purposes.
    /// </summary>
    /// <param name="map"></param>
    /// <param name="players"></param>
    /// <param name="mainPlayer"></param>
    public void DevelopmentGameStartInformationGenerated(Map map, List<Player> players, Player mainPlayer)
    {
        PhotonNetwork.OfflineMode = true;
        testMode = true;

        //mapModel = map;
        //mapModel.RegisterObserver(this);
        //this._state = new BuildAndTradeState(this);

        //this.players = players;
        //this.mainPlayerGuid = mainPlayer.guid;

        Init(new SFGameStateInfo());
    }

    public void Init(SFGameStateInfo gameStateInfo)
    {
        //InitialPlayerSetup();
        ObservePlayers(players);
        var observersOfMainPlayer = mainPlayer.GetObservers();
        HUD.GetComponent<HUDScript>().Init();
        Map.GetComponent<MapScript>().Init();
        payoutHandler = new PayoutHandler(mapModel);
        observersOfMainPlayer = mainPlayer.GetObservers();
        LoadGameState(gameStateInfo);
        observersOfMainPlayer = mainPlayer.GetObservers();
    }

    public void IFinishedMyTurn()
    {
        turnWasReceived = false;
        gameClient.HandoverTurnToNextPlayer();        
    }

    [PunRPC]
    void ActivateNormalPlayStep()
    {
        
        Debug.Log("Im requested to play a normal step");
        ActivateAllInteraction(true);
        PayoutLowPointsBonus(mainPlayer);
        mainPlayer.OnTurnReceived();
        SetState(new BuildAndTradeState(this));
    }

    [PunRPC]
    public void ActivateSetupPlayStep()
    {
        Debug.Log("Im requested to play a setup step");
        ActivateAllInteraction(true);
        SetState(new InitialSetupState(this));
    }

    [PunRPC]
    void DeactivatePlayStep()
    {
        ActivateAllInteraction(false);
    }

    public int OwnPlayerIndex()
    {
        return players.FindIndex(p => p.name == mainPlayer.name);
    }

    public void SetupButtonPressed()
    {
        //SetUpDebugState(new TwoTradeShipAndOneSpacePort(this, "encounter"));
        InitialPlayerSetup();

        //var actionInfo = new RemoteClientAction(RemoteClientActionType.GIVE_RESOURCE, null, GetIndexOfPlayer(GetMainPlayerFromDict()));
        //RunRPC("RemoteClientRequiresAction", RpcTarget.Others, SFFormatter.Serialize(actionInfo));

        //var encounterFactory = new EncounterCardFactory(this);
        //this.encounterCardHandler.PlayEncounterCard(encounterFactory.CreateEncounterCard32());

        //On7Rolled();


    }

    public void OpenTokenSelectionForMainPlayer()
    {
        GetHUDScript().OnPlayerDataChanged();
        GetMapScript().OnMapDataChanged();
    }

    public void RequestActionFromPlayers(
        RemoteClientAction remoteClientAction,
        List<Player> targets,
        System.Action<RemoteActionCallbackData> singleResponseReceivedCallback,
        System.Action<Dictionary<string, RemoteActionCallbackData>> allResponsesReceivedCallback
    )
    {
        dispatcher.SetTargets(targets);
        dispatcher.SetAction(remoteClientAction);
        dispatcher.MakeRequest(singleResponseReceivedCallback, allResponsesReceivedCallback);
    } 

    [PunRPC]
    public void RemoteClientRequiresAction(byte[] actionInfo)
    {
        var remoteAction = (RemoteClientAction)SFFormatter.Deserialize(actionInfo);

        if (remoteAction is GiveResourceRemoteClientAction)
        {
            HandleGiveResourceActionFromRemoteClient((GiveResourceRemoteClientAction)remoteAction);
            return;
        }

        if (remoteAction is GiveUpgradeRemoteClientAction)
        {
            HandleGiveupUpgradeActionFromRemoteClient((GiveUpgradeRemoteClientAction)remoteAction);
            return;
        }

        if (remoteAction is DiscardRemoteClientAction)
        {
            HandleSevenRollDiscardActionFromRemoteClient((DiscardRemoteClientAction)remoteAction);
            return;
        }

        if (remoteAction is TradeOfferRemoteClientAction)
        {
            HandleTradeOfferActionFromRemoteClient((TradeOfferRemoteClientAction)remoteAction);
            return;
        }
    }

    [PunRPC]
    public void OtherPlayerCancelledTrade()
    {
        GetHUDScript().tradeOfferView.SetActive(false);
    }

    void HandleTradeOfferActionFromRemoteClient(TradeOfferRemoteClientAction remoteAction)
    {
        var tradeOffer = remoteAction.tradeOffer;
        GetHUDScript().DisplayTradeOffer(tradeOffer, (accept) =>
        {
            SendResponseBackToCallerOfRemoteClientAction(remoteAction, new RemoteActionCallbackData(mainPlayer, accept));
        });
    }
    
    void HandleGiveResourceActionFromRemoteClient(GiveResourceRemoteClientAction remoteAction)
    {
        var requiredResources = remoteAction.numResources;
        GetHUDScript().OpenDiscardResourcePicker((hand) => {
            Debug.Log("hand picked, count: " + hand.Count());

            var sfTargetPlayer = remoteAction.sendFromPlayer;
            mainPlayer.PayToOtherPlayer(sfTargetPlayer, hand);
            Debug.Log("paying hand to player: " + sfTargetPlayer.name);

            SendResponseBackToCallerOfRemoteClientAction(remoteAction);
        }, requiredResources, requiredResources);
    }

    void HandleGiveupUpgradeActionFromRemoteClient(GiveUpgradeRemoteClientAction remoteAction)
    {
        var upgradesToDiscard = remoteAction.numUpgradesToDiscard;

        var encounterAction = new RemoveUpgradeAction(this);
        encounterAction.callback = ((value) => SendResponseBackToCallerOfRemoteClientAction(remoteAction));
        encounterAction.Execute();
    }

    void HandleSevenRollDiscardActionFromRemoteClient(DiscardRemoteClientAction remoteAction)
    {
        var numCardsToDump = On7RollStrategy.NumCardsToDump(mainPlayer.hand.Count());
        GetHUDScript().OpenDiscardResourcePicker((handPicked) =>
        {
            mainPlayer.SubtractHand(handPicked);
            SendResponseBackToCallerOfRemoteClientAction(remoteAction);
        }, numCardsToDump, numCardsToDump);
    }

    void SendResponseBackToCallerOfRemoteClientAction(RemoteClientAction remoteAction)
    {
        SendResponseBackToCallerOfRemoteClientAction(remoteAction, new RemoteActionCallbackData(mainPlayer));
    }

    void SendResponseBackToCallerOfRemoteClientAction(RemoteClientAction remoteAction, RemoteActionCallbackData data)
    {
        var sfTargetPlayer = remoteAction.sendFromPlayer;
        var photonTargetPlayer = gameClient.SFPlayerToPhotonPlayer(sfTargetPlayer);
        recentPhotonResponsePlayer = photonTargetPlayer;
        Debug.Log("Sending response to player: " + photonTargetPlayer.NickName);
        RunRPC("RemoteClientFulfilledAction", photonTargetPlayer, SFFormatter.Serialize(data));
    }

    /// <summary>
    /// Other client finished its trade offer proposal, either by accepting, declining or arborting the trade offer.
    /// </summary>
    [PunRPC]
    public void RemoteClientTradeOfferFinished()
    {
        GetHUDScript().tradeOfferView.SetActive(false);
    }

    

    [PunRPC]
    void RemoteClientFulfilledAction(byte[] data)
    {
        var remoteCallbackData = (RemoteActionCallbackData) SFFormatter.Deserialize(data);
        var playerWhoFullfilledAction = remoteCallbackData.player;
        Debug.Log("Response from " + playerWhoFullfilledAction.name + " received");
        dispatcherSomeoneFullfilledActionCallback?.Invoke(remoteCallbackData);
    }

    void InitialPlayerSetup()
    {
        ObservePlayers(players);
        var playersWithFullUpgrades = new List<Player>() { mainPlayer };
        foreach (var player in playersWithFullUpgrades)
        {
            for (int i = 0; i < 5; i++)
            {
                player.BuildUpgradeWithoutCost(new FreightPodUpgradeToken());
                player.BuildUpgradeWithoutCost(new BoosterUpgradeToken());
                player.BuildUpgradeWithoutCost(new CannonUpgradeToken());
            }
            player.AddHand(Hand.FromResources(5, 5, 5, 5, 5));
        }

        //foreach (var player in players)
        //{
        //    player.AddHand(Hand.FromResources(5, 5, 5, 5, 5));
        //}

        if (players.Count == 2)
        {
            Debug.Log("Player 1 color: " + players[0].color.ConvertToString());
            players[0].BuildTokenWithoutCost(
                mapModel,
                new ColonyBaseToken().GetType(),
                new SpacePoint(5, 5, 1),
                new SpacePortToken().GetType()
            );

            players[0].BuildTokenWithoutCost(
                mapModel,
                new ColonyBaseToken().GetType(),
                new SpacePoint(5, 5, 0),
                new ShipToken().GetType()
            );

            Debug.Log("Player 2 color: " + players[1].color.ConvertToString());
            players[1].BuildTokenWithoutCost(
                mapModel,
                new ColonyBaseToken().GetType(),
                new SpacePoint(3, 10, 0),
                new SpacePortToken().GetType()
            );

            players[1].BuildTokenWithoutCost(
                mapModel,
                new ColonyBaseToken().GetType(),
                new SpacePoint(4, 9, 1),
                new ShipToken().GetType()
            );
        } else
        {
            mainPlayer.BuildTokenWithoutCost(
            mapModel,
            new ColonyBaseToken().GetType(),
            new SpacePoint(5, 5, 1),
            new SpacePortToken().GetType()
        );

            mainPlayer.BuildTokenWithoutCost(
                mapModel,
                new ColonyBaseToken().GetType(),
                new SpacePoint(5, 5, 0),
                new ShipToken().GetType()
            );
        }
    }
  

    

    

    void ObservePlayers(List<Player> playersList)
    {
        foreach (var p in playersList)
        {
            p.RegisterObserver(this);
        }
    }

    public void SetUpDebugState(DebugStartState state)
    {
        debugStartState = state;
        //DebugStartState debugState = new PlayerHasRichHelpPoorBonusDebugState(this);
        //DebugStartState debugState = new ShipBuildingOneColonyShipAndOneSpacePort(this);
        //DebugStartState debugState = new BeatPirateTokenDebugState(this);
        //DebugStartState debugState = new BuildASpacePortDebugState(this);
        debugStartState.Setup();
    }

    public void On7Rolled()
    {
        var strategy = new On7RollStrategy(this);
        strategy.Execute();
    }

    public void PayoutLowPointsBonus(Player player)
    {
        //TODO:
        //var cards = drawPileHandler.availablePiles.DrawCardsFromHiddenDrawPile(2);
        //player.AddHand(Hand.FromResourceCards(cards));
        
    }

    public void SetState(GameState state)
    {
        State = state;
    }

    /// <summary>
    /// Gives player the given payout hand from the open draw piles.
    /// </summary>
    /// <param name="payout"></param>
    /// <param name="player"></param>
    void AddHandToPlayerFromDrawPile(Hand payout, Player player)
    {
        foreach (var card in payout.cards)
        {
            var c = drawPileHandler.availablePiles.DrawCardsOfOpenDrawPile(1, card.GetType());
            player.AddCard(c[0]);
        }
    }

    public void PayoutPlayers(DiceThrow dt)
    {
        foreach (var player in players)
        {
            var payout = payoutHandler.GetPayoutForPlayer(player, dt);

            if (payout.Count() == 0 && player.receivesBonusOnNoPayout)
            {
                //TODO: actionsThatNeedToBeFullfilledBeforeGameContinues.Add(new TakeResourcesAction(1, player));
            } else
            {
                AddHandToPlayerFromDrawPile(payout, player);
            }
        }
    }

    public void RunRPC(string methodName, RpcTarget target, System.Action<RemoteActionCallbackData> rpcCallback, params object[] parameters)
    {
        PhotonView photonView = PhotonView.Get(this);
        this.dispatcherSomeoneFullfilledActionCallback = rpcCallback;
        photonView.RPC(methodName, target, parameters);
        recentRpcCalls.Add(new RpcCall(methodName, target));
    }

    public void RunRPC(string methodName, RpcTarget target, params object[] parameters)
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC(methodName, target, parameters);
        recentRpcCalls.Add(new RpcCall(methodName, target));
    }

    public void RunRPC(string methodName, Photon.Realtime.Player remotePlayer, System.Action<RemoteActionCallbackData> rpcCallback, params object[] parameters)
    {
        PhotonView photonView = PhotonView.Get(this);
        this.dispatcherSomeoneFullfilledActionCallback = rpcCallback;
        photonView.RPC(methodName, remotePlayer, parameters);
    }

    public void RunRPC(string methodName, Photon.Realtime.Player remotePlayer, params object[] parameters)
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC(methodName, remotePlayer, parameters);
    }

    public void LoadGameState(SFGameStateInfo info)
    {
        currentGameStateInfo = info;
        
        var newPlayers = info.players;
        var newMap = info.mapmodel;

        //fix cross references
        //tokensOnMap, token owner and player.tokens need to be same instances again
        foreach (var player in newPlayers)
        {
            player.tokens = new List<Token>();
            foreach (var token in newMap.tokensOnMap)
            {
                if (token.owner.guid == player.guid)
                {
                    player.AddToken(token);
                }
            }
            Debug.Log("Playername: " + player.name + "; VP: " + player.GetVictoryPoints());
        }

        if (info.turnType != null)
        {
            if (IsMyTurn)
            {
                Debug.Log("Its my turn!");
                Debug.Log(this.currentGameStateInfo.playerToActTurnOrderPosition);
                Debug.Log(mainPlayer.TurnOrderPosition);
                Debug.Log(mainPlayer.name);
                if (!turnWasReceived)
                {
                    turnWasReceived = true;
                    var turnType = (TurnType)info.turnType;
                    if (turnType == TurnType.NORMAL)
                    {
                        ActivateNormalPlayStep();
                    }
                    if (turnType == TurnType.SETUP)
                    {
                        ActivateSetupPlayStep();
                    }
                }
            } else
            {
                ActivateAllInteraction(false);
            }
        }
        if (info.players != null && info.mapmodel != null)
        {
            UpdatePlayers(newPlayers);
            UpdateMap(newMap);

            //we need to update at the very end because the hud updates some
            //functionality based on the current map and therefore updates faulty if we change the map afterwards
            GetHUDScript().OnPlayerDataChanged();
            GetMapScript().OnMapDataChanged();

        }

        _state.OnGameDataChanged();
        ObservePlayers(currentGameStateInfo.players);
    }

    void UpdatePlayers(List<Player> newPlayerData)
    {
        numPlayersUpdated += 1;
        foreach (var newData in newPlayerData)
        {
            var player = players.Find(p => p.guid == newData.guid);
            if (player != null)
            {
                player.UpdateData(newData);
            }
        }

        //we lose the observers by serializing the players, therefore we need to observe them again after the update
        //TODO: ObservePlayers(newPlayerData);
    }

    void UpdateMap(Map newMap)
    {
        numMapUpdated += 1;
        Debug.Log("TH2 - Update map!");
        mapModel.UpdateData(newMap);
        mapModel.ReObserveTokens();
    }

    public int GetCurrentPlayerAtTurnIndex()
    {
        return 0;
    }




    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        switch (p_event_path)
        {
            case SFNotification.token_was_selected:
                _state.OnTokenClicked((Token)p_data[0], (GameObject)p_data[1]);
                break;

            case SFNotification.spacepoint_selected:
                var point = (SpacePoint)p_data[0];
                var spacePointObject = (GameObject)p_data[1];
                _state.OnSpacePointClicked(point, spacePointObject);
                break;

            case SFNotification.HUD_build_token_btn_clicked:
                _state.OnBuildShipOptionClicked((Token)p_data[0]);
                break;

            case SFNotification.HUD_build_upgrade_btn_clicked:
                _state.OnBuildUpgradeOptionClicked((Upgrade)p_data[0]);
                break;

            case SFNotification.next_button_clicked:
                _state.OnNextButtonClicked();
                break;

            case SFNotification.settle_button_clicked:
                _state.OnSettleButtonPressed();
                break;

            case SFNotification.token_can_settle:
                _state.OnTokenCanSettle((bool)p_data[0], (Token)p_data[1]);
                break;

        }
    }

    public List<Player> GetPlayers()
    {
        return players;
    }

    public Player GetMainPlayer()
    {
        return mainPlayer;
    }

    public MapScript GetMapScript()
    {
        return Map.GetComponent<MapScript>();
    }

    public HUDScript GetHUDScript()
    {
        return HUD.GetComponent<HUDScript>();
    }


    public void SubjectDataChanged(Subject subject, object[] data)
    {
        if (testMode)
        {
            return;
        }

        if (subject is Map)
        {
            GetMapScript().OnMapDataChanged();
        }

        if (subject is Player)
        {
            GetHUDScript().OnPlayerDataChanged();
        }

        // if we receive multiple updates in a short time (100ms), only react on the last one
        // TODO: probably better to do this inside the subject, so we have the functionality for all the observable stuff
        Debug.Log("GameController: Map or player data changed!");
        try
        {
            StopCoroutine("MakeUpdate");
            Debug.Log("Stopped Make Update coroutine");
        }
        catch
        {
            Debug.Log("Couldnt stop coroutine");
        }
        StartCoroutine("MakeUpdate");
    }

    public IEnumerator MakeUpdate()
    {
        yield return new WaitForSeconds(0.1F);
        Debug.Log("Updateing GameController!");

        SaveGameState();
        yield return null;
    }
}
