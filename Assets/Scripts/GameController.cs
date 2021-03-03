using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : SFController
{
    public GameObject HUD;
    public GameObject Map;
    public Map mapModel;

    public GameState state;
    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        var state = new OneTradeOneColonyShipAndOneSpacePort(this);
        state.Setup();
    }

    public void SetState(GameState state)
    {
        this.state = state;
    }

    Hand GetBasicPayoutForPlayer(Player player, DiceThrow dt) {
        Hand payout = new Hand();
        foreach (Token token in player.tokens)
        {
            Tile_[] tiles = mapModel.getTilesAtPoint(token.position);
            ResourceTile[] resourceTiles = mapModel.GetTilesOfType<ResourceTile>(tiles);
            foreach (ResourceTile resourceTile in resourceTiles)
            {
                DiceChip diceChip = resourceTile.diceChip;
                if (diceChip.isFaceUp && diceChip.fulfillsDiceThrow(dt.GetValue()))
                {
                    var helper = new Helper();
                    var numResources = token.GetResourceProductionMultiplier();
                    for (int i = 0; i < numResources; i++)
                    {
                        ResourceCard resourceCard = helper.CreateResourceCardFromResource(resourceTile.resource);
                        //player.AddCard(resourceCard);
                        payout.AddCard(resourceCard);
                    }
                }
            }
        }
        return payout;
    }

    Hand GetBonusPayoutForPlayer(Hand payoutThatRound, Player player)
    {
        //TODO: needs goooood refactoring, please dont code this ugly alright?

        Hand bonusPayout = new Hand();

        var carbonBonus = player.GetBonusForResource(new CarbonResource());
        if (carbonBonus != 0)
        {
            if (payoutThatRound.FindCardOfResource<CarbonResource>() != null)
            {
                var h = new Helper().GetHandWithResources(0, 0, 0, 0, carbonBonus);
                bonusPayout.AddHand(h);
            }
        }

        var fuelBonus = player.GetBonusForResource(new FuelResource());
        if (fuelBonus != 0)
        {
            if (payoutThatRound.FindCardOfResource<FuelResource>() != null)
            {
                var h = new Helper().GetHandWithResources(0, 0, fuelBonus);
                bonusPayout.AddHand(h);
            }
        }

        var foodBonus = player.GetBonusForResource(new FoodResource());
        if (foodBonus != 0)
        {
            if (payoutThatRound.FindCardOfResource<FoodResource>() != null)
            {
                var h = new Helper().GetHandWithResources(foodBonus);
                bonusPayout.AddHand(h);
            }
        }

        var oreBonus = player.GetBonusForResource(new OreResource());
        if (oreBonus != 0)
        {
            if (payoutThatRound.FindCardOfResource<OreResource>() != null)
            {
                var h = new Helper().GetHandWithResources(0, 0, 0, oreBonus);
                bonusPayout.AddHand(h);
            }
        }

        var goodsBonus = player.GetBonusForResource(new GoodsResource());
        if (goodsBonus != 0)
        {
            if (payoutThatRound.FindCardOfResource<GoodsResource>() != null)
            {
                var h = new Helper().GetHandWithResources(0, goodsBonus);
                bonusPayout.AddHand(h);
            }
        }

        return bonusPayout;
    }

    void AddPayoutToPlayer(Hand payout, Player player)
    {
        foreach (var card in payout.cards)
        {
            player.AddCard(card);
        }
    }

    // TODO: move to class specifically designed to handle payout calculations
    void PayoutPlayers(DiceThrow dt) 
    {
        var completePayout = new Hand();
        var basic = GetBasicPayoutForPlayer(player, dt);
        var bonus = GetBonusPayoutForPlayer(basic, player);

        completePayout.AddHand(bonus);
        completePayout.AddHand(basic);

        AddPayoutToPlayer(completePayout, player);
    }

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        switch (p_event_path)
        {
            case SFNotification.token_was_selected:
                state.OnTokenClicked((Token)p_data[0], (GameObject)p_data[1]);
                break;
            case SFNotification.spacepoint_selected:
                var tiles = mapModel.getTilesAtPoint((SpacePoint)p_data[0]);
                state.OnSpacePointClicked((SpacePoint)p_data[0], (GameObject)p_data[1]);
                break;

            case SFNotification.HUD_build_token_btn_clicked:
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

            case SFNotification.dice_thrown:
                //PayoutPlayers((DiceThrow)p_data[0]);
                PayoutPlayers(new DiceThrow(2, 1));
                break;

            case SFNotification.token_data_changed:
                if (mapModel != null)
                {
                    mapModel.OnTokenDataChanged((Token)p_data[0]);
                }
                break;

        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
