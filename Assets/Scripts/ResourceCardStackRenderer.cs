using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceCardStackRenderer : SFController
{
    private Hand _handLimit = null;
    private int maximumPickableCards = -1;
    private Hand pickedHand = new Hand();

    public GameObject stackPrefab;
    public List<GameObject> stackObjects = new List<GameObject>();
    public System.Action<Hand> ChangedCallback;

    // Start is called before the first frame update
    void Start()
    {
        _handLimit = null;
        stackObjects = new List<GameObject>();
        CreateStacks();
    }

    public void Init()
    {
        
    }

    public void SetHand(Hand hand)
    {
        pickedHand = hand;
        Changed();
    }

    public void SetChangedCallback(System.Action<Hand> callback)
    {
        this.ChangedCallback = callback;
    }

    public void SetTotalMaximum(int amount)
    {
        maximumPickableCards = amount;
    }

    bool MaximumIsSet()
    {
        return maximumPickableCards != -1;
    }

    bool MaximumReached()
    {
        return MaximumIsSet() && pickedHand.Count() == maximumPickableCards;
    }

    bool NewHandExceedsHandLimit(Hand newHand)
    {
        if (this._handLimit != null)
        {
            return !newHand.IsSubsetOf(_handLimit);
        } else
        {
            return false;
        }
    }

    public void IncreaseResource(ResourceCard card)
    {
        var futurePickedHand = pickedHand.SimpleClone();
        futurePickedHand.AddCard(card);
        Debug.Log("max: " + !MaximumReached());
        Debug.Log("exceeds: " + !NewHandExceedsHandLimit(futurePickedHand));
        Debug.Log("this.handLimit: " + this._handLimit);
        Debug.Log("this.handLimit != null " + this._handLimit != null);

        if (!MaximumReached() && !NewHandExceedsHandLimit(futurePickedHand))
        {
            pickedHand.AddCard(card);
        }
        Changed();
    }

    public void DecreaseResource(ResourceCard card)
    {
        try
        {
            pickedHand.RemoveCard(card);
        } catch
        {
            Debug.Log("some error occured, dont bother with it");
        }
        Changed();
    }

    public void SetHandLimit(Hand handLimit)
    {
        Debug.Log("Setting hand limit");
        this._handLimit = handLimit;
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
        pickedHand.RemoveAllCards();
        Changed();
    }

    void CreateStacks()
    {
        Debug.Log("CREATING STACKS");
        var allCardTypes = new Helper().GetAllResourceCardTypes();
        foreach (var cardType in allCardTypes)
        {
            var stack = Instantiate(stackPrefab, gameObject.transform, false);
            var stackScript = stack.GetComponent<SingleResourceCardStack>();
            stackScript.Initiliaze(cardType);
            stackScript.AddCallback = IncreaseResource;
            stackScript.RemoveCallback = DecreaseResource;
            stackObjects.Add(stack);
            stack.transform.parent = this.gameObject.transform;
        }
        Debug.Log("Num stackobjects 1: " + stackObjects.Count);
        Debug.Log("");
    }

    void Changed()
    {
        
        Draw();
        ChangedCallback?.Invoke(GetDisplayedHand());
    }

    public Hand GetDisplayedHand()
    {
        return pickedHand;
    }

    // Update is called once per frame
    void Update()
    {
        Draw();
    }

    public void Draw()
    {
        //Debug.Log("SingleResourceCardStacks before" + GetComponentsInChildren<SingleResourceCardStack>().Length);
        //Debug.Log("stackobjects count before" + stackObjects.Count);
        //foreach (var stack in stackObjects)
        //{
        //    GameObject.Destroy(stack);
        //}
        //stackObjects = new List<GameObject>();
        //Debug.Log("SingleResourceCardStacks after" + GetComponentsInChildren<SingleResourceCardStack>().Length);
        //Debug.Log("stackobjects count after" + stackObjects.Count);

        //CreateStacks();
        //Debug.Log("Num stackobjects 2" + stackObjects.Count);

        DrawResourceStacks();
    }

    void DrawResourceStacks()
    {
        Debug.Log("DRAWING");
        var singlestacks = GetComponentsInChildren<SingleResourceCardStack>().Length;
        Debug.Log("Num stackobjects" + stackObjects.Count);
        Debug.Log("Single Resource Card Stack" + gameObject.GetComponentsInChildren<SingleResourceCardStack>().Length);
        Debug.Log("Drawing hand!" + pickedHand.Count());
        foreach (var singleStackObject in GetComponentsInChildren<SingleResourceCardStack>())
        {
            //var singleStackObject = stack.GetComponent<SingleResourceCardStack>();
            var text = pickedHand.NumberCardsOfType(singleStackObject.GetCardType().GetType()).ToString();
            Debug.Log(text);
            singleStackObject.SetText(pickedHand.NumberCardsOfType(singleStackObject.GetCardType().GetType()).ToString());
        }
    }

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        
    }
}
