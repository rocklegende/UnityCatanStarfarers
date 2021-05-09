using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using com.onebuckgames.UnityStarFarers;

namespace Tests
{
    public class MapTests
    {
        private Helper helper = new Helper();
        private CircleChipGroup circle = new CircleChipGroup(new List<DiceChip>());

        private Map CreateSampleMap3x3()
        {
            Tile_[,] mapRepresentation = {
                { new BorderTile(), new BorderTile(), new BorderTile() },
                { new BorderTile(), new FoodResourceTile(circle), new BorderTile() },
                { new BorderTile(), new BorderTile(), new BorderTile() },
            };

            return new Map(mapRepresentation);
        }

        private Map CreateSampleMap4x3()
        {
            Tile_[,] mapRepresentation = {
                { new BorderTile(), new FoodResourceTile(circle), new FoodResourceTile(circle), new BorderTile() },
                { new BorderTile(), new FoodResourceTile(circle), new FoodResourceTile(circle), new BorderTile() },
                { new FoodResourceTile(circle), new FoodResourceTile(circle), new BorderTile(), new BorderTile() },
            };

            return new Map(mapRepresentation);
        }

        private Map CreateSampleMapWithObstacles()
        {
            Tile_[,] mapRepresentation = {
                { new BorderTile(), new EmptyTile(), new FoodResourceTile(circle), new BorderTile() },
                { new BorderTile(), new FoodResourceTile(circle), new FoodResourceTile(circle), new BorderTile() },
                { new FoodResourceTile(circle), new FoodResourceTile(circle), new BorderTile(), new BorderTile() },
            };

            return new Map(mapRepresentation);
        }

        private Map CreateSampleMapWithObstacles2()
        {
            Tile_[,] mapRepresentation = {
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile()  },
                { new BorderTile(), new EmptyTile(), new FoodResourceTile(circle), new EmptyTile(),  new BorderTile() },
                { new BorderTile(), new EmptyTile(), new FoodResourceTile(circle), new EmptyTile(), new BorderTile() },
                { new FoodResourceTile(circle), new EmptyTile(), new EmptyTile(), new EmptyTile(), new BorderTile() },
            };

            return new Map(mapRepresentation);
        }

