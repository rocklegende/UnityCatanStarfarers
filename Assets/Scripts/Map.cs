using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Tile
{
    NULL,
    BORDER,
    VALID
};

public class Map
{

    private Tile[,] mapRepresentation;
    private int offset;

    public Map(Tile[,] mapRepresentation)
    {
        this.mapRepresentation = mapRepresentation;
        this.offset = getOffset();
    }

    public int getOffset()
    {
        return ((int)Mathf.Ceil((float)height() / 2.0f) - 1);
    }

    public Tile getTileAt(HexCoordinates coords)
    {
        return mapRepresentation[coords.r, coords.q + offset];
    }

    public bool coordsAreInBounds(HexCoordinates coords)
    {
        var rIsValid = coords.r < height() - 1 && coords.r > -1;
        var qIsValid = coords.q + offset > -1 && coords.q + offset < width() - 1;
        return rIsValid && qIsValid;
    }

    public bool coordsAreValid(HexCoordinates coords)
    {
        if (!coordsAreInBounds(coords))
        {
            return false;
        } else
        {
            return getTileAt(coords) == Tile.VALID;
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
                    if (getTileAt(coordinates) == Tile.VALID)
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

    public HexCoordinates[] getHexesAtPoint(SpacePoint spacePoint)
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


    public SpacePoint[] getAllAvailableSpacePoints()
    {
        List<SpacePoint> positions = new List<SpacePoint>();
        HexCoordinates[] allHexCoordinates = getAllHexCoordinates();

        foreach (HexCoordinates coordinates in allHexCoordinates)
        {
            var firstPosition = new SpacePoint(coordinates, 0);
            var secondPosition = new SpacePoint(coordinates, 1);
            switch (getTileAt(coordinates))
            {
                case Tile.BORDER:
                    HexCoordinates westNeighborCoords = coordinates.getNeighborCoordinates(Direction.W);
                    if (coordsAreInBounds(westNeighborCoords))
                    {
                        if (getTileAt(westNeighborCoords) == Tile.VALID)
                        {
                            positions.Add(secondPosition);
                        }
                    }

                    HexCoordinates northwestNeighborCoords = coordinates.getNeighborCoordinates(Direction.NW);
                    if (coordsAreInBounds(northwestNeighborCoords))
                    {
                        if (getTileAt(northwestNeighborCoords) == Tile.VALID)
                        {
                            positions.Add(firstPosition);
                            positions.Add(secondPosition);
                        }
                    }

                    HexCoordinates NE_NeighborCoords = coordinates.getNeighborCoordinates(Direction.NE);
                    if (coordsAreInBounds(NE_NeighborCoords))
                    {
                        if (getTileAt(NE_NeighborCoords) == Tile.VALID)
                        {
                            positions.Add(firstPosition);
                        }
                    }

                    break;
                case Tile.VALID:
                    positions.Add(firstPosition);
                    positions.Add(secondPosition);
                    break;
                case Tile.NULL:
                    // do nothing
                    break;
            }
        }

        return positions.ToArray();
    }
}
