using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipDiceThrowRenderer : SFView
{

    public Text firstDiceText;
    public Text secondDiceText;
    public Action<ShipDiceThrow> callback { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        DrawDiceThrow(new ShipDiceThrow(6, 5));
    }

    // Update is called once per frame
    void Update()
    {

    }

    void DrawDiceThrow(ShipDiceThrow dt)
    {
        firstDiceText.text = dt.value1.ToString();
        secondDiceText.text = dt.value2.ToString();
    }

    public void OnBtnClick()
    {
        var diceThrower = new ShipDiceThrower();
        var dt = diceThrower.CastDice();
        DrawDiceThrow(dt);
        callback(dt);

        //app.Notify(SFNotification.ship_dice_thrown, this, new object[] { dt });
    }
}