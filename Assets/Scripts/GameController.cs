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

public enum RemoteClientActionType
{
    GIVE_RESOURCE,
    GIVEUP_UPGRADE,
    SEVEN_ROLL_DISCARD,
    TRADE_OFFER
}

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
    public class RemoteClientAction
    {
        public readonly RemoteClientActionType actionType;
        public readonly object[] parameters;
        public readonly int sendFromPlayerIndex;

        public RemoteClientAction(RemoteClientActionType actionType, object[] parameters, int sendFromPlayerIndex)
        {
            this.actionType = actionType;
            this.parameters = parameters;
            this.sendFromPlayerIndex = sendFromPlayerIndex;
        }
    }
}

[System.Serializable]
public class GameStartInformation
{
    public Map map { get; }
    /// <summary>
    /// Key: player name, Value: internal turn order position
    /// </summary>
    public Dictionary<string, int> turnOrder { get; }
    public GameStartInformation(Map map, Dictionary<string, int> turnOrder)
    {
        this.map = map;
        this.turnOrder = turnOrder;
    }
}


public class GameController : SFController, IGameController, Observer
{
    public GameObject HUD;
    public GameObject Map;
    public Map mapModel;

    public GameState state;
    public List<Player> players;
    public Dictionary<Photon.Realtime.Player, Player> networkPlayersOwnPlayersMap;
    public Player mainPlayer;
    public int currentPlayerAtTurnIndex = 0;
    public PayoutHandler payoutHandler;

    public EncounterCardHandler encounterCardHandler;
    public DrawPileHandler drawPileHandler;
    public DebugStartState debugStartState;
    public Photon.Realtime.Player recentPhotonResponsePlayer;

    /// <summary>
    /// Run GameController in testMode
    /// </summary>
    private bool testMode = false;
    private System.Action<RemoteActionCallbackData> dispatcherSomeoneFullfilledActionCallback;
    private System.Timers.Timer updateTimer;

