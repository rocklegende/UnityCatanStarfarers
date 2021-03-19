using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SingleResourceCardStack : MonoBehaviour
{
    ResourceCard cardType;
    public Button AddButton;
    public Button RemoveButton;
    public Image cardImage;
    public Text label;
    int limit = 1000;
    public Hand hand = new Hand();

    public SingleResourceCardStack()
    {
    }

    public void SetLimit(int limit)
    {
        this.limit = limit;
    }

    public void SetCardType(ResourceCard cardType)
    {
        this.cardType = cardType;
        SetImage(this.cardType.resource.cardImageName);
    }

    public ResourceCard GetCardType()
    {
        return this.cardType;
    }

    void SetImage(string imageName)
    {
        cardImage.sprite = new Helper().CreateSpriteFromImageName(imageName);
    }

    public void Reset()
    {
        hand.RemoveAllCards();
        Redraw();
    }

    public void Add()
    {
        if (this.cardType != null && hand.Count() + 1 <= limit)
        {
            hand.AddCard(this.cardType);
        }
        Redraw();
    }

    public void Remove()
    {
        if (hand.Count() > 0)
        {
            hand.RemoveCardOfType(this.cardType.GetType());
        }
        Redraw();
    }

    public void Redraw()
    {
        label.text = hand.Count().ToString();
    }


}
