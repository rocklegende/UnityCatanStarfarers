using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SimpleCardStack : MonoBehaviour
{
    ResourceCard cardType;
    public Image cardImage;
    public Text label;

    public SimpleCardStack()
    {
    }

    public void SetImage(string imageName)
    {
        cardImage.sprite = new Helper().CreateSpriteFromImageName(imageName);
    }

    public void SetAmount(int amount)
    {
        label.text = amount.ToString();
    }

    public void SetCardType(ResourceCard cardType)
    {
        this.cardType = cardType;
        SetImage(this.cardType.resource.cardImageName);
    }

    public System.Type GetCardType()
    {
        return cardType.GetType();
    }

}
