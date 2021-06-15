using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using com.onebuckgames.UnityStarFarers;
using System.Linq;

public class SFGameStateInfo
{
    public Map mapmodel;
    public List<Player> players;
    public int playerToActTurnOrderPosition;
    public string state;
    public TurnType? turnType;
    public int turnNumber;
}

public class SFGameClient_ : LoadBalancingClient
{
    public Dictionary<Photon.Realtime.Player, Player> networkPlayersOwnPlayersMap;
    public GameController gameController;
    public PhotonService photonService;
    private bool gameControllerIsInitialized = false;
    private int turnNumber = 0;
    public bool SkipSetupPhase = true;
    public int PlayerToActTurnOrderPosition = 0;
    public TurnType currentTurnType;

    public SFGameClient_()
    {

    }


    int GetNumSetupSteps()
    {
        return 2 * networkPlayersOwnPlayersMap.Count;
    }

    public int GetCurrentPlayerTurnOrderPosition()
    {
        return turnNumber % networkPlayersOwnPlayersMap.Count;
    }

    void PlayNextStep(SFGameStateInfo currentInfo)
    {
        turnNumber++;
        Debug.Log("Turnorder position before" + PlayerToActTurnOrderPosition);
        PlayerToActTurnOrderPosition = GetCurrentPlayerTurnOrderPosition();
        currentInfo.playerToActTurnOrderPosition = PlayerToActTurnOrderPosition;
        Debug.Log("Turnorder position after" + PlayerToActTurnOrderPosition);
        currentInfo.turnNumber = turnNumber;
        if (turnNumber < GetNumSetupSteps() && !SkipSetupPhase)
        {
            currentInfo.turnType = TurnType.SETUP;
        }
        else
        {
            currentInfo.turnType = TurnType.NORMAL;
        }
        SaveGameStateToProps(currentInfo);

    }

    public void HandoverTurnToNextPlayer(SFGameStateInfo currentInfo)
    {
        PlayNextStep(currentInfo);
    }

    public void Start()
    {
        var dict = MapPhotonPlayersToOwnPlayerModels();
        networkPlayersOwnPlayersMap = dict;
        if (PhotonNetwork.IsMasterClient)
        {
            //MasterClient generates map and sends it to all clients, so we dont create different maps
            MapGenerator generator = new DefaultMapGenerator();
            var mapModel = generator.GenerateRandomMap();

            //MasterClient also determines the order of the players and sends this information to all players
            var players = GetAllPlayersFromDict(dict);
            AssignTurnOrderPositionToPlayers(players);

            var startGameState = new SFGameStateInfo();
            startGameState.playerToActTurnOrderPosition = 0;
            startGameState.turnNumber = 0;
            startGameState.turnType = TurnType.NORMAL;
            startGameState.mapmodel = mapModel;
            startGameState.players = players;
            SaveGameStateToProps(startGameState);

        }
    }

    void AssignTurnOrderPositionToPlayers(List<Player> players)
    {
        int idx = 0;
        foreach (var player in players)
        {
            player.TurnOrderPosition = idx;
            idx++;
        }
    }

    Photon.Realtime.Player GetCurrentPlayerAtTurn()
    {
        return SFPlayerToPhotonPlayer(GetMainPlayerFromDict(networkPlayersOwnPlayersMap));
    }

    public int GetCurrentPlayerAtTurnIndex()
    {
        return 0;
        //var globalTurnManager = GameObject.Find("GlobalTurnManager");
        //var globalTurnManagerScript = globalTurnManager.GetComponent<GlobalTurnManager>();
        //return globalTurnManagerScript.GetCurrentPlayerAtTurnIndex();
    }

    public Player GetMainPlayer()
    {
        return GetMainPlayerFromDict(this.networkPlayersOwnPlayersMap);
    }