    void Start()
    {
        encounterCardHandler.encounterCardStack = new DefaultEncounterCardStack(this);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(this);

            //MasterClient generates map and sends it to all clients, so we dont create different maps
            //MasterClient also determines the order of the players and sends this information to all players
            MapGenerator generator = new DefaultMapGenerator();
            mapModel = generator.GenerateRandomMap();

            var turnOrder = GenerateRandomTurnOrder();
            var gameStartInformation = new GameStartInformation(mapModel, turnOrder);

            var gameStartInformationAsBytes = SFFormatter.Serialize(gameStartInformation);
            photonView.RPC("GameStartInformationGenerated", RpcTarget.All, gameStartInformationAsBytes);
        }
    }

    Dictionary<string, int> GenerateRandomTurnOrder()
    {
        var turnOrder = new Dictionary<string, int>();
        var numPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        var order = new List<int>();
        for (int i = 0; i < numPlayers; i++)
        {
            order.Add(i);
        }
        order.Shuffle();

        for (int i = 0; i < numPlayers; i++)
        {
            turnOrder[PhotonNetwork.CurrentRoom.Players.Values.ToList()[i].NickName] = order[i];
        }
        return turnOrder;
    }

    [PunRPC]
    void GameStartInformationGenerated(byte[] gameStartInformationAsBytes)
    {
        var gameStartInformation = (GameStartInformation)SFFormatter.Deserialize(gameStartInformationAsBytes);

        mapModel = gameStartInformation.map;
        mapModel.RegisterObserver(this);
        this.state = new StartState(this);

        //every client sets up the players itself, no need to send that over wire from MasterClient
        var dict = CreatePlayerMap(gameStartInformation.turnOrder);
        networkPlayersOwnPlayersMap = dict;
        players = GetAllPlayersFromDict();
        mainPlayer = GetMainPlayerFromDict();

        Init();
    }

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

        mapModel = map;
        mapModel.RegisterObserver(this);
        this.state = new StartState(this);

        this.players = players;
        this.mainPlayer = mainPlayer;

        Init();
    }

    public void Init()
    {
        ObservePlayers(players);

        HUD.GetComponent<HUDScript>().SetPlayers(players, mainPlayer);
        HUD.GetComponent<HUDScript>().isReceivingNotifications = true;

        Map.GetComponent<MapScript>().SetMap(mapModel);
        Map.GetComponent<MapScript>().isReceivingNotifications = true;

        payoutHandler = new PayoutHandler(mapModel);
        OnTurnChanged();
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

    [PunRPC]
    public void RemoteClientRequiresAction(byte[] actionInfo)
    {
        var remoteAction = (RemoteClientAction)SFFormatter.Deserialize(actionInfo);
        switch (remoteAction.actionType)
        {
            case RemoteClientActionType.GIVE_RESOURCE:
                HandleGiveResourceActionFromRemoteClient(remoteAction);
                break;
            case RemoteClientActionType.GIVEUP_UPGRADE:
                HandleGiveupUpgradeActionFromRemoteClient(remoteAction);
                break;
            case RemoteClientActionType.SEVEN_ROLL_DISCARD:
                HandleSevenRollDiscardActionFromRemoteClient(remoteAction);
                break;
            case RemoteClientActionType.TRADE_OFFER:
                HandleTradeOfferActionFromRemoteClient(remoteAction);
                break;
        }
    }

    void HandleTradeOfferActionFromRemoteClient(RemoteClientAction remoteAction)
    {
        var tradeOffer = (TradeOffer)remoteAction.parameters[0];
        GetHUDScript().DisplayTradeOffer(tradeOffer, (accept) =>
        {
            SendResponseBackToCallerOfRemoteClientAction(remoteAction, new RemoteActionCallbackData(mainPlayer, accept));
        });
    }

    void HandleGiveResourceActionFromRemoteClient(RemoteClientAction remoteAction)
    {
        var requiredResources = (int)remoteAction.parameters[0];
        GetHUDScript().OpenDiscardResourcePicker((hand) => {
            Debug.Log("hand picked, count: " + hand.Count());

            var sfTargetPlayer = players[remoteAction.sendFromPlayerIndex];
            mainPlayer.PayToOtherPlayer(sfTargetPlayer, hand);
            Debug.Log("paying hand to player: " + sfTargetPlayer.name);

            SendResponseBackToCallerOfRemoteClientAction(remoteAction);
        }, requiredResources, requiredResources);
    }

    void HandleGiveupUpgradeActionFromRemoteClient(RemoteClientAction remoteAction)
    {
        var upgradesToDiscard = (int)remoteAction.parameters[0];

        var encounterAction = new RemoveUpgradeAction(this);
        encounterAction.callback = ((value) => SendResponseBackToCallerOfRemoteClientAction(remoteAction));
        encounterAction.Execute();
    }

    void HandleSevenRollDiscardActionFromRemoteClient(RemoteClientAction remoteAction)
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
        var sfTargetPlayer = players[remoteAction.sendFromPlayerIndex];
        var photonTargetPlayer = SFPlayerToPhotonPlayer(sfTargetPlayer);
        recentPhotonResponsePlayer = photonTargetPlayer;
        Debug.Log("Sending response to player: " + photonTargetPlayer.NickName);
        RunRPC("RemoteClientFulfilledAction", photonTargetPlayer, SFFormatter.Serialize(new RemoteActionCallbackData(mainPlayer)));
    }

    void SendResponseBackToCallerOfRemoteClientAction(RemoteClientAction remoteAction, RemoteActionCallbackData data)
    {
        var sfTargetPlayer = players[remoteAction.sendFromPlayerIndex];
        var photonTargetPlayer = SFPlayerToPhotonPlayer(sfTargetPlayer);
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

    public Player PhotonPlayerToSFPlayer(Photon.Realtime.Player photonPlayer)
    {
        return networkPlayersOwnPlayersMap[photonPlayer];
    }

    public Photon.Realtime.Player SFPlayerToPhotonPlayer(Player sfPlayer)
    {
        foreach(var entry in networkPlayersOwnPlayersMap)
        {
            if (entry.Value.name == sfPlayer.name)
            {
                return entry.Key;
            }
        }
        return null;
    }

    [PunRPC]
    void RemoteClientFulfilledAction(byte[] data)
    {
        var remoteCallbackData = (RemoteActionCallbackData) SFFormatter.Deserialize(data);
        var playerWhoFullfilledAction = remoteCallbackData.player;
        Debug.Log("Response from " + playerWhoFullfilledAction.name + " received");
        dispatcherSomeoneFullfilledActionCallback(remoteCallbackData);
    }

    void InitialPlayerSetup()
    {
        //foreach(var player in players)
        //{
        //    for (int i = 0; i < 5; i++)
        //    {
        //        player.BuildUpgradeWithoutCost(new FreightPodUpgradeToken());
        //        player.BuildUpgradeWithoutCost(new BoosterUpgradeToken());
        //        player.BuildUpgradeWithoutCost(new CannonUpgradeToken());
        //    }
        //}

        foreach(var player in players)
        {
            player.AddHand(Hand.FromResources(5, 5, 5, 5, 5));
        }

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

    Dictionary<Photon.Realtime.Player, Player> CreatePlayerMap(Dictionary<string, int> turnOrder)
    {
        var dict = MapPhotonPlayersToOwnPlayerModelDict(turnOrder);
        return dict;
    }

    public int GetIndexOfPlayer(Player searchedPlayer)
    {
        var allPlayers = GetAllPlayersFromDict();
        return allPlayers.FindIndex(p => p.name == searchedPlayer.name);
    }

    Player GetMainPlayerFromDict()
    {
        return networkPlayersOwnPlayersMap[PhotonNetwork.LocalPlayer];
    }

    List<Player> GetAllPlayersFromDict()
    {
        return networkPlayersOwnPlayersMap.Values.ToList();
    }

    public Player GetCurrentPlayerAtTurn()
    {
        return players.Find(p => p.TurnOrderPosition == currentPlayerAtTurnIndex);
    }

    public bool IsMyTurn()
    {
        return GetCurrentPlayerAtTurn().name == mainPlayer.name;
    }

    Dictionary<Photon.Realtime.Player, Player> MapPhotonPlayersToOwnPlayerModelDict(Dictionary<string, int> turnOrder)
    {
        var dict = new Dictionary<Photon.Realtime.Player, Player>();

        var colors = new SFColor[]
        {
            new SFColor(Color.white),
            new SFColor(Color.green),
            new SFColor(Color.blue),
            new SFColor(Color.grey),
        };

        int idx = 0;
        foreach(var dictEntry in PhotonNetwork.CurrentRoom.Players)
        {
            var networkPlayer = dictEntry.Value;
            var SFPlayer = new Player(colors[idx]);
            SFPlayer.name = networkPlayer.NickName;
            SFPlayer.TurnOrderPosition = turnOrder[dictEntry.Value.NickName];
            dict.Add(networkPlayer, SFPlayer);
            idx++;
        }

        return dict;
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

    public void PassTurnToNextPlayer()
    {
        currentPlayerAtTurnIndex += 1;
        if (currentPlayerAtTurnIndex == players.Count)
        {
            currentPlayerAtTurnIndex = 0;
        }
        RunRPC("CurrentPlayerAtTurnChanged", RpcTarget.All, currentPlayerAtTurnIndex);
    }

    void OnTurnChanged()
    {
        Debug.Log("currentPlayerAtTurnIndex" + currentPlayerAtTurnIndex);
        Debug.Log("Current player: " + GetCurrentPlayerAtTurn().name);
        Debug.Log("Is my turn: " + IsMyTurn());

        if (IsMyTurn())
        {
            PayoutLowPointsBonus(mainPlayer);
            mainPlayer.OnTurnReceived();
            GetHUDScript().ActivateAllInteraction(true);
            SetState(new StartState(this));
        } else
        {
            GetHUDScript().ActivateAllInteraction(false);
        }
    }

    [PunRPC]
    void CurrentPlayerAtTurnChanged(int newPlayerAtTurnIndex)
    {
        currentPlayerAtTurnIndex = newPlayerAtTurnIndex;
        OnTurnChanged();
    }

    public void PayoutLowPointsBonus(Player player)
    {
        var cards = drawPileHandler.availablePiles.DrawCardsFromHiddenDrawPile(2);
        player.AddHand(Hand.FromResourceCards(cards));
        
    }

    public void SetState(GameState state)
    {
        this.state = state;
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
    }

    public void RunRPC(string methodName, RpcTarget target, params object[] parameters)
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC(methodName, target, parameters);
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

    public void RedrawMapOnAllClients()
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("RedrawMap", RpcTarget.Others, SFFormatter.Serialize(mapModel));
    }

    [PunRPC]
    public void RedrawMap(byte[] mapAsBytes)
    {
        var mapScript = GetMapScript();
        mapScript.map = (Map) SFFormatter.Deserialize(mapAsBytes);
        mapScript.RedrawMap();
    }

    [PunRPC]
    void PlayerInfoChangedOnRemoteClient(byte[] newPlayerDataAsBytes)
    {

        var playerData = (List<Player>)SFFormatter.Deserialize(newPlayerDataAsBytes);

        Debug.Log("Remote client changed player data!");
        foreach (var player in playerData)
        {
            Debug.Log(string.Format("name: {0}, cards: {1}", player.name, player.hand.Count()));
        }

        UpdatePlayers(playerData);
    }

    void OnLocalPlayerDataChanged()
    {
        Debug.Log("Local player data changed!");
        foreach (var player in players)
        {
            Debug.Log(string.Format("name: {0}, cards: {1}", player.name, player.hand.Count()));
        }

        UpdatePlayers(players);

        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("PlayerInfoChangedOnRemoteClient", RpcTarget.Others, SFFormatter.Serialize(players));
        
            
    }

    void UpdatePlayers(List<Player> newPlayerData)
    {

        var newDict = new Dictionary<Photon.Realtime.Player, Player>();
        foreach (var newData in newPlayerData)
        {
            foreach (var key in networkPlayersOwnPlayersMap.Keys.ToList())
            {
                if (key.NickName == newData.name)
                {
                    newDict[key] = newData;
                }
            }
        }
        networkPlayersOwnPlayersMap = newDict;
        players = GetAllPlayersFromDict();
        mainPlayer = GetMainPlayerFromDict();

        //we lose the observers by serializing the players, therefore we need to observe them again after the update
        ObservePlayers(newPlayerData);
        HUD.GetComponent<HUDScript>().UpdatePlayers(players, mainPlayer);
    }

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        switch (p_event_path)
        {
            case SFNotification.token_was_selected:

                state.OnTokenClicked((Token)p_data[0], (GameObject)p_data[1]);
                break;
            case SFNotification.spacepoint_selected:
                var point = (SpacePoint)p_data[0];
                var spacePointObject = (GameObject)p_data[1];
                state.OnSpacePointClicked(point, spacePointObject);
                break;

            case SFNotification.map_data_changed:
                RedrawMapOnAllClients();
                break;

            case SFNotification.HUD_build_token_btn_clicked:
                var tok = (Token)p_data[0];
                Debug.Log(tok);
                state.OnBuildShipOptionClicked((Token)p_data[0]);
                break;

            case SFNotification.HUD_build_upgrade_btn_clicked:
                state.OnBuildUpgradeOptionClicked((Token)p_data[0]);
                break;

            case SFNotification.next_button_clicked:
                state.OnNextButtonClicked();
                break;

            case SFNotification.settle_button_clicked:
                state.OnSettleButtonPressed();
                break;

            case SFNotification.token_can_settle:
                state.OnTokenCanSettle((bool)p_data[0], (Token)p_data[1]);
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

    

    public void SubjectDataChanged(object[] data)
    {
        if (testMode)
        {
            return;
        }
        // if we receive multiple updates in a short time (100ms), only react on the last one
        // TODO: probably better to do this inside the subject, so we have the functionality for all the observable stuff
        Debug.Log("Map data changed");
        try
        {
            StopCoroutine("MakeUpdate");
            Debug.Log("Stopped Make Update coroutine");
        } catch
        {
            Debug.Log("Couldnt stop coroutine");
        }
        StartCoroutine("MakeUpdate");
    }

    public IEnumerator MakeUpdate()
    {
        yield return new WaitForSeconds(0.5F);
        Debug.Log("Updateing GameController!");
        RedrawMapOnAllClients();
        OnLocalPlayerDataChanged();
    }
}
