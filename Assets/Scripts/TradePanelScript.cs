using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradePanelScript : MonoBehaviour
{
    public Button MakeTradeWithBankButton;
    public Button MakeTradeWithPlayersButton;
    public GameObject GiveResourceStackRenderer;
    ResourceCardStackRenderer _GiveResourcesStackScript;
    public GameObject ReceiveResourceStackRenderer;
    ResourceCardStackRenderer _ReceiveResourcesStackScript;
    TradingCalculator calculator;
    Player player;

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
        if (calculator != null)
        {
            var inputHand = _GiveResourcesStackScript.GetOutput();
            var outputHand = _ReceiveResourcesStackScript.GetOutput();

            if (calculator.BankTradeIsPossible(inputHand, outputHand))
            {
                MakeTradeWithBankButton.interactable = true;
            }
            else
            {
                MakeTradeWithBankButton.interactable = false;
            }

            if (calculator.PlayerTradeIsPossible(inputHand, outputHand))
            {
                MakeTradeWithPlayersButton.interactable = true;
            }
            else
            {
                MakeTradeWithPlayersButton.interactable = false;
            }
        }
        
    }

    public void OnTradeWithBankButtonClicked()
    {
        player.SubtractHand(_GiveResourcesStackScript.GetOutput());
        player.AddHand(_ReceiveResourcesStackScript.GetOutput());

        _GiveResourcesStackScript.ResetStacks();
        _ReceiveResourcesStackScript.ResetStacks();
    }

    public void OnTradeWithPlayersButtonClicked()
    {
        //TODO: OfferTradeToPlayers(inputHand, outputHand);
    }


}
