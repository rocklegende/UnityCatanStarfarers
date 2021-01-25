using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BuildDropDown : MonoBehaviour
{

    BuildDropDownOption[] options;
    public GameObject dropDownBtnPrefab;
    public bool isOpen = false;

    // Use this for initialization
    void Start()
    {
        //options = new BuildDropDownOption[] {
        //    new BuildDropDownOption("build_colony_ship_btn", new Cost(new Resource[] { new FoodResource(), new FoodResource(), new GoodsResource() }), PrintSomething),
        //    new BuildDropDownOption("build_trade_ship_btn", new Cost(new Resource[] { new FoodResource(), new FoodResource(), new FuelResource() }), PrintSomethingCool),
        //    new BuildDropDownOption("build_space_port_btn", new Cost(new Resource[] { new GoodsResource(), new GoodsResource(), new GoodsResource() }), PrintSomethingCooler)
        //};
        //Debug.Log("Th111");
        //DrawOptions();
        gameObject.SetActive(false);
    }

    public void SetOptions(BuildDropDownOption[] opts)
    {
        options = opts;
        DrawOptions();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void show()
    {
        isOpen = true;
        gameObject.SetActive(isOpen);
    }

    public void hide()
    {

        isOpen = false;
        gameObject.SetActive(isOpen);
    }

    public void toggle()
    {
        if (isOpen)
        {
            hide();
        } else
        {
            show();
        }
    }

    void DrawOptions()
    {

        float verticalOffset = 60.0f;
        Vector3 position = new Vector3(200, 200, 0);

        foreach(BuildDropDownOption option in options)
        {
            DrawOption(option, position);
            position += new Vector3(0, verticalOffset, 0);
        }
    }

    void DrawOption(BuildDropDownOption option, Vector3 position)
    {
        Sprite sprite = new Helper().CreateSpriteFromImageName(option.imageName);
        GameObject btn = (GameObject)Instantiate(dropDownBtnPrefab, position, Quaternion.identity);
        btn.GetComponentInChildren<BuildDropDownBtnImageScript>().SetSprite(sprite);
        Button btnComponent = btn.GetComponent<Button>();
        btnComponent.onClick.AddListener(delegate { option.method(); }); 

        Cost cost = new Cost(new Resource[] { new FoodResource(), new FoodResource(), new GoodsResource() }); 
        btn.GetComponentInChildren<CostRenderer>().SetCost(option.cost);

        btn.transform.parent = this.transform;

    }

}
