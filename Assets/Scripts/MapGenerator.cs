using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapGenerator
{

    // TODO: generate map from .txt file
    /*
     * E = EmptyTile
     * B = BorderTile
     * 
     * BBBBBBB
     * BEEEEEB
     * BEEEEEB
     * BBBBBBB
     * 
     * 
     */
    SpacePoint[] basePoints;
    SpacePoint[] randomSpawnPoints;
    List<TileGroup> randomSpawnTileGroup;
    Map map;

    public MapGenerator()
    {
        Helper helper = new Helper();
        basePoints = helper.SpacePointArrayFromShortRep(new string[]
        {
            "15,2,1",
            "13,5,0",
            "12,8,1",
            "10,11,0"
        });

        randomSpawnPoints = helper.SpacePointArrayFromShortRep(new string[]
        {
            "1,3,0",
            "0,6,1",
            "-2,10,1",
            "3,4,1",
            "0,10,0",
            "2,7,1",
            "5,3,0",
            "4,6,0",
            "3,10,1",
            "8,3,1",
            "7,6,0",
            "5,9,0",
            "10,3,0",
            "9,7,1",
            "7,10,1"
        });

        var orz = new OrzelTradeStation();

        randomSpawnTileGroup = new List<TileGroup>();
        randomSpawnTileGroup.Add(new TileGroup(new Tile_[] { new GoodsResourceTile(), new GoodsResourceTile(), new OreResourceTile() }));
        randomSpawnTileGroup.Add(new TileGroup(new Tile_[] { new GoodsResourceTile(), new GoodsResourceTile(), new OreResourceTile() }));
        randomSpawnTileGroup.Add(new TileGroup(new Tile_[] { new GoodsResourceTile(), new GoodsResourceTile(), new OreResourceTile() }));
        randomSpawnTileGroup.Add(new TileGroup(new Tile_[] { new GoodsResourceTile(), new GoodsResourceTile(), new OreResourceTile() }));
        randomSpawnTileGroup.Add(new TileGroup(new Tile_[] { new OrzelTradeStationTile(orz, 2), new OrzelTradeStationTile(orz, 3), new OrzelTradeStationTile(orz, 1) }));
        randomSpawnTileGroup.Add(new TileGroup(new Tile_[] { new GoodsResourceTile(), new GoodsResourceTile(), new OreResourceTile() }));
        randomSpawnTileGroup.Add(new TileGroup(new Tile_[] { new FuelResourceTile(), new GoodsResourceTile(), new OreResourceTile() }));
        randomSpawnTileGroup.Add(new TileGroup(new Tile_[] { new GoodsResourceTile(), new GoodsResourceTile(), new OreResourceTile() }));
        randomSpawnTileGroup.Add(new TileGroup(new Tile_[] { new FuelResourceTile(), new GoodsResourceTile(), new OreResourceTile() }));
        randomSpawnTileGroup.Add(new TileGroup(new Tile_[] { new FuelResourceTile(), new GoodsResourceTile(), new OreResourceTile() }));
        randomSpawnTileGroup.Add(new TileGroup(new Tile_[] { new FuelResourceTile(), new GoodsResourceTile(), new OreResourceTile() }));
        randomSpawnTileGroup.Add(new TileGroup(new Tile_[] { new FuelResourceTile(), new GoodsResourceTile(), new OreResourceTile() }));
        randomSpawnTileGroup.Add(new TileGroup(new Tile_[] { new FuelResourceTile(), new GoodsResourceTile(), new OreResourceTile() }));
        randomSpawnTileGroup.Add(new TileGroup(new Tile_[] { new FuelResourceTile(), new GoodsResourceTile(), new OreResourceTile() }));
        randomSpawnTileGroup.Add(new TileGroup(new Tile_[] { new FuelResourceTile(), new GoodsResourceTile(), new OreResourceTile() }));
        randomSpawnTileGroup.Add(new TileGroup(new Tile_[] { new FuelResourceTile(), new GoodsResourceTile(), new OreResourceTile() }));
        randomSpawnTileGroup.Add(new TileGroup(new Tile_[] { new CarbonResourceTile(), new GoodsResourceTile(), new OreResourceTile() }));
    }

    public Map GenerateRandomMap() {
        Tile_[,] raw = RawMap();

        map = new Map(raw);
        PopulateBasePoints();
        PopulateRandomSpawnPoints();
        AssignDiceChips();

        return map;
    }

    void RotateRandomSpawnTileGroups() {
        foreach (TileGroup tg in randomSpawnTileGroup)
        {
            tg.ShiftTiles(ThreadSafeRandom.ThisThreadsRandom.Next(tg.GetTiles().Length));
        }
    }


    void PopulateBasePoints()
    {
        map.SetTileGroupAtSpacePoint(new TileGroup(new Tile_[] { new FuelResourceTile(), new GoodsResourceTile(), new OreResourceTile() }), basePoints[0]);
        map.SetTileGroupAtSpacePoint(new TileGroup(new Tile_[] { new OreResourceTile(), new CarbonResourceTile(), new FoodResourceTile() }), basePoints[1]);
        map.SetTileGroupAtSpacePoint(new TileGroup(new Tile_[] { new OreResourceTile(), new FuelResourceTile(), new GoodsResourceTile() }), basePoints[2]);
        map.SetTileGroupAtSpacePoint(new TileGroup(new Tile_[] { new FoodResourceTile(), new FuelResourceTile(), new CarbonResourceTile() }), basePoints[3]);
    }

    void PopulateRandomSpawnPoints()
    {
        // TODO: bug here at rotating or shuffling the spawn groups
        RotateRandomSpawnTileGroups();

        foreach (SpacePoint point in randomSpawnPoints)
        {
            randomSpawnTileGroup.Shuffle();
            TileGroup tg = randomSpawnTileGroup[0];
            randomSpawnTileGroup.RemoveAt(0);


            var tilesAtSpacePointBefore = map.getTilesAtPoint(point);

            map.SetTileGroupAtSpacePoint(tg, point);
            var tilesAtSpacePointAfter = map.getTilesAtPoint(point);
            Debug.Log("bla");
        }
    }

    void AssignDiceChips()
    {
        foreach(ResourceTile resourceTile in map.GetAllTilesOfType<ResourceTile>())
        {
            CircleChipGroup group = new CircleChipGroup();
            DiceChip dc = new DiceChip3(group);
            dc.Flip();
            resourceTile.SetDiceChip(dc);
        }
    }



    Tile_[,] RawMap() {
        int height = 13;
        int width = 17;

        int offset = ((int)Mathf.Ceil((float)height / 2.0f) - 1);

        int left_offset = offset;
        int right_offset = 0;

        Tile_[,] tiles = new Tile_[height, width + offset];

        for (int row = 0; row < height; row++)
        {
            int current_col = 0;
            for (int i = 0; i < left_offset; i++)
            {
                tiles[row, current_col] = new BorderTile();
                current_col++;
            }

            for (int col = 0; col < width; col++)
            {
                if (row == 0 || row == height - 1)
                {
                    tiles[row, current_col] = new BorderTile();
                    
                }
                else if (row == 1 || row == height - 2)
                {
                    if (col == 14 || col == 15)
                    {
                        tiles[row, current_col] = new EmptyTile();
                    } else
                    {
                        tiles[row, current_col] = new BorderTile();
                    }
                    
                }
                else
                {
                    if (col == 0 || col == width - 1 || (col == 1 && row % 2 == 1 && row > 2))
                    {
                        tiles[row, current_col] = new BorderTile();
                    } else
                    {
                        tiles[row, current_col] = new EmptyTile();
                    }
                }
                current_col++;
            }

            for (int j = 0; j < right_offset; j++)
            {
                tiles[row, current_col] = new BorderTile();
                current_col++;
            }

            if (row != 0 && row % 2 == 0)
            {
                left_offset--;
                right_offset++;
            }

            
        }

        return tiles;



    }


}
