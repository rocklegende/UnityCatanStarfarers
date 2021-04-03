using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawPileView_ : MonoBehaviour, Observer
{
    AvailablePiles model;
    DrawPileHandler handler;
    public GameObject simpleHandRendererObject;
    public Text hiddenText;
    Hand handModelOpenStacks;


    public void SubjectDataChanged(object[] data)
    {
        simpleHandRendererObject.GetComponent<SimpleHandRenderer>().hand = model.GetOpenResourcesCombinedHand();
        Draw();
    }

    public void Init(AvailablePiles piles, DrawPileHandler handler)
    {
        model = piles;
        handModelOpenStacks = piles.GetOpenResourcesCombinedHand();
        simpleHandRendererObject.GetComponent<SimpleHandRenderer>().Initiliaze(handModelOpenStacks);
        Draw();
        piles.RegisterObserver(this);
    }

    public void Draw()
    {
        simpleHandRendererObject.GetComponent<SimpleHandRenderer>().Draw();
        hiddenText.text = model.hiddenDrawPile.CardsLeft().ToString();
    }
}
