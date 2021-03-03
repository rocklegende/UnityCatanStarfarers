using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public interface FriendShipCardSelectorDelegate
{
    void didSelectFriendshipCard(TradeStation tradeStation, AbstractFriendshipCard card);
}

public class FriendShipCardSelector : MonoBehaviour
{
    private int activeIndex = 0;
    private AbstractFriendshipCard[] friendshipCards;
    private float distanceBetweenCards = 120.0f;
    public TradeStation tradeStation;

    public Button rightButton;
    public Button leftButton;
    public Button selectButton;
    public GameObject cardPrefab;
    public GameObject cardContainer;

    public FriendShipCardSelectorDelegate delegate_;

    private GameObject[] actualCardObjects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetCards(AbstractFriendshipCard[] cards)
    {
        friendshipCards = cards;
        if (actualCardObjects != null)
        {
            foreach (var obj in actualCardObjects)
            {
                Destroy(obj);
            }
        }

        actualCardObjects = Draw();
        SetCardActive(activeIndex);
    }

    public void SetTradeStation(TradeStation ts)
    {
        tradeStation = ts;
    }

    // Update is called once per frame
    void Update()
    {

    }

    GameObject[] Draw()
    {
        var position = new Vector3(0, 0, 0);
        var gameObjects = new List<GameObject>();
        foreach (var friendShipCard in friendshipCards)
        {
            GameObject cardObject = Instantiate(cardPrefab, transform, false);
            cardObject.transform.localPosition = position;
            cardObject.transform.parent = cardContainer.transform;
            cardObject.GetComponent<CardScript>().SetText(friendShipCard.GetText());
            cardObject.GetComponent<CardScript>().SetBackground(friendShipCard.GetBackgroundSprite());
            // TODO: cardObject.GetComponent<CardScript>().SetBenefitImage

            position += new Vector3(distanceBetweenCards, 0);
            gameObjects.Add(cardObject);
        }

        return gameObjects.ToArray();
    }

    private void SetCardActive(int activeIndex)
    {
        var disabledIndexes = new List<int>();
        for (int i = 0; i < friendshipCards.Length; i++)
        {
            if (i != activeIndex)
            {
                disabledIndexes.Add(i);
            }
        }

        foreach (var idx in disabledIndexes)
        {
            actualCardObjects[idx].GetComponent<CardScript>().Disable();
        }

        actualCardObjects[activeIndex].GetComponent<CardScript>().Enable();
    }

    public void OnSelectButtonPressed()
    {
        delegate_.didSelectFriendshipCard(tradeStation, friendshipCards[activeIndex]);
    }

    public void ShiftCards(float x_translation)
    {
        cardContainer.transform.position += new Vector3(x_translation, 0);
    }

    public void OnLeftButtonPressed()
    {
        if (activeIndex > 0)
        {
            SetActiveIndex(activeIndex - 1);
            ShiftCards(distanceBetweenCards);
        }
    }

    public void OnRightButtonPressed()
    {
        if (activeIndex < friendshipCards.Length - 1)
        {
            SetActiveIndex(activeIndex + 1);
            ShiftCards(-distanceBetweenCards);
        }
    }

    void SetActiveIndex(int newActiveIndex)
    {
        activeIndex = newActiveIndex;
        SetCardActive(activeIndex);
    }

}
