using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDScript : SFController
{
    public Text oreCardStackText;
    public Text carbonCardStackText;
    public Text foodCardStackText;
    public Text fuelCardStackText;
    public Text goodsCardStackText;
    private int ore = 0;
    private int food = 0;
    private int goods = 0;
    private int fuel = 0;
    private int carbon = 0;

    public GameObject buildShipsDropDownRef;
    public GameObject upgradesDropDownRef;

    //GameObject costrendererPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: optimal, just pass a Hand to the HUDScript and the HUD will render it
        //DrawHand();

        var buildShipsOptions = new BuildDropDownOption[] {
            new BuildDropDownOption("build_colony_ship_btn", new Cost(new Resource[] { new FoodResource(), new FoodResource(), new GoodsResource() }), PrintSomething),
            new BuildDropDownOption("build_trade_ship_btn", new Cost(new Resource[] { new FoodResource(), new FoodResource(), new FuelResource() }), BuildTradeShipBtnPressed),
            new BuildDropDownOption("build_space_port_btn", new Cost(new Resource[] { new GoodsResource(), new GoodsResource(), new GoodsResource() }), PrintSomethingCooler)
        };
        buildShipsDropDownRef.GetComponent<BuildDropDown>().SetOptions(buildShipsOptions);

        var upgradeOptions = new BuildDropDownOption[] {
            new BuildDropDownOption("booster", new Cost(new Resource[] { new FuelResource(), new FuelResource() }), PrintSomething),
            new BuildDropDownOption("cannon", new Cost(new Resource[] { new CarbonResource(), new CarbonResource()}), PrintSomethingCool),
            new BuildDropDownOption("freightpod", new Cost(new Resource[] { new OreResource(), new OreResource() }), PrintSomethingCooler)
        };
        upgradesDropDownRef.GetComponent<BuildDropDown>().SetOptions(upgradeOptions);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PrintSomething()
    {
        Debug.Log("231");
    }

    void PrintSomethingCool()
    {
        Debug.Log("cool");
    }

    void PrintSomethingCooler()
    {
        Debug.Log("cooler");
    }

    public void BuildTradeShipBtnPressed()
    {
        app.Notify(SFNotification.HUD_build_tradeship_button_clicked, this);
        CloseAllDropDowns();
    }

    public void BuildColonyShipBtnPressed()
    {
        
        upgradesDropDownRef.GetComponent<BuildDropDown>().hide(); //close other dropdown if open
        buildShipsDropDownRef.GetComponent<BuildDropDown>().toggle();

    }

    public void CloseAllDropDowns()
    {

        buildShipsDropDownRef.GetComponent<BuildDropDown>().hide();
        upgradesDropDownRef.GetComponent<BuildDropDown>().hide();
    }

    public void BuildUpgradeBtnPressed()
    {
        buildShipsDropDownRef.GetComponent<BuildDropDown>().hide(); //close other dropdown if open
        upgradesDropDownRef.GetComponent<BuildDropDown>().toggle();
    }

    public void BuildSpaceportBtnPressed()
    {

    }

    public void MakeTradeBtnPressed()
    {

    }


    public void AddOre()
    {
        ore += 1;
        oreCardStackText.text = ore.ToString();
    }

    public void AddFood()
    {
        food += 1;
        foodCardStackText.text = food.ToString();
    }

    public void AddGoods()
    {
        goods += 1;
        goodsCardStackText.text = goods.ToString();
    }

    public void AddFuel()
    {
        fuel += 1;
        fuelCardStackText.text = fuel.ToString();
    }

    public void AddCarbon()
    {
        carbon += 1;
        carbonCardStackText.text = carbon.ToString();
    }

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        Debug.Log("AHSDKAHSDKSA");
    }
}
