using System;
public class TileGroup
{
    private Tile_[] tiles;
    public TileGroup(Tile_[] tiles)
    {

        if (tiles.Length != 3)
        {
            throw new ArgumentException("TileGroup has to consist of exactly 3 tiles.");
        }
        this.tiles = tiles;
    }

    public Tile_[] GetTiles()
    {
        return this.tiles;
    }

}
