using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDScript : SFController
{
    public Player player;
    public Text oreCardStackText;
    public Text carbonCardStackText;
    public Text foodCardStackText;
    public Text fuelCardStackText;
    public Text goodsCardStackText;
    public Text boosterText;
    public Text cannonsText;
    public Text freightPodsText;

    public GameObject buildShipsDropDownRef;
    public GameObject upgradesDropDownRef;

    public GameObject oreCardStack;
    public GameObject carbonCardStack;
    public GameObject fuelCardStack;
    public GameObject goodsCardStack;
    public GameObject foodCardStack;
    public BuildDropDownOption[] buildDropDownOptions;
    public bool isReceivingNotifications = false;

    public Text stateText;



    // Start is called before the first frame update
    void Start()
    {
        // TODO: optimal, just pass a Hand to the HUDScript and the HUD will render it
        //DrawHand();

        List<BuildDropDownOption> options = new List<BuildDropDownOption>();

        GameObject.FindObjectOfType<Button>();

        Token colonyWithShip = new ColonyBaseToken();
        colonyWithShip.attachToken(new ShipToken());

        Token tradeWithShip = new TradeBaseToken();
        tradeWithShip.attachToken(new ShipToken());

        Token spacePortToken = new SpacePortToken();
        var spacePortOption = new BuildDropDownOption("build_space_port_btn", spacePortToken, BuildTokenBtnPressed);
        var buildTradeOption = new BuildDropDownOption("build_trade_ship_btn", tradeWithShip, BuildTokenBtnPressed);
        var buildColonyOption = new BuildDropDownOption("build_colony_ship_btn", colonyWithShip, BuildTokenBtnPressed);

        var buildShipsOptions = new BuildDropDownOption[] {
            buildColonyOption,
            buildTradeOption,
            spacePortOption
        };
        options.Add(buildColonyOption);
        options.Add(buildTradeOption);
        options.Add(spacePortOption);
        buildDropDownOptions = options.ToArray();
        buildShipsDropDownRef.GetComponent<BuildDropDown>().SetOptions(buildShipsOptions);

        var upgradeOptions = new BuildDropDownOption[] {
            new BuildDropDownOption("booster", new BoosterUpgradeToken(), BuildUpgradeBtnPressed),
            new BuildDropDownOption("cannon", new CannonUpgradeToken(), BuildUpgradeBtnPressed),
            new BuildDropDownOption("freightpod", new FreightPodUpgradeToken(), BuildUpgradeBtnPressed)
        };
        upgradesDropDownRef.GetComponent<BuildDropDown>().SetOptions(upgradeOptions);
        
    }

    // Update is called once per frame
    void Update()
    {   if (buildDropDownOptions != null && player != null)
        {
            foreach(BuildDropDownOption option in buildDropDownOptions)
            {
                bool interactable = player.CanBuildToken(option.token);
                buildShipsDropDownRef.GetComponent<BuildDropDown>().SetOptionInteractable(option, interactable);
            }
        }
    }

    public void SetStateText(string text)
    {
        stateText.text = text;
    }

    public void SetPlayer (Player player)
    {
        this.player = player;
        
    }

    void OnPlayerDataChanged()
    {
        Draw();
    }

    private void Draw()
    {
        DrawResourceStacks();
        DrawUpgrades();
    }

    void DrawResourceStacks()
    {
        Hand hand = player.hand;
        carbonCardStackText.text = hand.NumberCardsOfType<CarbonCard>().ToString();
        goodsCardStackText.text = hand.NumberCardsOfType<GoodsCard>().ToString();
        fuelCardStackText.text = hand.NumberCardsOfType<FuelCard>().ToString();
        foodCardStackText.text = hand.NumberCardsOfType<FoodCard>().ToString();
        oreCardStackText.text = hand.NumberCardsOfType<OreCard>().ToString();
    }

    void DrawUpgrades()
    {
        boosterText.text = player.ship.Boosters.ToString();
        cannonsText.text = player.ship.Cannons.ToString();
        freightPodsText.text = player.ship.FreightPods.ToString();
    }

    void BuildTokenBtnPressed(Token token)
    {
        CloseAllDropDowns();
        app.Notify(SFNotification.HUD_build_token_btn_clicked, this, new object[] { token });
    }

    public void NextButtonClicked()
    {
        app.Notify(SFNotification.next_button_clicked, this);
    }

    void BuildUpgradeBtnPressed(Token token)
    {
        CloseAllDropDowns();
        app.Notify(SFNotification.HUD_build_upgrade_btn_clicked, this, new object[] { token });
    }

    public void SettleButtonPressed()
    {
        app.Notify(SFNotification.settle_button_clicked, this);
    }

    //public void BuildTradeShipBtnPressed()
    //{
    //    app.Notify(SFNotification.HUD_build_token_btn_clicked, this);
    //    CloseAllDropDowns();
    //}

    //public void BuildColonyShipBtnPressed()
    //{
    //    app.Notify(SFNotification.HUD_build_colonyship_button_clicked, this);
    //    CloseAllDropDowns();
    //}

    //public void BuildSpaceportBtnPressed()
    //{
    //    app.Notify(SFNotification.HUD_build_spaceport_button_clicked, this);
    //    CloseAllDropDowns();
    //}

    public void BuildShipsToggleBtnPressed()
    {
        
        upgradesDropDownRef.GetComponent<BuildDropDown>().hide(); //close other dropdown if open
        buildShipsDropDownRef.GetComponent<BuildDropDown>().toggle();

    }

    public void CloseAllDropDowns()
    {

        buildShipsDropDownRef.GetComponent<BuildDropDown>().hide();
        upgradesDropDownRef.GetComponent<BuildDropDown>().hide();
    }

    public void BuildUpgradeToggleBtnPressed()
    {
        buildShipsDropDownRef.GetComponent<BuildDropDown>().hide(); //close other dropdown if open
        upgradesDropDownRef.GetComponent<BuildDropDown>().toggle();
    }

    public void MakeTradeBtnPressed()
    {

    }

    public void AddOre()
    {
        
    }

    public void AddFood()
    {
        
    }

    public void AddGoods()
    {
        
    }

    public void AddFuel()
    {
        
    }

    public void AddCarbon()
    {
        
    }

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        if (isReceivingNotifications)
        {
            switch(p_event_path)
            {
                case SFNotification.player_data_changed:
                    OnPlayerDataChanged();
                    break;
            }
        }
    }
}
