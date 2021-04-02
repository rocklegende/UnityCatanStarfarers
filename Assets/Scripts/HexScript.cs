using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TradeStationRenderer : SFElement
{
    //public TradeStation tradeStation;
    //public HexCoordinates hexCoords;
    //public GameObject dockStationPrefab;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    SpriteRenderer sr = this.gameObject.GetComponentInChildren<SpriteRenderer>();
    //    sr.sprite = null;
    //    Draw();
    //}

    //public void SetTile(Tile_ tile)
    //{
    //    this.tile = tile;
    //    Draw();
    //}

    //public void Draw()
    //{
    //    foreach (TradeStationTile tile in tradeStation.GetTiles()) 

    //    if (tile is TradeStationTile)
    //    {
    //        // add prefabs for docking stations that are clickable
    //        TradeStationTile tst = (TradeStationTile)tile;
    //        //CreateTradeStationDocks(tst.numDockingPorts);
    //    }


    //    MeshRenderer mr = this.gameObject.GetComponentInChildren<MeshRenderer>();
    //    //mr.material.color = Color.yellow;
    //    mr.material.color = (Color)tile.GetColor();
    //}

    //void CreateTradeStationDocks(int numDocks)
    //{
    //    //TODO: keep this, use inside TileGroupRenderer later
    //    for (int i = 0; i < numDocks; i++)
    //    {
    //        //TODO: use width of tile for calculation
    //        var rad = Mathf.Deg2Rad * (i * 360f / numDocks);
    //        Vector3 position = new Vector3(Mathf.Cos(rad) * 0.5f, Mathf.Sin(rad) * 0.5f, 0) + this.transform.position;
    //        GameObject btn = (GameObject)Instantiate(dockStationPrefab, position, Quaternion.identity);
    //        btn.transform.parent = this.gameObject.transform;
    //    }

    //}


    //// Update is called once per frame
    //void Update()
    //{

    //}
}

public class HexScript : MonoBehaviour
{
    public Tile_ tile;
    public (int, int) mapArrayIndexes;
    public HexCoordinates hexCoords;
    public GameObject dockStationPrefab;

    int smallestFontSize;
    int largestFontSize;

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer sr = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        sr.sprite = null;
        //numberContainer.SetActive(false);
        Draw();
    }

    public void SetTile(Tile_ tile)
    {
        this.tile = tile;
        Draw();
    }

    public void Draw()
    {
        if (tile is ResourceTile)
        {
            ResourceTile rt = (ResourceTile)tile;
            SpriteRenderer sr = this.gameObject.GetComponentInChildren<SpriteRenderer>();
            if (rt.diceChip != null)
            {
                var textMesh = GetComponentInChildren<TextMeshPro>();
                if (textMesh.IsNotNull())
                {
                    if (!rt.diceChip.isFaceUp || rt.diceChip is PirateToken)
                    {
                        //numberContainer.SetActive(false);
                        textMesh.text = "";
                        string textureName = rt.diceChip.isFaceUp ? rt.diceChip.GetTextureName() : rt.chipGroup.GetTextureName();

                        Texture2D texture = Resources.Load(textureName) as Texture2D;
                        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.0f, 0.0f));
                        sr.sprite = sprite;
                    }
                    else
                    {
                        //numberContainer.SetActive(true);
                        sr.sprite = null;
                        textMesh.text = rt.diceChip.GetValuesAsString();
                        textMesh.fontSize = (float)GetFontSizeFromDiceValues(rt.diceChip.GetValues());
                    }
                }
                
                
            }
        }

        MeshRenderer mr = this.gameObject.GetComponentInChildren<MeshRenderer>();
        mr.material.color = (Color)tile.GetColor();
    }

    int GetFontSizeFromDiceValues(List<int> diceValues)
    {

        if (diceValues.Contains(2) || diceValues.Contains(12))
        {
            return 6;
        }
        if (diceValues.Contains(3) || diceValues.Contains(11))
        {
            return 7;
        }
        if (diceValues.Contains(4) || diceValues.Contains(10))
        {
            return 8;
        }
        if (diceValues.Contains(5) || diceValues.Contains(9))
        {
            return 9;
        }
        if (diceValues.Contains(6) || diceValues.Contains(8))
        {
            return 10;
        }
        return 10;

    }
    

    // Update is called once per frame
    void Update()
    {
    }
}