    public int GetMainPlayerIndex()
    {
        return 0;
        //return GetAllPlayersFromDict(this.networkPlayersOwnPlayersMap).FindIndex(p => p == this.networkPlayersOwnPlayersMap[this.LocalPlayer]);
    }

    Player GetMainPlayerFromDict(Dictionary<Photon.Realtime.Player, Player> dict)
    {
        return dict[PhotonNetwork.LocalPlayer];
    }

    List<Player> GetAllPlayersFromDict(Dictionary<Photon.Realtime.Player, Player> dict)
    {
        return dict.Values.ToList();
    }

    public Player PhotonPlayerToSFPlayer(Photon.Realtime.Player photonPlayer)
    {
        return networkPlayersOwnPlayersMap[photonPlayer];
    }

    public Photon.Realtime.Player SFPlayerToPhotonPlayer(Player sfPlayer)
    {
        foreach (var entry in networkPlayersOwnPlayersMap)
        {
            if (entry.Value.name == sfPlayer.name)
            {
                return entry.Key;
            }
        }
        return null;
    }

    public void SaveGameStateToProps(SFGameStateInfo gameStateInfo)
    {
        photonService.SaveGameStateToProps(gameStateInfo);
    }

    Dictionary<Photon.Realtime.Player, Player> MapPhotonPlayersToOwnPlayerModels()
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
        foreach (var dictEntry in PhotonNetwork.CurrentRoom.Players)
        {
            var networkPlayer = dictEntry.Value;
            var SFPlayer = new Player(colors[idx]);
            SFPlayer.name = networkPlayer.NickName;
            SFPlayer.TurnOrderPosition = idx;
            dict.Add(networkPlayer, SFPlayer);
            idx++;
        }

        return dict;
    }

    public void GameStateInfoChanged(SFGameStateInfo newGameStateInfo)
    {
        if (newGameStateInfo.turnType != null)
        {
            currentTurnType = (TurnType)newGameStateInfo.turnType;
        }

        PlayerToActTurnOrderPosition = newGameStateInfo.playerToActTurnOrderPosition;
        turnNumber = newGameStateInfo.turnNumber;
        Debug.Log("new game state received: turnNumber is" + turnNumber);

        if (newGameStateInfo.players != null && newGameStateInfo.mapmodel != null)
        {
            UpdatePlayers(newGameStateInfo.players);
            //FixCrossReferences
            //foreach (var player in newGameStateInfo.players)
            //{
            //    player.tokens = new List<Token>();
            //    foreach (var token in newGameStateInfo.mapmodel.tokensOnMap)
            //    {
            //        if (token.owner.guid == player.guid)
            //        {
            //            player.AddToken(token);
            //        }
            //    }
            //    Debug.Log("Playername: " + player.name + "; VP: " + player.GetVictoryPoints());
            //}
            if (!gameControllerIsInitialized)
            {
                gameControllerIsInitialized = true;
                gameController.Initialize(newGameStateInfo, GetMainPlayer().name);
            }
        }
        
        if (gameControllerIsInitialized)
        {
            gameController.OnGameStateChangedRemotely(newGameStateInfo);
        }
    }

    void UpdatePlayers(List<Player> newPlayerData)
    {
        foreach (var newData in newPlayerData)
        {
            foreach (var key in networkPlayersOwnPlayersMap.Keys.ToList())
            {
                if (key.NickName == newData.name)
                {
                    networkPlayersOwnPlayersMap[key].UpdateData(newData);
                }
            }
        }
    }

    // void Changed
    // inform gamecontroller that something changed (gamestate)
    // gamecontroller has also reference to this client and receives changes

    //public void SaveCurrentStateToRoomProperties(SFGameState sfGameState) { }

    // pass turn to next player..
    // IsMyTurn...
    // CurrentPlayerAtTurn...
    // GetOpenGames...
    // rejoin game...

    //change room properties
    //receive notification when properties of room change

    //do all the communication with the photon server here, so we could easily mock it
}
