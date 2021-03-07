using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Map
{

    private Tile_[,] mapRepresentation;
    private int offset;
    private MapScript changeDelegate;
    public TileGroup[] tileGroups;

    public Map(Tile_[,] mapRepresentation, MapScript changeDelegate = null, TileGroup[] tileGroups = null)
    {
        this.mapRepresentation = mapRepresentation;
        this.offset = getOffset();
        this.changeDelegate = changeDelegate;
        this.tileGroups = tileGroups;
    }

    void DataChanged()
    {
        var notifier = new SFElement();
        notifier.app.Notify(SFNotification.map_data_changed, notifier); 
    }

    bool TilesAreBorders(Tile_ tile1, Tile_ tile2)
    {
        return tile1 is BorderTile && tile2 is BorderTile;
    }

    bool GetPointsTokenCanFlyTo(Token token, int steps)
    {
        throw new NotImplementedException("This functionality is not implemented yet");
        //TODO: return all the points this token can fly to
    }

    public Token[] TokenAtPoint(SpacePoint point, Player[] players)
    {
        List<Token> tokenList = new List<Token>();
        foreach(var tok in new Helper().GetAllTokenOfPlayers(players))
        {
            if (tok.position.Equals(point))
            {
                tokenList.Add(tok);
            }
        }

        return tokenList.ToArray();
    }

    public SpacePoint[] GetNeighborsOfSpacePoint(SpacePoint point)
    {
        var actualValidNeighbors = new List<SpacePoint>();
        var coords = point.coordinates;

        var e_coords = coords.E();
        var ne_coords = coords.NE();
        var s_coords = coords;
        var nw_coords = coords.NW();

        if (point.vertexNumber == 0)
        {
            var n_point = new SpacePoint(ne_coords, 1);
            var sw_point = new SpacePoint(s_coords, 1);
            var se_point = new SpacePoint(e_coords, 1);

            var nw_tile = getTileAt(nw_coords);
            var ne_tile = getTileAt(ne_coords);
            var s_tile = getTileAt(s_coords);

            if (!TilesAreBorders(ne_tile, nw_tile)) {
                if (ne_tile != null && nw_tile != null && !(ne_tile.blocksTraffic() && nw_tile.blocksTraffic()))
                {
                    actualValidNeighbors.Add(n_point);
                }
            }

            if (!TilesAreBorders(ne_tile, s_tile))
            {
                if (ne_tile != null && s_tile != null && !(ne_tile.blocksTraffic() && s_tile.blocksTraffic()))
                {
                    actualValidNeighbors.Add(se_point);
                }       
            }

            if (!TilesAreBorders(nw_tile, s_tile))
            {
                if (nw_tile != null && s_tile != null && !(nw_tile.blocksTraffic() && s_tile.blocksTraffic()))
                {
                    actualValidNeighbors.Add(sw_point);
                }
            }   
            return actualValidNeighbors.ToArray();
        }
        else if (point.vertexNumber == 1)
        {
            var s_point = new SpacePoint(coords.SW(), 0);
            var ne_point = new SpacePoint(coords, 0);
            var nw_point = new SpacePoint(coords.W(), 0);

            var rechts_unten_tile = getTileAt(coords);
            var links_unten_tile = getTileAt(coords.W());
            var oben_tile = getTileAt(coords.NW());

            if (!TilesAreBorders(rechts_unten_tile, links_unten_tile))
            {
                if (rechts_unten_tile != null && links_unten_tile != null && !(rechts_unten_tile.blocksTraffic() && links_unten_tile.blocksTraffic()))
                    {
                        actualValidNeighbors.Add(s_point);
                    }
            }

            if (!TilesAreBorders(links_unten_tile, oben_tile))
            {
                if (links_unten_tile != null && oben_tile != null && !(links_unten_tile.blocksTraffic() && oben_tile.blocksTraffic()))
                {
                    actualValidNeighbors.Add(nw_point);
                }
            }

            if (!TilesAreBorders(rechts_unten_tile, oben_tile))
            {
                if (rechts_unten_tile != null && oben_tile != null && !(rechts_unten_tile.blocksTraffic() && oben_tile.blocksTraffic()))
                {
                    actualValidNeighbors.Add(ne_point);
                }
            }
            return actualValidNeighbors.ToArray();
        }
        else
        {
            throw new ArgumentException("Vertexnumber has to be between 0 and 1");
        }
    }

    public SpacePoint[] GetSpacePointsInDistance(SpacePoint origin, int distance)
    {
        return GetSpacePointsInsideRange(origin, distance, distance);
    }

    public SpacePoint[] GetSpacePointsInsideRange(SpacePoint origin, int max, int min = 0)
        //returns every point that is <= 'range' away from given SpacePoint
    {
        if (min < 0 || max < 0 || min > max)
        {
            throw new ArgumentException("Please insert distance >= 0");
        }

        if (max == 0 && min == 0)
        {
            return new SpacePoint[] { origin };
        }

        List<SpacePoint> visitedPoints = new List<SpacePoint>();
        List<List<SpacePoint>> tracker = new List<List<SpacePoint>>();
        int steps = 0;
        tracker.Add(new List<SpacePoint>() { origin });
        visitedPoints.Add(origin);

        while (steps < max)
        {
            tracker.Add(new List<SpacePoint>());
            foreach (var point in tracker[steps])
            {

                var neighbors = GetNeighborsOfSpacePoint(point);
                foreach (SpacePoint neighbor in neighbors)
                {
                    bool neighborVisited = new Helper().SpacePointArrayContainsPoint(visitedPoints.ToArray(), neighbor);
                    if (!neighborVisited)
                    {
                        visitedPoints.Add(neighbor);
                        tracker[steps + 1].Add(neighbor);
                    }
                }
            }
            steps++;
        }

        List<SpacePoint> points = new List<SpacePoint>();
        for (int i = min; i <= max; i++)
        {
            points.AddRange(tracker[i]);
        }
        return points.ToArray(); 
    }

    public int distanceBetweenPoints(SpacePoint origin, SpacePoint destination)
    {
        // TODO: seems broken for large distances
        if (origin.Equals(destination))
        {
            return 0;
        }
        var queue = new Queue<SpacePoint>();
        queue.Enqueue(origin);
        var helper = new Helper();
        int count = 0;
        List<SpacePoint> visitedPoints = new List<SpacePoint>();

        while (queue.Count != 0)
        {
            var point = queue.Dequeue();
            visitedPoints.Add(point);
            var neighbors = GetNeighborsOfSpacePoint(point);
            foreach (SpacePoint neighbor in neighbors)
            {
                if (neighbor.Equals(destination))
                {
                    //we found our point!
                    neighbor.prev = point;
                    var temp = neighbor;
                    int dist = 0;
                    while (temp.prev != null)
                    {
                        temp = temp.prev;
                        dist += 1;
                    }
                    return dist;
                }
                bool neighborVisited = helper.SpacePointArrayContainsPoint(visitedPoints.ToArray(), neighbor);
                if (!neighborVisited)
                {
                    visitedPoints.Add(neighbor);
                    neighbor.prev = point;
                    queue.Enqueue(neighbor);
                }
            }
            count++;
            if (count > 1000)
            {
                break;
            }

        }
        return -1;
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
        if (coordsAreInBounds(coords))
        {
            (int, int) arrayIndexes = coordsToArrayIndexes(coords);
            return mapRepresentation[arrayIndexes.Item1, arrayIndexes.Item2];
        } else
        {
            return null;
        }
    }

    public T[] GetTilesOfType<T>() where T : Tile_
    {
        List<T> validTiles = new List<T>();
        foreach (Tile_ tile in mapRepresentation)
        {
            if (tile is T)
            {
                validTiles.Add((T)tile);
            }
        }

        return validTiles.ToArray();
    }

    public T[] GetTilesOfType<T>(Tile_[] tiles) where T : Tile_
    {
        List<T> list = new List<T>();
        foreach (Tile_ tile in tiles)
        {
            if (tile is T)
            {
                list.Add((T)tile);
            }
        }

        return list.ToArray();
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

    public TileGroup FindTileGroupAtPoint(SpacePoint point)
    {
        foreach(var tile in getTilesAtPoint(point))
        {
            var tileGroup = FindTileGroupContainingTile(tile);
            if (tileGroup != null)
            {
                return tileGroup;
            }
        }
        return null;
    }

    public void OnTokenDataChanged(Token token)
    {
        var newPositionOfToken = token.position;
        var tilesAtPoint = getTilesAtPoint(newPositionOfToken);

        foreach (var tile in tilesAtPoint)
        {
            var tileGroup = FindTileGroupContainingTile(tile);
            var canSettle = false;
            var foundTileGroup = false;
            if (tileGroup != null)
            {
                tileGroup.OnTokenEnteredArea(token);
                foundTileGroup = true;
                try
                {
                    canSettle = tileGroup.RequestSettleOfToken(token);
                } catch (Exception e)
                {
                    Debug.Log("Token cannot settle: " + e.GetType());
                }
            }

            NotifyThatTokenCanSettle(token, canSettle);

            if (foundTileGroup)
            {
                break;
            }
        }
    }

    void NotifyThatTokenCanSettle(Token token, bool canSettle)
    {
        var notifier = new SFElement();
        notifier.app.Notify(SFNotification.token_can_settle, notifier, new object[] { canSettle, token });
    }

    public TileGroup FindTileGroupContainingTile(Tile_ tile)
    {
        if (tileGroups != null)
        {
            foreach(var group in tileGroups)
            {
                foreach (var t in group.GetTiles())
                {
                    if (t == tile)
                    {
                        return group;
                    }
                    
                }
            }
        }
        return null;
    }


    public SpacePoint[] applyFilter(SpacePoint[] points, SpacePointFilter filter, Player[] players)
    {
        List<SpacePoint> validPoints = new List<SpacePoint>();

        foreach (SpacePoint point in points)
        {
            if (filter.pointFulfillsFilter(point, this, players))
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
