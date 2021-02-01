using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class SpacePointFilter
{
    public abstract bool pointFulfillsFilter(SpacePoint point, Map map);
}

public class Map
{

    private Tile_[,] mapRepresentation;
    private int offset;
    private MapScript changeDelegate;

    public Map(Tile_[,] mapRepresentation, MapScript changeDelegate = null)
    {
        this.mapRepresentation = mapRepresentation;
        this.offset = getOffset();
        this.changeDelegate = changeDelegate;
    }

    public Tile_[,] getRepresentation()
    {
        return this.mapRepresentation;
    }

    public int getOffset()
    {
        return ((int)Mathf.Ceil((float)height() / 2.0f) - 1);
    }

    public (int, int) coordsToArrayIndexes(HexCoordinates coords)
    {
        return (coords.r, coords.q + offset);
    }

    public HexCoordinates  arrayIndexesToCoords((int, int) indexes)
    {
        return new HexCoordinates(indexes.Item2 - offset, indexes.Item1);
    }

    public Tile_ getTileAt(HexCoordinates coords)
    {
        (int, int) arrayIndexes = coordsToArrayIndexes(coords);
        return mapRepresentation[arrayIndexes.Item1, arrayIndexes.Item2];
    }

    public Tile_[] getTilesAtPoint(SpacePoint point)
    {
        List<Tile_> tiles = new List<Tile_>();
        HexCoordinates[] hexCoords = getValidHexCoordinatesAtPoint(point);
        foreach (HexCoordinates coords in hexCoords)
        {
            Tile_ tile = getTileAt(coords);
            tiles.Add(tile);
        }
        return tiles.ToArray();
    }

    public bool coordsAreInBounds(HexCoordinates coords)
    {
        /*
         * Returns True if the coords are inside the bounds of the mapRepresentation array
         */

        var rIsValid = coords.r < height() && coords.r > -1;
        var qIsValid = coords.q + offset > -1 && coords.q + offset < width();
        return rIsValid && qIsValid;
    }

    public bool coordsAreValid(HexCoordinates coords)
    {
        /*
         * Returns True if the coords are inside the bounds of the mapRepresentation array and is not border or null tile/
         */
        if (!coordsAreInBounds(coords))
        {
            return false;
        } else
        {
            return !(getTileAt(coords) is NullTile) && !(getTileAt(coords) is BorderTile);
        }
    }

    public int width()
    {
        return mapRepresentation.GetLength(1);
    }

    public int height()
    {
        return mapRepresentation.GetLength(0);
    }

    public void SetTileGroupAtSpacePoint(TileGroup group, SpacePoint point)
    {
        // look if we get 3 valid hex coordinates at this point, if not then we cannot set the TileGroup here
        HexCoordinates[] coordinates = getValidHexCoordinatesAtPoint(point);
        
        if (coordinates.Length != 3)
        {
            throw new ArgumentException("Can't set TileGroup at this point, because we don't have 3 valid hexes");
        }

        for (int i = 0; i < group.GetTiles().Length; i++)
        {
            (int, int) indexes = coordsToArrayIndexes(coordinates[i]);
            mapRepresentation[indexes.Item1, indexes.Item2] = group.GetTiles()[i];
        }

        RepresentationChanged();

    }

    public void RepresentationChanged()
    {

        //TODO: use notification system
        if (changeDelegate != null)
        {
            this.changeDelegate.RepresentationChanged();
        }
        
    }

    public SpacePoint[] getAllSpacePointsInDistance(SpacePoint origin, int distance)
    {
        SpacePoint[] allPointsOnMap = getAllAvailableSpacePoints();
        List<SpacePoint> result = new List<SpacePoint>();

        foreach (SpacePoint point in allPointsOnMap)
        {
            if (point.DistanceTo(origin) == distance)
            {
                result.Add(point);
            }
        }
        return result.ToArray();
    }

    public HexCoordinates[] getAllHexCoordinates(bool onlyValidTiles = false)
    {
        List<HexCoordinates> allCoords = new List<HexCoordinates>();
        for (int q = -offset; q < width() - offset; q++)
        {
            for (int r = 0; r < height(); r++)
            {
                var coordinates = new HexCoordinates(q, r);
                if (onlyValidTiles)
                {

                    if (coordsAreValid(coordinates))
                    {
                        allCoords.Add(coordinates);
                    }

                }
                else
                {
                    allCoords.Add(coordinates);
                }

            }
        }
        return allCoords.ToArray();
    }

    public HexCoordinates[] getValidHexCoordinatesAtPoint(SpacePoint spacePoint)
    {
        Helper helper = new Helper();
        HexCoordinates[] allHexCoordinates = helper.getCoordinatesOfHexesAtPoint(spacePoint);

        List<HexCoordinates> validCoords = new List<HexCoordinates>();
        foreach(HexCoordinates coords in allHexCoordinates)
        {
            if (coordsAreValid(coords))
            {
                validCoords.Add(coords);
            }
        }


        return validCoords.ToArray();
    }


    public SpacePoint[] applyFilter(SpacePoint[] points, SpacePointFilter filter)
    {
        List<SpacePoint> validPoints = new List<SpacePoint>();

        foreach (SpacePoint point in points)
        {
            if (filter.pointFulfillsFilter(point, this))
            {
                validPoints.Add(point);
            }
        }

        return validPoints.ToArray();
    }


    public SpacePoint[] getAllAvailableSpacePoints()
    {
        List<SpacePoint> positions = new List<SpacePoint>();
        HexCoordinates[] allHexCoordinates = getAllHexCoordinates();

        foreach (HexCoordinates coordinates in allHexCoordinates)
        {
            var firstPosition = new SpacePoint(coordinates, 0);
            var secondPosition = new SpacePoint(coordinates, 1);
            Tile_ tile = getTileAt(coordinates);

            if (tile is BorderTile)
            {
                HexCoordinates westNeighborCoords = coordinates.getNeighborCoordinates(Direction.W);

                if (coordsAreValid(westNeighborCoords))
                {
                    positions.Add(secondPosition);
                }

                HexCoordinates northwestNeighborCoords = coordinates.getNeighborCoordinates(Direction.NW);
                if (coordsAreValid(northwestNeighborCoords))
                {
                    positions.Add(firstPosition);
                    positions.Add(secondPosition);
                }

                HexCoordinates NE_NeighborCoords = coordinates.getNeighborCoordinates(Direction.NE);
                if (coordsAreValid(NE_NeighborCoords))
                {
                    positions.Add(firstPosition);
                }
            }

            else { 
                // tile is valid
                positions.Add(firstPosition);
                positions.Add(secondPosition);
            }
        }

        return positions.ToArray();
    }
}
