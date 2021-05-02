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
    public System.Action<ResourceCard> AddCallback;
    public System.Action<ResourceCard> RemoveCallback;

    public SingleResourceCardStack()
    {
    }

    public void Initiliaze(ResourceCard cardType)
    {
        SetCardType(cardType);
        AddButton.onClick.AddListener(() => Add());
        RemoveButton.onClick.AddListener(() => Remove());
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

    public void Add()
    {
        AddCallback(this.cardType);
    }

    public void Remove()
    {
        RemoveCallback(this.cardType);
    }

    public void SetText(string text)
    {
        label.text = text;
    }
}
