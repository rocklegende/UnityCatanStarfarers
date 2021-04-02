using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public interface SelectionView
{
    void InitializeViewFromObject(SFModel obj);
}

public class PlayerView : MonoBehaviour, SelectionView
{
    public GameObject playerNameText;
    public void InitializeViewFromObject(SFModel obj)
    {
        if (obj is Player)
        {
            var player = (Player)obj;
            playerNameText.GetComponentInChildren<Text>().text = player.name;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
