using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildDropDownBtnImageScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSprite(Sprite sprite)
    {
        Image image = GetComponent<Image>();
        image.sprite = sprite;
    }
}
