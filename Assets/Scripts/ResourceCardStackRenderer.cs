using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceCardStackRenderer : SFController
{
    public Hand inputHand;
    public Player player;
    public Text oreCardStackText;
    public Text carbonCardStackText;
    public Text foodCardStackText;
    public Text fuelCardStackText;
    public Text goodsCardStackText;

    public GameObject stackPrefab;
    public List<GameObject> stackObjects;
    public System.Action<Hand> ChangedCallback;

    // Start is called before the first frame update
    void Start()
    {
        CreateStacks();
    }

    public void SetChangedCallback(System.Action<Hand> callback)
    {
        this.ChangedCallback = callback;
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
        SetLimits();
    }

    public void SetLimits()
    {
        var hand = player.hand;

        var fuelStack = FindStackWithResource(new FuelCard());
        if (fuelStack != null)
        {
            fuelStack.SetLimit(hand.NumberCardsOfType<FuelCard>());
        }

        var oreStack = FindStackWithResource(new OreCard());
        if (oreStack != null)
        {
            oreStack.SetLimit(hand.NumberCardsOfType<OreCard>());
        }

        var goodsStack = FindStackWithResource(new GoodsCard());
        if (goodsStack != null)
        {
            goodsStack.SetLimit(hand.NumberCardsOfType<GoodsCard>());
        }

        var foodStack = FindStackWithResource(new FoodCard());
        if (foodStack != null)
        {
            foodStack.SetLimit(hand.NumberCardsOfType<FoodCard>());
        }

        var CarbonStack = FindStackWithResource(new CarbonCard());
        if (CarbonStack != null)
        {
            CarbonStack.SetLimit(hand.NumberCardsOfType<CarbonCard>());
        }
    }

    SingleResourceCardStack FindStackWithResource(ResourceCard card)
    {
        foreach (var stackObject in stackObjects)
        {
            var stackScript = stackObject.GetComponent<SingleResourceCardStack>();
            if (stackScript.GetCardType().GetType() == card.GetType())
            {
                return stackScript;
            }
        }
        return null;
    }



    public void ResetStacks()
    {
        foreach (var stackObject in stackObjects)
        {
            var stackScript = stackObject.GetComponent<SingleResourceCardStack>();
            stackScript.Reset();
        }
        Changed();
    }

    void CreateStacks()
    {
        var allCardTypes = new Helper().GetAllResourceCardTypes();
        foreach (var cardType in allCardTypes)
        {

            var stack = Instantiate(stackPrefab, gameObject.transform, false);
            var stackScript = stack.GetComponent<SingleResourceCardStack>();
            stackScript.SetCardType(cardType);
            stackScript.AddButton.onClick.AddListener(() => AddCard(stackScript));
            stackScript.RemoveButton.onClick.AddListener(() => RemoveCard(stackScript));

            stackObjects.Add(stack);
            stack.transform.parent = this.gameObject.transform;
        }
    }

    void AddCard(SingleResourceCardStack stackScript)
    {
        stackScript.Add();
        Changed();
    }

    void RemoveCard(SingleResourceCardStack stackScript)
    {
        stackScript.Remove();
        Changed();
    }

    void Changed()
    {
        ChangedCallback(GetOutput());
    }

    GameObject FindStackOfCardType(ResourceCard cardType)
    {
        foreach(var stack in stackObjects)
        {
            if (stack.GetComponent<SingleResourceCardStack>().GetCardType() == cardType)
            {
                return stack;
            }
        }
        return null;
    }

    void SetInputHand()
    {

    }

    public Hand GetOutput()
    {
        // combine hands of all
        Hand hand = new Hand();
        foreach(var stack in stackObjects)
        {
            hand.AddHand(stack.GetComponent<SingleResourceCardStack>().hand);
        }

        return hand;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            SetLimits();
        }
    }

    void DrawResourceStacks()
    {
        carbonCardStackText.text = inputHand.NumberCardsOfType<CarbonCard>().ToString();
        goodsCardStackText.text = inputHand.NumberCardsOfType<GoodsCard>().ToString();
        fuelCardStackText.text = inputHand.NumberCardsOfType<FuelCard>().ToString();
        foodCardStackText.text = inputHand.NumberCardsOfType<FoodCard>().ToString();
        oreCardStackText.text = inputHand.NumberCardsOfType<OreCard>().ToString();
    }

    public void OnCarbonCardClicked()
    {

    }

    public void OnFuelCardClicked()
    {

    }

    public void OnGoodsCardClicked()
    {

    }

    public void OnFoodCardClicked()
    {

    }

    public void OnOreCardClicked()
    {

    }

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        switch (p_event_path)
        {
            case SFNotification.player_data_changed:
                if (player != null)
                {
                    SetLimits();
                }

                break;
        }
    }
}
