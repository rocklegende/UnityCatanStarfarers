using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeOfferView : SFController
{
    public System.Action<bool> callback;
    public Button AcceptButton;
    public Button DeclineButton;
    public GameObject GiveResourceCostRenderer;
    public TradeOffer tradeOffer;
    CostRenderer _GiveResourcesCostRendererScript;
    public GameObject ReceiveResourceCostRenderer;
    CostRenderer _ReceiveResourcesCostRendererScript;
        

    // Start is called before the first frame update
    void Start()
    {
        _GiveResourcesCostRendererScript = GiveResourceCostRenderer.GetComponent<CostRenderer>();
        _ReceiveResourcesCostRendererScript = ReceiveResourceCostRenderer.GetComponent<CostRenderer>();
    }

    public Hand GetGiveHand()
    {
        return GiveResourceCostRenderer.GetComponent<CostRenderer>().cost.hand;
    }

    public Hand GetReceiveHand()
    {
        return ReceiveResourceCostRenderer.GetComponent<CostRenderer>().cost.hand;
    }
        
    public void Init(TradeOffer tradeOffer, System.Action<bool> callback)
    {
        this.tradeOffer = tradeOffer;
        this.callback = callback;

        GiveResourceCostRenderer.GetComponent<CostRenderer>().SetCost(new Cost(tradeOffer.receiveHand)); //is flipped on purpose because the trade offer is always from the perspective of the trade offerer
        ReceiveResourceCostRenderer.GetComponent<CostRenderer>().SetCost(new Cost(tradeOffer.giveHand));
    }

    public void Draw()
    {
        GiveResourceCostRenderer.GetComponent<CostRenderer>().Draw();
        ReceiveResourceCostRenderer.GetComponent<CostRenderer>().Draw();
    }


    public void AcceptButtonPressed()
    {
        callback?.Invoke(true);
    }

    public void DeclineButtonPressed()
    {
        gameObject.SetActive(false);
        callback?.Invoke(false);
    }

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        //throw new System.NotImplementedException();
    }
}
