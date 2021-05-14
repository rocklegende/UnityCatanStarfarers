using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace com.onebuckgames.UnityStarFarers
{
    [Serializable]
    public class Map : SFModel, Observer
    {
        public List<TileGroup> tileGroups;
        public List<Token> tokensOnMap;
        public List<SpacePoint> AllAvailableSpacePoints;

        private Tile_[,] mapRepresentation;
        private int offset;

        

        public Map(Tile_[,] mapRepresentation, List<TileGroup> tileGroups = null)
        {
            this.mapRepresentation = mapRepresentation;
            this.offset = GetOffset();
            this.tileGroups = tileGroups;
            this.tokensOnMap = new List<Token>();

            if (this.tileGroups != null)
            {
                foreach (var group in this.tileGroups)
                {
                    group.RegisterObserver(this);
                }
            }

            AllAvailableSpacePoints = GetAllAvailableSpacePoints();
        }

        public void UpdateData(Map newMapData)
        {
            tileGroups = newMapData.tileGroups;
            tokensOnMap = newMapData.tokensOnMap;
            AllAvailableSpacePoints = newMapData.AllAvailableSpacePoints;
            offset = newMapData.offset;
            mapRepresentation = newMapData.mapRepresentation;
        }

        void DataChanged()
        {
            Notify(null);
        }

        public void ReObserveTokens()
        {
            foreach(var token in tokensOnMap)
            {
                token.RegisterObserver(this);
            }
        }

        public void AddToken(Token token)
        {
            tokensOnMap.Add(token);
            token.associatedMap = this;
            token.RegisterObserver(this);
            DataChanged();
        }

        public void RemoveToken(Token token)
        {
            tokensOnMap.Remove(token);
            token.associatedMap = null;
            token.RemoveObserver(this);
            DataChanged();
        }

        public void SettleToken(Token token)
        {
            token.settle();
            var tileGroup = FindTileGroupAtPoint(token.position);
            if (tileGroup != null)
            {
                tileGroup.HandleTokenSettled(token);
            }
        }

        /// <summary>
        /// Token is currently on a settle point of a tilegroup
        /// </summary>
        /// <param name="token"></param>
        /// <param name="futurePositionOfToken"></param>
        /// <returns></returns>
        public bool TokenIsBlockingSettlePoint(Token token, SpacePoint futurePositionOfToken = null)
        {
            SpacePoint pointToSearch;
            if (futurePositionOfToken != null)
            {
                pointToSearch = futurePositionOfToken;
            }
            else
            {
                pointToSearch = token.position;
            }

            var tileGroup = FindTileGroupAtPoint(pointToSearch);
            if (tileGroup != null)
            {
                return tileGroup.GetSettlePoints().Contains(pointToSearch);
            }
            return false;
        }

        /// <summary>
        /// Returns true if the given token can currently settle.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="futurePositionOfToken">
        /// Use this if the token will be at that point on the next move.
        /// So if we move that Token in to that position, would it be able to settle there?
        /// </param>
        /// <returns></returns>
        public bool TokenCanSettle(Token token, SpacePoint futurePositionOfToken = null)
        {

            SpacePoint pointToSearch;
            if (futurePositionOfToken != null)
            {
                pointToSearch = futurePositionOfToken;
            } else
            {
                pointToSearch = token.position;
            }


            var tileGroup = FindTileGroupAtPoint(pointToSearch);
            if (tileGroup == null)
            {
                return false;
            }

            try
            {
                var success = tileGroup.RequestSettleOfToken(token, futurePositionOfToken);
                return success;
            }
            catch
            {
                return false;
            }

        }

        bool TilesAreBorders(Tile_ tile1, Tile_ tile2)
        {
            return tile1 is BorderTile && tile2 is BorderTile;
        }

        public Token TokenAtPoint(SpacePoint point)
        {
            return tokensOnMap.Find(tok => tok.position.Equals(point));
        }

        public List<TradeStation> GetTradeStations()
        {
            return tileGroups
                .Where(group => group is TradeStation)
                .Select(entry => (TradeStation)entry)
                .ToList();
        }

        /// <summary>
        /// Returns all neighbor points of given SpacePoint.<br></br>
        /// <b>NOTE: </b>Unreachable points are excluded
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public List<SpacePoint> GetNeighborsOfSpacePoint(SpacePoint point)
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
                return actualValidNeighbors;
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
                return actualValidNeighbors;
            }
            else
            {
                throw new ArgumentException("Vertexnumber has to be between 0 and 1");
            }
        }

        public List<SpacePoint> GetSpacePointsInDistance(SpacePoint origin, int distance)
        {
            return GetSpacePointsInsideRange(origin, distance, distance);
        }

        /// <summary>
        /// Returns list of every point that is inside the given min max range (min and max values are included) from given SpacePoint.
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns>List(SpacePoint)</returns>
        public List<SpacePoint> GetSpacePointsInsideRange(SpacePoint origin, int max, int min = 0)
        {
            return GetSpacePointsInsideRangeWithDistanceMap(origin, max, min).Keys.ToList();
        }

        /// <summary>
        /// Returns distance map of every point that is inside the given min max range (min and max values are included) from given SpacePoint.
        /// Key is SpacePoint, Value is distance from origin.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public Dictionary<SpacePoint, int> GetSpacePointsInsideRangeWithDistanceMap(SpacePoint origin, int max, int min = 0)
        {
            Dictionary<SpacePoint, int> distanceMap = new Dictionary<SpacePoint, int>();

            if (min < 0 || max < 0 || min > max)
            {
                throw new ArgumentException("Please insert distance >= 0");
            }

            if (max == 0 && min == 0)
            {
                distanceMap.Add(origin, 0);
                return distanceMap;
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
                        bool neighborVisited = visitedPoints.Contains(neighbor);
                        if (!neighborVisited)
                        {
                            visitedPoints.Add(neighbor);
                            tracker[steps + 1].Add(neighbor);
                        }
                    }
                }
                steps++;
            }

            for (int distance = min; distance <= max; distance++)
            {
                foreach (var point in tracker[distance])
                {
                    distanceMap.Add(point, distance);
                }
            }
            return distanceMap;
        }

        /// <summary>
        /// Returns the real distance between SpacePoints, including going around obstacles. <br/>
        /// Uses BFS.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
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
                    bool neighborVisited = visitedPoints.Contains(neighbor);
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

        public int GetOffset()
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

        /// <summary>
        /// Returns True if the coords are inside the bounds of the mapRepresentation array.
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public bool coordsAreInBounds(HexCoordinates coords)
        {
            var rIsValid = coords.r < height() && coords.r > -1;
            var qIsValid = coords.q + offset > -1 && coords.q + offset < width();
            return rIsValid && qIsValid;
        }

        /// <summary>
        /// Returns True if the coords are inside the bounds of the mapRepresentation array and is not border or null tile.
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public bool coordsAreValid(HexCoordinates coords)
        {
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

            group.RegisterObserver(this);
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
            if (token.position != null)
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
                        }
                        catch (Exception e)
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


        public List<SpacePoint> applyFilter(List<SpacePoint> points, SpacePointFilter filter)
        {
            List<SpacePoint> validPoints = new List<SpacePoint>();

            foreach (SpacePoint point in points)
            {
                if (filter.pointFulfillsFilter(point, this))
                {
                    validPoints.Add(point);
                }
            }

            return validPoints;
        }

        public List<SpacePoint> GetSpacePointsFullfillingFilters(List<SpacePointFilter> filters)
        {
            List<SpacePoint> points = AllAvailableSpacePoints; //GetAllAvailableSpacePoints();
            foreach (var filter in filters)
            {
                points = applyFilter(points, filter);
            }
            return points;
        }

        /// <summary>
        /// Returns all theoretically possible SpacePoints (also including SpacePoints that are not reachable).
        /// </summary>
        /// <returns></returns>
        List<SpacePoint> GetAllAvailableSpacePoints()
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

            return positions;
        }

        public void SubjectDataChanged(object[] data)
        {
            if (data != null)
            {
                if (data.Length > 0)
                {
                    if (data[0] is Token)
                    {
                        OnTokenDataChanged((Token)data[0]);
                    }
                }
            } 
            DataChanged();
            //delegate changes up the chain, if something changes inside
            //the tilegroup or tokens than just send a single message that something in the map data changed
        
        }
    }

}