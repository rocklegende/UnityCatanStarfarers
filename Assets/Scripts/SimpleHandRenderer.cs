using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHandRenderer : MonoBehaviour, Observer
{
    public GameObject simpleCardStackPrefab;
    List<SimpleCardStack> cardStackScripts = new List<SimpleCardStack>();
    public Hand hand;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initiliaze(Hand hand)
    {
        this.hand = hand;
        hand.RegisterObserver(this);
        CreateStacks();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateStacks()
    {
        var allCardTypes = new Helper().GetAllResourceCardTypes();
        foreach (var cardType in allCardTypes)
        {

            var stack = Instantiate(simpleCardStackPrefab, gameObject.transform, false);
            var stackScript = stack.GetComponent<SimpleCardStack>();
            stackScript.SetCardType(cardType);
            cardStackScripts.Add(stackScript);
            stack.transform.parent = this.gameObject.transform;
        }

        Draw();
    }

    public void SubjectDataChanged()
    {
        Draw();
    }

    public void Draw()
    {
        foreach (var stackScript in cardStackScripts)
        {
            stackScript.SetAmount(hand.NumberCardsOfType(stackScript.GetCardType()));
        }
    }
}
