using System;
public class TileGroup
{
    private Tile_[] tiles;
    public TileGroup(Tile_[] tiles, int rightShifts = 0)
    {
        // shifts: shifts the tiles x times
        if (tiles.Length != 3)
        {
            throw new ArgumentException("TileGroup has to consist of exactly 3 tiles.");
        }
        this.tiles = tiles;
        ShiftTiles(rightShifts);
    }

    public Tile_[] GetTiles()
    {
        return this.tiles;
    }

    public void ShiftTiles(int numShifts)
    {
        Tile_[] shifted = new Tile_[tiles.Length];

        for (int i = 0; i < tiles.Length; i++)
        {
            shifted[(i + numShifts) % shifted.Length] = tiles[i];
        }
        this.tiles = shifted;
    }

}

public class ResourceTileGroup : TileGroup
{
    ResourceTile[] tiles;
    public ResourceTileGroup(ResourceTile[] tiles, int rightShifts = 0) : base(tiles, rightShifts)
    {
        this.tiles = tiles;
    }

    public void RevealDiceChips()
    {
        foreach( var tile in tiles)
        {
            tile.GetDiceChip().isFaceUp = true;
        }
    }
}
