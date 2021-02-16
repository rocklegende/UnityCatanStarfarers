using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Text cardText;
    public GameObject disablePanel;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string text)
    {
        cardText.text = text;
    }

    public void Enable()
    {
        disablePanel.SetActive(false);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        transform.position = new Vector3(transform.position.x, transform.position.y, 10);
    }

    public void Disable()
    {
        disablePanel.SetActive(true);
        transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}
