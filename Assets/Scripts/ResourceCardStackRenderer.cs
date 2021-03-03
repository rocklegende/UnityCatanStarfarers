using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceCardStackRenderer : SFController
{
    Player player;
    public Text oreCardStackText;
    public Text carbonCardStackText;
    public Text foodCardStackText;
    public Text fuelCardStackText;
    public Text goodsCardStackText;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    void DrawResourceStacks()
    {
        if (player != null)
        {
            Hand hand = player.hand;
            carbonCardStackText.text = hand.NumberCardsOfType<CarbonCard>().ToString();
            goodsCardStackText.text = hand.NumberCardsOfType<GoodsCard>().ToString();
            fuelCardStackText.text = hand.NumberCardsOfType<FuelCard>().ToString();
            foodCardStackText.text = hand.NumberCardsOfType<FoodCard>().ToString();
            oreCardStackText.text = hand.NumberCardsOfType<OreCard>().ToString();
        }
    }

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        switch (p_event_path)
        {
            case SFNotification.player_data_changed:
                DrawResourceStacks();
                break;
        }
    }
}
