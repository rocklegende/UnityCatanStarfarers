using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeOfferView : SFController
{
    public System.Action<bool> callback;
    public Button AcceptButton;
    public Button DeclineButton;
    public GameObject GiveResourceStackRenderer;
    public TradeOffer tradeOffer;
    ResourceCardStackRenderer _GiveResourcesStackScript;
    public GameObject ReceiveResourceStackRenderer;
    ResourceCardStackRenderer _ReceiveResourcesStackScript;
        

    // Start is called before the first frame update
    void Start()
    {
        _GiveResourcesStackScript = GiveResourceStackRenderer.GetComponent<ResourceCardStackRenderer>();
        _ReceiveResourcesStackScript = ReceiveResourceStackRenderer.GetComponent<ResourceCardStackRenderer>();
    }

    public void Init(TradeOffer tradeOffer, System.Action<bool> callback)
    {
        this.tradeOffer = tradeOffer;
        this.callback = callback;

        GiveResourceStackRenderer.GetComponent<ResourceCardStackRenderer>().SetHand(tradeOffer.receiveHand); //is flipped on purpose because the trade offer is always from the perspective of the trade offerer
        ReceiveResourceStackRenderer.GetComponent<ResourceCardStackRenderer>().SetHand(tradeOffer.giveHand);
    }

    public void Draw()
    {
        GiveResourceStackRenderer.GetComponent<ResourceCardStackRenderer>().Draw();
        ReceiveResourceStackRenderer.GetComponent<ResourceCardStackRenderer>().Draw();
    }


    public void AcceptButtonPressed()
    {
        callback(true);
    }

    public void DeclineButtonPressed()
    {
        callback(false);
    }

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        //throw new System.NotImplementedException();
    }
}
