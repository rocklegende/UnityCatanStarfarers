using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceThrowRenderer : SFView
{

    public Text firstDiceText;
    public Text secondDiceText;
    // Start is called before the first frame update
    void Start()
    {
        DrawDiceThrow(new DiceThrow(4, 5));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DrawDiceThrow(DiceThrow dt)
    {
        firstDiceText.text = dt.firstDice.ToString();
        secondDiceText.text = dt.secondDice.ToString();
    }

    public void OnBtnClick()
    {
        Cast();
    }

    public DiceThrow Cast(bool notify = false)
    {
        var diceThrower = new DiceThrower();
        var dt = diceThrower.throwDice();
        DrawDiceThrow(dt);
        
        if (notify)
        {
            app.Notify(SFNotification.dice_thrown, this, new object[] { dt });
        }
        return dt;
    }
}
