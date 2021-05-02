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

    void Update()
    {
        //TODO: somehow displaying a hand via SetHand does not display the hand immediately, therefore the workaround for now is to update it every frame
        Draw();
    }

    public void Draw()
    {
        DrawResourceStacks();
    }

    void DrawResourceStacks()
    {        
        foreach (var singleStackObject in GetComponentsInChildren<SingleResourceCardStack>())
        {
            singleStackObject.SetText(pickedHand.NumberCardsOfType(singleStackObject.GetCardType().GetType()).ToString());
        }
    }

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        
    }
}
