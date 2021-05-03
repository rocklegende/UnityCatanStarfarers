using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeView : MonoBehaviour, SelectionView
{
    public GameObject textLabel;
    public void InitializeViewFromObject(SFModel obj)
    {
        if (obj is Token)
        {
            var token = (Token)obj;
            textLabel.GetComponentInChildren<Text>().text = token.id;
        }
    }

    
}
