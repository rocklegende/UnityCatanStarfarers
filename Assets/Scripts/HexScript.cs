using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexScript : MonoBehaviour
{
    public Tile_ tile;
    public (int, int) mapArrayIndexes;
    public HexCoordinates hexCoords;
    public GameObject dockStationPrefab;

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer sr = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        sr.sprite = null;
        Draw();
    }

    public void SetTile(Tile_ tile)
    {
        this.tile = tile;
        Draw();
    }

    public void SetDiceChip(DiceChip dc)
    {
        if (tile is ResourceTile)
        {
            ResourceTile rt = (ResourceTile)tile;
            rt.SetDiceChip(dc);
        }
        Draw();
    }

    public void FlipDiceChip()
    {
        ResourceTile rt = (ResourceTile)tile;
        rt.diceChip.Flip();
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
                Sprite sprite = rt.diceChip.GetSprite();
                sr.sprite = sprite;
            }
            

        }

        if (tile is TradeStationTile)
        {
            // add prefabs for docking stations that are clickable
            TradeStationTile tst = (TradeStationTile)tile;
            CreateTradeStationDocks(tst.numDockingPorts);
        }


        MeshRenderer mr = this.gameObject.GetComponentInChildren<MeshRenderer>();
        //mr.material.color = Color.yellow;
        mr.material.color = (Color)tile.GetColor();
    }

    void CreateTradeStationDocks(int numDocks)
    {
        for (int i = 0; i < numDocks; i++)
        {
            //TODO: use width of tile for calculation
            var rad = Mathf.Deg2Rad * (i * 360f / numDocks);
            Vector3 position = new Vector3(Mathf.Cos(rad) * 0.5f, Mathf.Sin(rad) * 0.5f, 0) + this.transform.position;
            GameObject btn = (GameObject)Instantiate(dockStationPrefab, position, Quaternion.identity);
            btn.transform.parent = this.gameObject.transform;
        }
        
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
