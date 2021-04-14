﻿using System.Linq;
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
    int GetCurrentPlayerAtTurn();
    MapScript GetMapScript();
    HUDScript GetHUDScript();
}


public class GameController : SFController, IGameController, Observer
{
    public GameObject HUD;
    public GameObject Map;
    public Map mapModel;

    public GameState state;
    public List<Player> players;
    public Player mainPlayer;
    public int currentPlayerAtTurn = 0;
    public PayoutHandler payoutHandler;

    private bool SetupCompleted = false;

    public EncounterCardHandler encounterCardHandler;
    public DrawPileHandler drawPileHandler;
    public DebugStartState debugStartState;

    // Start is called before the first frame update
    void Start()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(this);

            MapGenerator generator = new DefaultMapGenerator();
            mapModel = generator.GenerateRandomMap(); //MasterClient generates map and sends it to all clients, so we dont create different maps

            var mapAsBytes = SFFormatter.Serialize(mapModel);
            photonView.RPC("MapWasGenerated", RpcTarget.All, mapAsBytes);
        }



        //SetupButtonPressed();

        //MapGenerator generator2 = new DefaultMapGenerator();
        //mapModel = generator2.GenerateRandomMap();
        //MapWasGenerated(SFFormatter.Serialize(mapModel));

    }

    List<Player> CreatePlayers()
    {
        var dict = MapPhotonPlayersToOwnPlayerModelDict();
        var players = dict.Values.ToList();
        return players;
    }

    

    Dictionary<Photon.Realtime.Player, Player> MapPhotonPlayersToOwnPlayerModelDict()
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
            dict.Add(networkPlayer, SFPlayer);
            idx++;
        }

        return dict;
    }



    [PunRPC]
    void MapWasGenerated(byte[] mapAsBytes)
    {
        mapModel = (Map)SFFormatter.Deserialize(mapAsBytes);
        mapModel.RegisterObserver(this);
        this.state = new StartState(this);

        //every client sets up the players itself, no need to send that over wire from MasterClient
        players = CreatePlayers();
        var playersDict = MapPhotonPlayersToOwnPlayerModelDict();
        //players = new Player[] { new Player(new SFColor(Color.white)), new Player(new SFColor(Color.red)) };

        mainPlayer = playersDict[PhotonNetwork.LocalPlayer];
        Debug.Log("PlayerNameBla");
        Debug.Log(mainPlayer.name);
        Debug.Log("PlayerNameBla");
        //mainPlayer = players[0];


        HUD.GetComponent<HUDScript>().SetPlayers(players, mainPlayer);
        HUD.GetComponent<HUDScript>().isReceivingNotifications = true;

        Map.GetComponent<MapScript>().SetMap(mapModel);
        Map.GetComponent<MapScript>().SetPlayers(players);
        Map.GetComponent<MapScript>().isReceivingNotifications = true;

        payoutHandler = new PayoutHandler(mapModel);
    }

    public void SetupButtonPressed()
    {
        mainPlayer.AddHand(Hand.FromResources(5, 5, 5, 5, 5));

        mainPlayer.BuildTokenWithoutCost(
            mapModel,
            new ColonyBaseToken().GetType(),
            new SpacePoint(5, 5, 1),
            new SpacePortToken().GetType()
        );
        //SetUpDebugState(new EncounterCardTestingStateManual(this));
    }

    public void SetUpDebugState(DebugStartState state)
    {
        debugStartState = state;
        //DebugStartState debugState = new PlayerHasRichHelpPoorBonusDebugState(this);
        //DebugStartState debugState = new ShipBuildingOneColonyShipAndOneSpacePort(this);
        //DebugStartState debugState = new BeatPirateTokenDebugState(this);
        //DebugStartState debugState = new BuildASpacePortDebugState(this);
        debugStartState.Setup();
        SetupCompleted = true;
    }

    public void On7Rolled()
    {
        var strategy = new On7RollStrategy();
        strategy.Execute(players, currentPlayerAtTurn, drawPileHandler.availablePiles);
    }

    public void PassTurnToNextPlayer()
    {
        currentPlayerAtTurn += 1;
        if (currentPlayerAtTurn == players.Count)
        {
            currentPlayerAtTurn = 0;
        }
        PayoutLowPointsBonus(players[currentPlayerAtTurn]);
        players[currentPlayerAtTurn].OnTurnReceived();
    }

    public void PayoutLowPointsBonus(Player player)
    {
        var cards = drawPileHandler.availablePiles.DrawCardsFromHiddenDrawPile(2);
        foreach (var c in cards)
        {
            player.AddCard(c);
        }
    }

    public void SetState(GameState state)
    {
        this.state = state;
    }

    void AddPayoutToPlayer(Hand payout, Player player)
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
                AddPayoutToPlayer(payout, player);
            }
        }
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
                point.print();
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

            //case SFNotification.token_data_changed:
            //    if (mapModel != null)
            //    {
            //        mapModel.OnTokenDataChanged((Token)p_data[0]);
            //    }
            //    break;

            case SFNotification.token_can_settle:
                state.OnTokenCanSettle((bool)p_data[0], (Token)p_data[1]);
                break;

        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Player> GetPlayers()
    {
        return players;
    }

    public Player GetMainPlayer()
    {
        return mainPlayer;
    }

    public int GetCurrentPlayerAtTurn()
    {
        return currentPlayerAtTurn;
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
        Debug.Log("Map Data Changed");
        RedrawMapOnAllClients();
    }
}
