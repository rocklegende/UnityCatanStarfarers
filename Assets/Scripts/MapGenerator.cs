using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapGenerator
{
    SpacePoint[] basePoints;
    SpacePoint[] randomSpawnPoints;
    List<TileGroup> spaceTileGroups;
    List<TileGroup> spaceTileGroupsCopy;
    List<ResourceTileGroup> baseTileGroupsCopy;
    Map map;
    CircleChipGroup circleGroup;
    TriangleChipGroup triangleGroup;
    SquareChipGroup squareGroup;
    private bool isRandom = false;

    public MapGenerator()
    {

        CreateChipGroups();
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

        spaceTileGroups = new List<TileGroup>();
        spaceTileGroups.Add(new ResourceTileGroup(new ResourceTile[] { new GoodsResourceTile(circleGroup), new GoodsResourceTile(squareGroup), new OreResourceTile(triangleGroup) }));
        spaceTileGroups.Add(new ResourceTileGroup(new ResourceTile[] { new GoodsResourceTile(circleGroup), new GoodsResourceTile(squareGroup), new OreResourceTile(triangleGroup) }));
        spaceTileGroups.Add(new ResourceTileGroup(new ResourceTile[] { new GoodsResourceTile(circleGroup), new GoodsResourceTile(squareGroup), new OreResourceTile(triangleGroup) }));
        spaceTileGroups.Add(new ResourceTileGroup(new ResourceTile[] { new GoodsResourceTile(circleGroup), new GoodsResourceTile(squareGroup), new OreResourceTile(triangleGroup) }));
        spaceTileGroups.Add(new OrzelTradeStation());
        spaceTileGroups.Add(new KhornemTradeStation());
        spaceTileGroups.Add(new RahnaTradeStation());
        spaceTileGroups.Add(new ResourceTileGroup(new ResourceTile[] { new FuelResourceTile(circleGroup), new GoodsResourceTile(squareGroup), new OreResourceTile(triangleGroup) }));
        spaceTileGroups.Add(new ResourceTileGroup(new ResourceTile[] { new FuelResourceTile(circleGroup), new GoodsResourceTile(squareGroup), new OreResourceTile(triangleGroup) }));
        spaceTileGroups.Add(new ResourceTileGroup(new ResourceTile[] { new FuelResourceTile(circleGroup), new GoodsResourceTile(squareGroup), new OreResourceTile(triangleGroup) }));
        spaceTileGroups.Add(new ResourceTileGroup(new ResourceTile[] { new FuelResourceTile(circleGroup), new GoodsResourceTile(squareGroup), new OreResourceTile(triangleGroup) }));
        spaceTileGroups.Add(new AxxaTradeStation());
        spaceTileGroups.Add(new ResourceTileGroup(new ResourceTile[] { new FuelResourceTile(circleGroup), new GoodsResourceTile(squareGroup), new OreResourceTile(triangleGroup) }));
        spaceTileGroups.Add(new ResourceTileGroup(new ResourceTile[] { new FuelResourceTile(circleGroup), new GoodsResourceTile(squareGroup), new OreResourceTile(triangleGroup) }));
        spaceTileGroups.Add(new ResourceTileGroup(new ResourceTile[] { new FuelResourceTile(circleGroup), new GoodsResourceTile(squareGroup), new OreResourceTile(triangleGroup) }));
        spaceTileGroups.Add(new ResourceTileGroup(new ResourceTile[] { new FuelResourceTile(circleGroup), new GoodsResourceTile(squareGroup), new OreResourceTile(triangleGroup) }));
        spaceTileGroups.Add(new ResourceTileGroup(new ResourceTile[] { new FuelResourceTile(circleGroup), new GoodsResourceTile(squareGroup), new OreResourceTile(triangleGroup) }));

        spaceTileGroupsCopy = new Helper().CreateCopyOfList(spaceTileGroups);
    }

    void CreateChipGroups()
    {
        circleGroup = new CircleChipGroup(GetBasicDiceChips());
        squareGroup = new SquareChipGroup(GetBasicDiceChips());
        triangleGroup = new TriangleChipGroup(GetOnlyPirateToken());
    }

    // Debug method
    List<DiceChip> GetOnlyPirateToken()
    {
        var list = new List<DiceChip>();

        for (int i = 0; i < 30; i++)
        {
            list.Add(new PirateToken(new FreightPodsBeatCondition(4)));
        }


        return list;
    }

    List<DiceChip> GetBasicDiceChips()
    {
        var list = new List<DiceChip>();

        for (int i = 0; i < 30; i++)
        {
            list.Add(new DiceChip3());
        }

        list.Add(new PirateToken(new FreightPodsBeatCondition(4)));
        list.Add(new PirateToken(new FreightPodsBeatCondition(3)));

        return list;
    }

    public Map GenerateRandomMap() {
        Tile_[,] raw = RawMap();

        map = new Map(raw);
        SetupBaseColonies();
        SetupSpaceColonies();
        spaceTileGroupsCopy.AddRange(baseTileGroupsCopy);
        map.tileGroups = spaceTileGroupsCopy.ToArray();

        return map;
    }

    void RotateRandomSpawnTileGroups() {
        foreach (TileGroup tg in spaceTileGroups)
        {
            tg.ShiftTiles(ThreadSafeRandom.ThisThreadsRandom.Next(tg.GetTiles().Length));
        }
    }

    List<ResourceTileGroup> GetTileGroupsBasePoints()
    {
        var groups = new List<ResourceTileGroup> {
            new ResourceTileGroup(new ResourceTile[] { new FuelResourceTile(circleGroup), new GoodsResourceTile(squareGroup), new OreResourceTile(triangleGroup) }),
            new ResourceTileGroup(new ResourceTile[] { new OreResourceTile(circleGroup), new CarbonResourceTile(squareGroup), new FoodResourceTile(triangleGroup) }),
            new ResourceTileGroup(new ResourceTile[] { new OreResourceTile(circleGroup), new FuelResourceTile(squareGroup), new GoodsResourceTile(triangleGroup) }),
            new ResourceTileGroup(new ResourceTile[] { new FoodResourceTile(circleGroup), new FuelResourceTile(squareGroup), new CarbonResourceTile(triangleGroup) })
        };

        return groups;
    }


    void SetupBaseColonies()
    {
        var baseGroups = GetTileGroupsBasePoints();
        baseTileGroupsCopy = new Helper().CreateCopyOfList(baseGroups);
        AssignDiceChips(baseGroups);

        for (int i = 0; i < baseGroups.Count; i++)
        {
            baseGroups[i].SetCenter(basePoints[i]);
            baseGroups[i].RevealDiceChips();
            map.SetTileGroupAtSpacePoint(baseGroups[i], basePoints[i]);
            
        }
    }

    void SetupSpaceColonies()
    {
        if (isRandom)
        {
            RotateRandomSpawnTileGroups();
        }

        foreach (SpacePoint point in randomSpawnPoints)
        {
            if (isRandom)
            {
                spaceTileGroups.Shuffle();
            }
            TileGroup tg = spaceTileGroups[0];
            tg.SetCenter(point);
            spaceTileGroups.RemoveAt(0);

            if (tg is ResourceTileGroup)
            {
                AssignDiceChips(new List<ResourceTileGroup>() { (ResourceTileGroup) tg });
            }
            
            map.SetTileGroupAtSpacePoint(tg, point);
        }
    }

    void AssignDiceChips(List<ResourceTileGroup> resourceTileGroups)
    {
        foreach (var group in resourceTileGroups)
        {
            foreach (ResourceTile resourceTile in group.GetTiles())
            {
                try
                {
                    var chip = resourceTile.chipGroup.RetrieveChip();
                    resourceTile.SetDiceChip(chip);
                }
                catch (ArgumentException e)
                {
                    Debug.Log("Something went wrong while assigning dicechips to the resource tiles: " + e.Message);
                }
            
            }
        }
    }
    /// <summary>
    /// Generates basic map with empty tiles and border tiles. 
    /// </summary>
    /// <returns></returns>
    Tile_[,] RawMap() {

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
