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
    public Hand hand = new Hand();

    public SingleResourceCardStack()
    {
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

    public void Add()
    {
        if (this.cardType != null)
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