        private Map CreateSampleMapWithObstacles3()
        {
            Tile_[,] mapRepresentation = {
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile()  },
                { new BorderTile(), new BorderTile(), new FoodResourceTile(circle), new EmptyTile(),  new BorderTile() },
                { new BorderTile(), new FoodResourceTile(circle), new FoodResourceTile(circle), new BorderTile(), new BorderTile() },
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile() },
            };

            return new Map(mapRepresentation);
        }

        private Map CreateSampleMap4x4()
        {
            Tile_[,] mapRepresentation = {
                { new BorderTile(), new FoodResourceTile(circle), new FoodResourceTile(circle), new BorderTile() },
                { new BorderTile(), new FoodResourceTile(circle), new FoodResourceTile(circle), new BorderTile() },
                { new FoodResourceTile(circle), new FoodResourceTile(circle), new BorderTile(), new BorderTile() },
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile() },
            };

            return new Map(mapRepresentation);
        }

        private Map CreateSampleMap4x5()
        {
            Tile_[,] mapRepresentation = {
                { new BorderTile(), new FoodResourceTile(circle), new FoodResourceTile(circle), new BorderTile() },
                { new BorderTile(), new FoodResourceTile(circle), new FoodResourceTile(circle), new BorderTile() },
                { new FoodResourceTile(circle), new FoodResourceTile(circle), new BorderTile(), new BorderTile() },
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile() },
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile() },
            };

            return new Map(mapRepresentation);
        }

        private Map CreateSampleMap5x6()
        {
            Tile_[,] mapRepresentation = {
                { new BorderTile(), new FoodResourceTile(circle), new FoodResourceTile(circle), new BorderTile(), new BorderTile() },
                { new BorderTile(), new FoodResourceTile(circle), new FoodResourceTile(circle), new BorderTile(), new BorderTile() },
                { new FoodResourceTile(circle), new FoodResourceTile(circle), new BorderTile(), new BorderTile(), new BorderTile(), },
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile() },
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile() },
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile()},
            };

            return new Map(mapRepresentation);
        }


        private bool spacePointArraysAreEqual(List<SpacePoint> spacePointsList1, List<SpacePoint> spacePointsList2)
        {
            var spacePoints1 = spacePointsList1.ToArray();
            var spacePoints2 = spacePointsList2.ToArray();

            if (spacePoints1.Length != spacePoints2.Length)
            {
                return false;
            }
            Array.Sort(spacePoints1, new SpacePointComparer());
            Array.Sort(spacePoints2, new SpacePointComparer());

            for (int i = 0; i < spacePoints1.Length; i++)
            {
                if (!spacePoints1[i].IsEqualTo(spacePoints2[i]))
                {
                    return false;
                } 
            }

            return true;
        }

        private void printSpacePoints(List<SpacePoint> spacePoints)
        {
            foreach(SpacePoint spacePoint in spacePoints)
            {
                //spacePoint.print();
            }
        }

        [Test]
        public void MapGetNeighborsOfSpacePoint1()
        {
            Map map = CreateSampleMapWithObstacles();

            var origin = new SpacePoint(new HexCoordinates(0, 1), 0);
            List<SpacePoint> neighbors = map.GetNeighborsOfSpacePoint(origin);

            var se_point = new SpacePoint(new HexCoordinates(1, 1), 1);
            var n_point = new SpacePoint(new HexCoordinates(1, 0), 1);
            var sw_point = new SpacePoint(new HexCoordinates(0, 1), 1);

            Assert.False(neighbors.Contains(se_point)); // should be blocked
            Assert.True(neighbors.Contains(n_point)); // should NOT be blocked
            Assert.True(neighbors.Contains(sw_point)); // should NOT be blocked
        }

        [Test]
        public void MapGetNeighborsOfSpacePoint2()
        {
            Map map = CreateSampleMapWithObstacles();

            var origin = new SpacePoint(new HexCoordinates(1, 1), 1);
            List<SpacePoint> neighbors = map.GetNeighborsOfSpacePoint(origin);

            var s_point = new SpacePoint(new HexCoordinates(0, 2), 0);
            var ne_point = new SpacePoint(new HexCoordinates(1, 1), 0);
            var nw_point = new SpacePoint(new HexCoordinates(0, 1), 0);

            Assert.False(neighbors.Contains(s_point)); // should be blocked
            Assert.False(neighbors.Contains(ne_point)); // should be blocked
            Assert.False(neighbors.Contains(nw_point)); // should be blocked
        }

        [Test]
        public void MapGetNeighborsOfSpacePoint3()
        {
            Map map = CreateSampleMapWithObstacles();

            var origin = new SpacePoint(new HexCoordinates(0, 1), 1);
            List<SpacePoint> neighbors = map.GetNeighborsOfSpacePoint(origin);

            var s_point = new SpacePoint(new HexCoordinates(-1, 2), 0);
            var ne_point = new SpacePoint(new HexCoordinates(0, 1), 0);
            var nw_point = new SpacePoint(new HexCoordinates(-1, 1), 0);

            Assert.True(neighbors.Contains(s_point)); // should be blocked
            Assert.True(neighbors.Contains(ne_point)); // should be blocked
            Assert.True(neighbors.Contains(nw_point)); // should be blocked
        }

        [Test]
        public void MapGetDistanceBetweenPoints1()
        {
            Map map = CreateSampleMapWithObstacles2();

            var origin = new SpacePoint(new HexCoordinates(1, 2), 1);
            var destination = new SpacePoint(new HexCoordinates(1, 2), 0);
            Assert.AreEqual(map.distanceBetweenPoints(origin, destination), 5);
            Assert.AreEqual(map.distanceBetweenPoints(destination, origin), 5);
        }

        [Test]
        public void MapGetDistanceBetweenPoints2()
        {
            Map map = CreateSampleMapWithObstacles2();

            var origin = new SpacePoint(new HexCoordinates(1, 1), 1);
            var destination = new SpacePoint(new HexCoordinates(1, 2), 0);
            Assert.AreEqual(map.distanceBetweenPoints(origin, destination), 3);
            Assert.AreEqual(map.distanceBetweenPoints(destination, origin), 3);
        }

        [Test]
        public void MapGetAllAvailableSpacePoints()
        {

            Map map = CreateSampleMap3x3();

            List<SpacePoint> actualPoints = map.AllAvailableSpacePoints;

            List<SpacePoint> expectedPoints = new List<SpacePoint>
            {
                new SpacePoint(new HexCoordinates(0, 1), 0),
                new SpacePoint(new HexCoordinates(0, 1), 1),
                new SpacePoint(new HexCoordinates(1, 1), 1),
                new SpacePoint(new HexCoordinates(0, 2), 0),
                new SpacePoint(new HexCoordinates(0, 2), 1),
                new SpacePoint(new HexCoordinates(-1, 2), 0)
            };

            Assert.True(spacePointArraysAreEqual(actualPoints, expectedPoints));
        }

        [Test]
        public void MapGetAllSpacePointsInDistance2()
        {

            Map map = CreateSampleMapWithObstacles3();

            SpacePoint origin = new SpacePoint(new HexCoordinates(1, 1), 0);
            List<SpacePoint> spacePointsTwoStepsAway = map.GetSpacePointsInDistance(origin, 2);

            List<SpacePoint> expectedPoints = new List<SpacePoint>
            {
                new SpacePoint(new HexCoordinates(0, 2), 0),
                new SpacePoint(new HexCoordinates(1, 2), 0),
                new SpacePoint(new HexCoordinates(2, 1), 0),
            }; 

            Assert.True(spacePointArraysAreEqual(spacePointsTwoStepsAway, expectedPoints));
        }

        [Test]
        public void MapGetAllSpacePointsInDistance3()
        {

            Map map = CreateSampleMapWithObstacles3();
            SpacePoint origin = new SpacePoint(new HexCoordinates(2, 1), 0);

            List<SpacePoint> spacePointsThreeStepsAway = map.GetSpacePointsInDistance(origin, 3);
            List<SpacePoint> expectedPoints = new List<SpacePoint>
            {
                new SpacePoint(new HexCoordinates(1, 1), 1),
                new SpacePoint(new HexCoordinates(2, 2), 1),
            };

            Assert.True(spacePointArraysAreEqual(spacePointsThreeStepsAway, expectedPoints));
        }

        [Test]
        public void MapGetSpacePointsInDistanceZeroCase()
        {

            Map map = CreateSampleMapWithObstacles3();
            SpacePoint origin = new SpacePoint(new HexCoordinates(1, 1), 0);

            List<SpacePoint> spacePointsThreeStepsAway = map.GetSpacePointsInDistance(origin, 0);
            List<SpacePoint> expectedPoints = new List<SpacePoint>
            {
                new SpacePoint(new HexCoordinates(1, 1), 0),
            };

            Assert.True(spacePointArraysAreEqual(spacePointsThreeStepsAway, expectedPoints));
        }

        [Test]
        public void MapGetSpacePointsInsideRangeCase()
        {

            Map map = CreateSampleMapWithObstacles3();
            SpacePoint origin = new SpacePoint(new HexCoordinates(1, 2), 0);

            List<SpacePoint> spacePointsThreeStepsAway = map.GetSpacePointsInsideRange(origin, 3, 2);
            List<SpacePoint> expectedPoints = new List<SpacePoint>
            {
                new SpacePoint(new HexCoordinates(1, 1), 0),
                new SpacePoint(new HexCoordinates(1, 1), 1),
                new SpacePoint(new HexCoordinates(2, 1), 0),
                new SpacePoint(new HexCoordinates(2, 2), 0),
                new SpacePoint(new HexCoordinates(1, 3), 0),
                new SpacePoint(new HexCoordinates(1, 3), 1),
                new SpacePoint(new HexCoordinates(3, 1), 1),
            };

            Assert.True(spacePointArraysAreEqual(spacePointsThreeStepsAway, expectedPoints));
        }



        [Test]
        public void MapGetOffset()
        {
            Map map = CreateSampleMap4x3();
            Assert.AreEqual(map.getOffset(), 1);
            map = CreateSampleMap4x4();
            Assert.AreEqual(map.getOffset(), 1);
            map = CreateSampleMap4x5();
            Assert.AreEqual(map.getOffset(), 2);
        }

        [Test]
        public void MapGetWidth()
        {
            Map map = CreateSampleMap4x3();
            Assert.AreEqual(map.width(), 4);
            map = CreateSampleMap5x6();
            Assert.AreEqual(map.width(), 5);
        }

        [Test]
        public void MapGetHeight()
        {
            Map map = CreateSampleMap4x3();
            Assert.AreEqual(map.height(), 3);
            map = CreateSampleMap5x6();
            Assert.AreEqual(map.height(), 6);
        }

        [Test]
        public void MapCoordAreInBounds()
        {
            Map map = CreateSampleMap4x3();
            Assert.False(map.coordsAreInBounds(new HexCoordinates(3, 0)));
            Assert.True(map.coordsAreInBounds(new HexCoordinates(2, 0)));
            Assert.True(map.coordsAreInBounds(new HexCoordinates(-1, 0)));
            Assert.False(map.coordsAreInBounds(new HexCoordinates(-2, 0)));


            Assert.False(map.coordsAreInBounds(new HexCoordinates(0, -1)));
            Assert.True(map.coordsAreInBounds(new HexCoordinates(0, 0)));
            Assert.True(map.coordsAreInBounds(new HexCoordinates(0, 1)));
            Assert.True(map.coordsAreInBounds(new HexCoordinates(0, 2)));
            Assert.False(map.coordsAreInBounds(new HexCoordinates(0, 3)));
        }

        [Test]
        public void MapHexesAtPointTest1()
        {
            Map map = CreateSampleMap4x4();

            SpacePoint point = new SpacePoint(new HexCoordinates(1, 0), 1);
            HexCoordinates[] validCoords = map.getValidHexCoordinatesAtPoint(point);
            HexCoordinates[] expected = new HexCoordinates[]
            {
                new HexCoordinates(0, 0),
                new HexCoordinates(1, 0)
            };

            Assert.True(helper.HexCoordinateGroupsAreEqual(validCoords, expected));
        }

        [Test]
        public void MapHexesAtPointTest2()
        {
            Map map = CreateSampleMap4x4();

            SpacePoint point = new SpacePoint(new HexCoordinates(0, 0), 1);
            HexCoordinates[] validCoords = map.getValidHexCoordinatesAtPoint(point);
            HexCoordinates[] expected = new HexCoordinates[]
            {
                new HexCoordinates(0, 0),
            };

            Assert.True(helper.HexCoordinateGroupsAreEqual(validCoords, expected));
        }

        [Test]
        public void MapHexesAtPointTest3()
        {
            Map map = CreateSampleMap4x4();

            var newmap = TestHelper.SerializeAndDeserialize(map);

            SpacePoint point = new SpacePoint(new HexCoordinates(1, 0), 0);
            HexCoordinates[] validCoords = newmap.getValidHexCoordinatesAtPoint(point);
            HexCoordinates[] expected = new HexCoordinates[]
            {
                new HexCoordinates(1, 0),
            };

            Assert.True(helper.HexCoordinateGroupsAreEqual(validCoords, expected));
        }

        

        [Test]
        public void TestCoordsToArrayIndexes()
        {

            Map map = CreateSampleMap3x3(); //offset is one

            HexCoordinates coords = new HexCoordinates(-1, 3);
            (int, int) expectedIndexes = (3, 0);
            Assert.True(expectedIndexes == map.coordsToArrayIndexes(coords));


        }

        [Test]
        public void TestArrayIndexesToCoords1()
        {

            Map map = CreateSampleMap3x3(); //offset is one

            (int, int) indexes = (2, 2);
            HexCoordinates expectedCoords = new HexCoordinates(1, 2);

            Assert.True(expectedCoords.Equals(map.arrayIndexesToCoords(indexes)));


        }

        [Test]
        public void TestArrayIndexesToCoords2()
        {

            Map map = CreateSampleMap3x3(); //offset is one

            (int, int) indexes = (3, 0);
            HexCoordinates expectedCoords = new HexCoordinates(-1, 3);
            Assert.True(expectedCoords.Equals(map.arrayIndexesToCoords(indexes)));


        }


        [Test]
        public void TestSetTileGroupAtSpacePointPositiveTest1()
        {
            Map map = CreateSampleMap4x4();
            TileGroup tg = new ResourceTileGroup(new ResourceTile[] { new OreResourceTile(circle), new FoodResourceTile(circle), new GoodsResourceTile(circle) });
            SpacePoint point = new SpacePoint(new HexCoordinates(1, 1), 1);
            map.SetTileGroupAtSpacePoint(tg, point);
            Assert.True(map.getTileAt(new HexCoordinates(1, 1)) is OreResourceTile);
            Assert.True(map.getTileAt(new HexCoordinates(0, 1)) is FoodResourceTile);
            Assert.True(map.getTileAt(new HexCoordinates(1, 0)) is GoodsResourceTile);
        }

        [Test]
        public void TestSetTileGroupAtSpacePointPositiveTest2()
        {
            Map map = CreateSampleMap4x4();
            TileGroup tg = new ResourceTileGroup(new ResourceTile[] { new OreResourceTile(circle), new FoodResourceTile(circle), new GoodsResourceTile(circle) });
            SpacePoint point = new SpacePoint(new HexCoordinates(0, 1), 0);
            map.SetTileGroupAtSpacePoint(tg, point);
            Assert.True(map.getTileAt(new HexCoordinates(0, 1)) is OreResourceTile);
            Assert.True(map.getTileAt(new HexCoordinates(0, 0)) is FoodResourceTile);
            Assert.True(map.getTileAt(new HexCoordinates(1, 0)) is GoodsResourceTile);
        }

        [Test]
        public void TestSetTileGroupAtSpacePointNegativeTest()
        {
            Map map = CreateSampleMap4x4();
            TileGroup tg = new ResourceTileGroup(new ResourceTile[] { new OreResourceTile(circle), new FoodResourceTile(circle), new GoodsResourceTile(circle) });
            SpacePoint point = new SpacePoint(new HexCoordinates(1, 1), 0);
            try
            {
                // should fail since (2,0) is BorderTile and cant be set
                map.SetTileGroupAtSpacePoint(tg, point);
                Assert.True(false);
            }
            catch (ArgumentException e)
            {
                Assert.True(true);
            }
        }

        [Test]
        public void ColonyBlockadePreventionTest()
        {
            MapGenerator generator = new DefaultMapGenerator();
            var mapModel = generator.GenerateRandomMap();

            var player = new TestHelper().CreateGenericPlayer();

            player.BuildTokenWithoutCost(
                mapModel,
                new ColonyBaseToken().GetType(),
                new SpacePoint(new HexCoordinates(5, 5), 1),
                new SpacePortToken().GetType()
            );

            var flyableColony = player.BuildTokenWithoutCost(
                mapModel,
                new ColonyBaseToken().GetType(),
                new SpacePoint(new HexCoordinates(5, 5), 0),
                new ShipToken().GetType()
            );

            flyableColony.stepsLeft = 4;
            var pointShouldNotBeReachable = new SpacePoint(new HexCoordinates(7, 4), 0);
            Assert.AreEqual(4, mapModel.distanceBetweenPoints(flyableColony.position, pointShouldNotBeReachable));

            var filter = new IsExactlyStepsAwayAndCannotSettleOnPointCounter(flyableColony, 4);
            //point should not fulfill the filter because the tilegroup is not revealed
            var tileGroup = (ResourceTileGroup)mapModel.FindTileGroupAtPoint(pointShouldNotBeReachable);
            Assert.True(!tileGroup.IsRevealed());
            Assert.True(!filter.pointFulfillsFilter(pointShouldNotBeReachable, mapModel));
        }

        Token BlockadeSetup(Map map, Player player, Player opponent)
        {
            player.BuildTokenWithoutCost(
                map,
                new ColonyBaseToken().GetType(),
                new SpacePoint(5, 5, 1),
                new SpacePortToken().GetType()
            );
            var flyableColony = player.BuildTokenWithoutCost(
                map,
                new ColonyBaseToken().GetType(),
                new SpacePoint(5, 5, 0),
                new ShipToken().GetType()
            );

            opponent.BuildTokenWithoutCost(
                map,
                new ColonyBaseToken().GetType(),
                new SpacePoint(7, 4, 0),
                new SpacePortToken().GetType()
            );

            return flyableColony;
        }

        [Test]
        public void SpacePortBlockadePreventionTest()
        {
            MapGenerator generator = new DefaultMapGenerator();
            var mapModel = generator.GenerateRandomMap();

            var player = new TestHelper().CreateGenericPlayer();
            var opponent = new Player(new SFColor(Color.green));

            var flyableColony = BlockadeSetup(mapModel, player, opponent);
            flyableColony.stepsLeft = 3;

            var pointShouldNotBeReachable = new SpacePoint(7, 4, 1);
            Assert.AreEqual(3, mapModel.distanceBetweenPoints(flyableColony.position, pointShouldNotBeReachable));

            var filter = new IsNeighborOfOwnSpacePortOrNotExactlyStepsAway(flyableColony, player, 3);
            //point should not fulfill the filter because the point is next to a other spaceport
            Assert.True(!filter.pointFulfillsFilter(pointShouldNotBeReachable, mapModel));
        }

        [Test]
        public void SpacePortBlockadePreventionTestOwnSpacePort()
        {
            MapGenerator generator = new DefaultMapGenerator();
            var mapModel = generator.GenerateRandomMap();

            var player = new TestHelper().CreateGenericPlayer();
            var opponent = new Player(new SFColor(Color.green));

            var flyableColony = BlockadeSetup(mapModel, player, opponent);
            flyableColony.stepsLeft = 2;

            var pointShouldBeReachable = new SpacePoint(4, 5, 0);
            Assert.AreEqual(2, mapModel.distanceBetweenPoints(flyableColony.position, pointShouldBeReachable));

            var filter = new IsNeighborOfOwnSpacePortOrNotExactlyStepsAway(flyableColony, player, 2);
            //point should fulfill the filter because the point is next to own spaceport
            Assert.True(filter.pointFulfillsFilter(pointShouldBeReachable, mapModel));
        }

        [Test]
        public void GetTradeStationsTest()
        {
            MapGenerator generator = new DefaultMapGenerator();
            Map map = generator.GenerateRandomMap();

            var tradeStation = map.GetTradeStations();
            Assert.AreEqual(4, tradeStation.Count);
        }

        [Test]
        public void MapSerialization()
        {
            MapGenerator generator = new DefaultMapGenerator();
            Map original = generator.GenerateRandomMap();

            var player = new TestHelper().CreateGenericPlayer();
            player.BuildTokenWithoutCost(
                original,
                new ColonyBaseToken().GetType(),
                new SpacePoint(5, 5, 1),
                new SpacePortToken().GetType()
            );

            var final = TestHelper.SerializeAndDeserialize(original);

            Assert.AreEqual(final.getOffset(), original.getOffset());
            Assert.AreEqual(final.width(), original.width());
            Assert.AreEqual(final.height(), original.height());
            Assert.False(object.ReferenceEquals(original, final));
        }

        [Test]
        public void SpacePortCannotBeBuildOnOtherSettledColonies()
        {
            var position = new SpacePoint(8, 6, 1);
            Map map = new DefaultMapGenerator().GenerateRandomMap();
            var players = TestHelper.CreateGenericPlayers3();
            players[1].BuildTokenWithoutCost(
                map,
                new ColonyBaseToken().GetType(),
                position
            );

            var pointsOfFirstPlayer = map.GetSpacePointsFullfillingFilters(
                new List<SpacePointFilter>()
                {
                    new IsOwnSettledColonySpacePointFilter(players[0])
                }
            );

            var pointsOfSecondPlayer = map.GetSpacePointsFullfillingFilters(
                new List<SpacePointFilter>()
                {
                    new IsOwnSettledColonySpacePointFilter(players[1])
                }
            );

            Assert.AreEqual(0, pointsOfFirstPlayer.Count);
            Assert.AreEqual(1, pointsOfSecondPlayer.Count);
        }

        [Test]
        public void ShipCannotBeBuildNextToOtherSpacePorts()
        {
            var position = new SpacePoint(8, 6, 1);
            Map map = new DefaultMapGenerator().GenerateRandomMap();
            var players = TestHelper.CreateGenericPlayers3();
            players[1].BuildTokenWithoutCost(
                map,
                new ColonyBaseToken().GetType(),
                position,
                new SpacePortToken().GetType()
            );

            var pointsOfFirstPlayer = map.GetSpacePointsFullfillingFilters(
                new List<SpacePointFilter>()
                {
                    new IsNeighborOwnSpacePortFilter(players[0])
                }
            );

            var pointsOfSecondPlayer = map.GetSpacePointsFullfillingFilters(
                new List<SpacePointFilter>()
                {
                    new IsNeighborOwnSpacePortFilter(players[1])
                }
            );

            Assert.AreEqual(0, pointsOfFirstPlayer.Count);
            Assert.AreEqual(3, pointsOfSecondPlayer.Count);
        }

    }
}
