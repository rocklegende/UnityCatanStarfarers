using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ColorCreator
{
    public static Color Create(int r, int g, int b, int a)
    {
        return new Color((float)r / 255.0F, (float)g / 255.0F, (float)b / 255.0F, (float)a / 255.0F);
    }
}

public class PlayerSelectionBox : MonoBehaviour
{

    public GameObject playerNameText;
    public bool isSelected = false;
    public System.Action<PlayerSelectionBox> itemSelectedCallback;
    private Player player;

    Color disabledColor = ColorCreator.Create(200, 200, 200, 128);
    Color selectedColor = ColorCreator.Create(256, 256, 256, 256);

    public void SetPlayer(Player player)
    {
        this.player = player;
        playerNameText.GetComponentInChildren<TextMeshProUGUI>().text = player.name;
    }

    public Player GetPlayer()
    {
        return player;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        Highlight();
    }

    public void ButtonPressed()
    {
        itemSelectedCallback(this);
    }  

    void Highlight()
    {
        var image = GetComponent<Image>();
        if (isSelected)
        {
            image.color = selectedColor;
        } else
        {
            image.color = disabledColor;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Highlight();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
