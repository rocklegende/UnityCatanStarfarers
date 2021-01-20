using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexScript : MonoBehaviour
{
    public Tile_ tile;
    public (int, int) mapArrayIndexes;
    public HexCoordinates hexCoords;

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
        ResourceTile rt = (ResourceTile)tile;
        rt.SetDiceChip(dc); 
        Draw();
    }

    public void FlipDiceChip()
    {
        ResourceTile rt = (ResourceTile)tile;
        rt.diceChip.Flip();
        Draw();
    }

    void Draw()
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
        MeshRenderer mr = this.gameObject.GetComponentInChildren<MeshRenderer>();
        mr.material.color = (Color)tile.GetColor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
