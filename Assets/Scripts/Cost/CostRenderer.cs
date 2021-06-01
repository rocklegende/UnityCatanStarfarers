using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class CostRenderer : MonoBehaviour
{
    public Cost cost;
    // Use this for initialization
    void Start()
    {
    }

    Dictionary<string, int> GetGroupedResources()
    {
        Dictionary<string, int> resourcesCount = new Dictionary<string, int>();

        foreach(Resource resource in cost.hand.ToResourceList())
        {
            try
            {
                resourcesCount.Add(resource.cardImageName, 1);
            }
            catch (ArgumentException e)
            {
                resourcesCount[resource.cardImageName] += 1;
            } 
        }

        return resourcesCount;
    }

    public void SetCost(Cost cost)
    {
        this.cost = cost;
        Draw();
    }

    public void Draw()
    {
        var resourcesCount = GetGroupedResources();
        var position = new Vector3(0, 0, 0);
        var step = 40.0f;

        foreach (KeyValuePair<string, int> entry in resourcesCount)
        {
            DrawCard(entry.Key, entry.Value, position);
            position += new Vector3(step, 0, 0);
        }

    }

    public void DrawCard(string imageName, int numCards, Vector3 position)
    {
        Sprite sprite = new Helper().CreateSpriteFromImageName(imageName);

        GameObject TextContainer = new GameObject();
        Text numCardsText = TextContainer.AddComponent<Text>();
        numCardsText.text = numCards.ToString();
        numCardsText.font = GameConstants.GetFont();
        numCardsText.color = Color.black;
        numCardsText.alignment = TextAnchor.UpperCenter;

        GameObject ImageContainer = new GameObject(); //Create the GameObject
        Image NewImage = ImageContainer.AddComponent<Image>(); //Add the Image Component script
        NewImage.sprite = sprite; //Set the Sprite of the Image Component on the new GameObject



        TextContainer.GetComponent<RectTransform>().SetParent(this.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
        TextContainer.SetActive(true); //Activate the GameObject

        ImageContainer.GetComponent<RectTransform>().SetParent(this.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
        ImageContainer.SetActive(true);

        RectTransform rt = ImageContainer.GetComponent<RectTransform>();
        rt.localPosition = position + new Vector3(0, -20, 0);
        rt.pivot = new Vector2(0, 1);
        rt.sizeDelta = new Vector2(30, 40);

        RectTransform rt_text = TextContainer.GetComponent<RectTransform>();
        rt_text.localPosition = position;
        rt_text.pivot = new Vector2(0, 1);
        rt_text.sizeDelta = new Vector2(30, 40);



    } 

    // Update is called once per frame
    void Update()
    {

    }
}
