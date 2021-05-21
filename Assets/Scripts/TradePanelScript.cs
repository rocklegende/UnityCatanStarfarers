using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using com.onebuckgames.UnityStarFarers;

[System.Serializable]
public class TradeOffer
{
    public readonly Hand giveHand;
    public readonly Hand receiveHand;
    public readonly Player tradeOfferComingFromPlayer;
    public TradeOffer(Hand giveHand, Hand receiveHand, Player tradeOfferComingFromPlayer)
    {
        this.giveHand = giveHand;
        this.receiveHand = receiveHand;
        this.tradeOfferComingFromPlayer = tradeOfferComingFromPlayer;
    }
}

public class TradePanelScript : SFController
{
    public System.Action<Hand, Hand> callback;
    public System.Action<TradeOffer> tradeOfferCallback;
    public Button MakeTradeWithBankButton;
    public Button MakeTradeWithPlayersButton;
    public GameObject GiveResourceStackRenderer;
    ResourceCardStackRenderer _GiveResourcesStackScript;
    public GameObject ReceiveResourceStackRenderer;
    ResourceCardStackRenderer _ReceiveResourcesStackScript;
    TradeOffer currentTradeOffer;
    TradingCalculator calculator;
    Player player;
    public GameObject tradeOfferResponseTableView;

    int exactInput = -1;
    int exactOutput = -1;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Init(Player player)
    {
        _GiveResourcesStackScript = GiveResourceStackRenderer.GetComponent<ResourceCardStackRenderer>();
        _ReceiveResourcesStackScript = ReceiveResourceStackRenderer.GetComponent<ResourceCardStackRenderer>();

        _GiveResourcesStackScript.SetChangedCallback(DataChangedInsideResourcePicker);
        _ReceiveResourcesStackScript.SetChangedCallback(DataChangedInsideResourcePicker);

        SetPlayer(player);
        DrawButtons();
    }

    void SetPlayer(Player player)
    {
        this.player = player;
        calculator = new TradingCalculator(this.player);
        _GiveResourcesStackScript.SetHandLimit(this.player.hand);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Specify if the input hand is only valid for trade if an exact number of cards is reached inside the input hand.
    /// This is useful for the encounter action where you can decide to trade in resources.
    /// </summary>
    public void SetExactInput(int amount)
    {
        exactInput = amount;
    }

    /// <summary>
    /// Specify if the output hand is only valid for trade if an exact number of cards is reached inside the output hand.
    /// This is useful for the encounter action where you can decide to trade in resources.
    /// </summary>
    public void SetExactOutput(int amount)
    {
        exactOutput = amount;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void DataChangedInsideResourcePicker(Hand newHand)
    {
        DrawButtons();
    }

    void DrawButtons()
    {
        var inputHand = _GiveResourcesStackScript.GetDisplayedHand();
        var outputHand = _ReceiveResourcesStackScript.GetDisplayedHand();
        if (exactInput != -1 && exactOutput != -1)
        {
            MakeTradeWithBankButton.interactable = inputHand.Count() == exactInput && outputHand.Count() == exactOutput;
            MakeTradeWithPlayersButton.interactable = false; // Player Trade should not be allowed in this mode;
        } else
        {
            if (calculator != null)
            {
                MakeTradeWithBankButton.interactable = calculator.BankTradeIsPossible(inputHand, outputHand);
                MakeTradeWithPlayersButton.interactable = calculator.PlayerTradeIsPossible(inputHand, outputHand);
            }
        }
    }

    public void OnTradeWithBankButtonClicked()
    {
        var inputHand = _GiveResourcesStackScript.GetDisplayedHand();
        var outputHand = _ReceiveResourcesStackScript.GetDisplayedHand();

        player.SubtractHand(inputHand);
        player.AddHand(outputHand);

        callback(inputHand, outputHand);

        _GiveResourcesStackScript.ResetStacks();
        _ReceiveResourcesStackScript.ResetStacks();
    }

    public void OnTradeWithPlayersButtonClicked()
    {
        var inputHand = _GiveResourcesStackScript.GetDisplayedHand();
        var outputHand = _ReceiveResourcesStackScript.GetDisplayedHand();

        
        //var sendToPlayers = globalGamecontroller.players; //For dev: 
        var sendToPlayers = globalGamecontroller.players.Where(player => player != globalGamecontroller.mainPlayer).ToList(); //For prod: 

        var tradeOffer = new TradeOffer(inputHand, outputHand, player);
        currentTradeOffer = tradeOffer;
        var action = new RemoteClientAction(RemoteClientActionType.TRADE_OFFER, new object[] { tradeOffer }, globalGamecontroller.mainPlayer);

        tradeOfferResponseTableView.GetComponent<TradeOfferResponseTableView>().callback = AcceptAcceptedTradeOffer;
        foreach (var sendToPlayer in sendToPlayers)
        {
            var dispatcher = new RemoteActionDispatcher(globalGamecontroller);
            dispatcher.RequestActionFromPlayers(new List<Player>() { sendToPlayer }, action, PlayerRespondedToTradeOffer);
        }
        
    }

    void PlayerRespondedToTradeOffer(Dictionary<string, RemoteActionCallbackData> responses)
    {
        var playerWhoResponded = responses.Values.ToList().First().player;
        Debug.Log(string.Format("Player responded to trade offer: ", playerWhoResponded.name));
        if ((bool)responses.Values.ToList().First().data == true)
        {
            tradeOfferResponseTableView.GetComponent<TradeOfferResponseTableView>().AddRow(responses.Values.ToList().First().player);
        }
    }


    void AcceptAcceptedTradeOffer(Player playerWhoAcceptedTrade)
    {
        var _playerToTradeWith = globalGamecontroller.players.Find(p => p.name == playerWhoAcceptedTrade.name);
        Debug.Log(string.Format("Accepted trade from {0}, making trade..", _playerToTradeWith.name));
        MakeTradeWithPlayer(currentTradeOffer, player, _playerToTradeWith);
        globalGamecontroller.RunRPC("RemoteClientTradeOfferFinished", Photon.Pun.RpcTarget.Others);
    }

    public static void MakeTradeWithPlayer(TradeOffer tradeOffer, Player originOfTrade, Player playerToTradeWith)
    {
        originOfTrade.PayToOtherPlayer(playerToTradeWith, tradeOffer.giveHand);
        playerToTradeWith.PayToOtherPlayer(originOfTrade, tradeOffer.receiveHand);
    }

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        //
    }
}
