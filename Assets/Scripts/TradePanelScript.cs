using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradePanelScript : MonoBehaviour
{
    public System.Action<Hand, Hand> callback;
    public Button MakeTradeWithBankButton;
    public Button MakeTradeWithPlayersButton;
    public GameObject GiveResourceStackRenderer;
    ResourceCardStackRenderer _GiveResourcesStackScript;
    public GameObject ReceiveResourceStackRenderer;
    ResourceCardStackRenderer _ReceiveResourcesStackScript;
    TradingCalculator calculator;
    Player player;

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
        _GiveResourcesStackScript.SetPlayer(this.player);
    }

    /// <summary>
    /// Specify if the input hand is only valid for trade if an exact number of cards is reached inside the input hand.
    /// </summary>
    public void SetExactInput(int amount)
    {
        exactInput = amount;
    }

    /// <summary>
    /// Specify if the output hand is only valid for trade if an exact number of cards is reached inside the output hand.
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
        var inputHand = _GiveResourcesStackScript.GetOutput();
        var outputHand = _ReceiveResourcesStackScript.GetOutput();
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
        var inputHand = _GiveResourcesStackScript.GetOutput();
        var outputHand = _ReceiveResourcesStackScript.GetOutput();

        player.SubtractHand(inputHand);
        player.AddHand(outputHand);

        callback(inputHand, outputHand);

        _GiveResourcesStackScript.ResetStacks();
        _ReceiveResourcesStackScript.ResetStacks();
    }

    public void OnTradeWithPlayersButtonClicked()
    {
        //TODO: OfferTradeToPlayers(inputHand, outputHand);
    }


}
