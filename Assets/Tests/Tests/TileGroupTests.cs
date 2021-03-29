using System.Collections;
using System.Collections.Generic;
using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TileGroupTests
    {

        public CircleChipGroup circle = new CircleChipGroup(new List<DiceChip>());
        // A Test behaves as an ordinary method
        [Test]
        public void TileGroupSuccessfulCreationWith3Tiles()
        {
            // Use the Assert class to test conditions
            ResourceTile[] tiles = { new FoodResourceTile(circle), new FoodResourceTile(circle), new FoodResourceTile(circle) };
            TileGroup group = new ResourceTileGroup(tiles);

        }

        [Test]
        public void TileGroupFailCreationWithLessThan3Tiles()
        {
            try
            {
                ResourceTile[] tiles = { new FoodResourceTile(circle), new FoodResourceTile(circle) };
                TileGroup group = new ResourceTileGroup(tiles);
                Assert.True(false); // Fail this test if we come to this point
            }
            catch (ArgumentException e)
            {
                Assert.True(true);
            }

        }

        [Test]
        public void ShiftTilesBy0()
        {
            var foodTile = new FoodResourceTile(circle);
            var goodsTile = new GoodsResourceTile(circle);
            var oreTile = new OreResourceTile(circle);

            var tg = new ResourceTileGroup(new ResourceTile[] { foodTile, goodsTile, oreTile });
            tg.ShiftTiles(0);
            Assert.True(tg.GetTiles()[0] == foodTile && tg.GetTiles()[1] == goodsTile && tg.GetTiles()[2] == oreTile);

        }

        [Test]
        public void ShiftTilesBy3()
        {
            var foodTile = new FoodResourceTile(circle);
            var goodsTile = new GoodsResourceTile(circle);
            var oreTile = new OreResourceTile(circle);

            var tg = new ResourceTileGroup(new ResourceTile[] { foodTile, goodsTile, oreTile });

            tg.ShiftTiles(3);
            Assert.True(tg.GetTiles()[0] == foodTile && tg.GetTiles()[1] == goodsTile && tg.GetTiles()[2] == oreTile);

        }

        [Test]
        public void ShiftTilesBy1()
        {
            var foodTile = new FoodResourceTile(circle);
            var goodsTile = new GoodsResourceTile(circle);
            var oreTile = new OreResourceTile(circle);

            var tg = new ResourceTileGroup(new ResourceTile[] { foodTile, goodsTile, oreTile });
            tg.ShiftTiles(1);
            Assert.True(tg.GetTiles()[0] == oreTile && tg.GetTiles()[1] == foodTile && tg.GetTiles()[2] == goodsTile);

        }

        [Test]
        public void ShiftTilesBy2()
        {
            var foodTile = new FoodResourceTile(circle);
            var goodsTile = new GoodsResourceTile(circle);
            var oreTile = new OreResourceTile(circle);

            var tg = new ResourceTileGroup(new ResourceTile[] { foodTile, goodsTile, oreTile });
            tg.ShiftTiles(2);
            Assert.True(tg.GetTiles()[0] == goodsTile && tg.GetTiles()[1] == oreTile && tg.GetTiles()[2] == foodTile);

        }

        [Test]
        public void TileGroupFailCreationWithMoreThan3Tiles()
        {
            try
            {
                ResourceTile[] tiles = { new FoodResourceTile(circle), new FoodResourceTile(circle), new FoodResourceTile(circle), new FoodResourceTile(circle) };
                TileGroup group = new ResourceTileGroup(tiles);
                Assert.True(false); // Fail this test if we come to this point
            }
            catch (ArgumentException e)
            {
                Assert.True(true);
            }
        }

        ResourceTileGroup CreateResourceTileGroup(SpacePoint center)
        {
            var rtg = new ResourceTileGroup(new ResourceTile[] { new FoodResourceTile(circle), new FoodResourceTile(circle), new FoodResourceTile(circle) });
            rtg.SetCenter(center);
            return rtg;
        }

        [Test]
        public void RequestSettleResourceTileGroupToken_NOT_OnSettleSpot()
        {
            var center = new SpacePoint(new HexCoordinates(4, 4), 0);
            var rtg = CreateResourceTileGroup(center);

            Token token = new ColonyBaseToken();
            token.position = center;
            try
            {
                rtg.RequestSettleOfToken(token);
                Assert.True(false);
            } catch (NotOnSettleSpotException e)
            {
                Assert.True(true);
            }
        }

        Tuple<ResourceTileGroup, Token> CreateColonyTokenAndTileGroup(SpacePoint tokenPos, SpacePoint tileGroupCenter)
        {
            var rtg = new ResourceTileGroup(new ResourceTile[] { new FoodResourceTile(circle), new FoodResourceTile(circle), new FoodResourceTile(circle) });
            rtg.SetCenter(tileGroupCenter);
            Token token = new ColonyBaseToken();
            token.position = tokenPos;

            return new Tuple<ResourceTileGroup, Token>(rtg, token);
        }

        Tuple<ResourceTileGroup, Token> CreateTradeTokenAndTileGroup(SpacePoint tokenPos, SpacePoint tileGroupCenter)
        {
            var rtg = new ResourceTileGroup(new ResourceTile[] { new FoodResourceTile(circle), new FoodResourceTile(circle), new FoodResourceTile(circle) });
            rtg.SetCenter(tileGroupCenter);
            Token token = new TradeBaseToken();
            token.position = tokenPos;

            return new Tuple<ResourceTileGroup, Token>(rtg, token);
        }

        Tuple<TradeStation, Token> CreateColonyTokenAndTradeStation(SpacePoint tokenPos, SpacePoint tileGroupCenter)
        {
            var tradeStation = new OrzelTradeStation();
            tradeStation.SetCenter(tileGroupCenter);
            Token token = new ColonyBaseToken();
            token.position = tokenPos;

            return new Tuple<TradeStation, Token>(tradeStation, token);
        }

        Tuple<TradeStation, Token> CreateTradeTokenAndTradeStation(SpacePoint tokenPos, SpacePoint tileGroupCenter)
        {
            var tradeStation = new OrzelTradeStation();
            tradeStation.SetCenter(tileGroupCenter);
            Token token = new TradeBaseToken();
            token.position = tokenPos;

            return new Tuple<TradeStation, Token>(tradeStation, token);
        }

        [Test]
        public void RequestSettleResourceTileGroupToken_IS_OnSettleSpot()
        {
            var center = new SpacePoint(new HexCoordinates(4, 4), 0);
            var tokenPos = new SpacePoint(new HexCoordinates(4, 4), 1);
            var tuple = CreateColonyTokenAndTileGroup(tokenPos, center);

            tuple.Item1.RequestSettleOfToken(tuple.Item2);
            Assert.True(true);
        }

        [Test]
        public void RequestSettleResourceTileGroupToken_IS_OnSettleSpot_IS_TradeShip()
        {
            var center = new SpacePoint(new HexCoordinates(4, 4), 0);
            var tokenPos = new SpacePoint(new HexCoordinates(4, 4), 1);
            var tuple = CreateTradeTokenAndTileGroup(tokenPos, center);

            try
            {
                tuple.Item1.RequestSettleOfToken(tuple.Item2);
                Assert.True(false);
            } catch (WrongTokenTypeException e)
            {
                Assert.True(true);
            }
        }

        [Test]
        public void RequestSettleResourceTileGroupToken_IS_OnSettleSpot_IS_ColonyShip()
        {
            var center = new SpacePoint(new HexCoordinates(4, 4), 0);
            var tokenPos = new SpacePoint(new HexCoordinates(4, 4), 1);
            var tuple = CreateColonyTokenAndTileGroup(tokenPos, center);

            try
            {
                tuple.Item1.RequestSettleOfToken(tuple.Item2);
                Assert.True(true);
            }
            catch (WrongTokenTypeException e)
            {
                Assert.True(false);
            }
        }

        [Test]
        public void RequestSettleTradeStationToken_NOT_OnSettleSpot()
        {
            var center = new SpacePoint(new HexCoordinates(4, 4), 0);
            var tokenPos = new SpacePoint(new HexCoordinates(4, 4), 1);
            var tuple = CreateTradeTokenAndTradeStation(tokenPos, center);

            try
            {
                tuple.Item1.RequestSettleOfToken(tuple.Item2);
                Assert.True(false);
            }
            catch (NotOnSettleSpotException e)
            {
                Assert.True(true);
            }
        }

        [Test]
        public void RequestSettleTradeStationToken_Is_OnSettleSpot_NOT_tradeship()
        {
            var center = new SpacePoint(new HexCoordinates(4, 4), 0);
            var tokenPos = new SpacePoint(new HexCoordinates(4, 4), 0);
            var tuple = CreateColonyTokenAndTradeStation(tokenPos, center);

            try
            {
                tuple.Item1.RequestSettleOfToken(tuple.Item2);
                Assert.True(false);
            }
            catch (WrongTokenTypeException e)
            {
                Assert.True(true);
            }
        }

        [Test]
        public void RequestSettleTradeStation_Is_OnSettleSpot_IS_tradeship_NO_spaceleft()
        {
            var center = new SpacePoint(new HexCoordinates(4, 4), 0);
            var tokenPos = new SpacePoint(new HexCoordinates(4, 4), 0);
            var tuple = CreateTradeTokenAndTradeStation(tokenPos, center);

            try
            {
                var capacity = tuple.Item1.GetCapacity();
                for (int i = 0; i < capacity; i++)
                {
                    tuple.Item1.dockedSpaceships.Add(new ColonyBaseToken());
                }
                tuple.Item1.RequestSettleOfToken(tuple.Item2);
                Assert.True(false);
            }
            catch (TradeStationIsFullException e)
            {
                Assert.True(true);
            }
        }

        public void RequestSettleTradeStationNotEnoughFreightPods()
        {
            var center = new SpacePoint(new HexCoordinates(4, 4), 0);
            var tokenPos = new SpacePoint(new HexCoordinates(4, 4), 0);
            var tuple = CreateTradeTokenAndTradeStation(tokenPos, center);
            var tradeStation = tuple.Item1;
            var token = tuple.Item2;

            var testHelper = new TestHelper();
            var player = testHelper.CreateGenericPlayer();
            token.owner = player;

            try
            {
                var capacity = tradeStation.GetCapacity();
                for (int i = 0; i < 2; i++)
                {
                    tradeStation.OnTokenSettled(token);
                }
                tradeStation.RequestSettleOfToken(token);
                Assert.True(false);
            }
            catch (NotEnoughFreightPodsToDockException e)
            {
                Assert.True(true);
            }
        }

        [Test]
        public void TestDockingBehaviourOfTradeStation()
        {
            var center = new SpacePoint(new HexCoordinates(4, 4), 0);
            var tokenPos = new SpacePoint(new HexCoordinates(4, 4), 0);
            var tuple = CreateTradeTokenAndTradeStation(tokenPos, center);
            var tradeStation = tuple.Item1;
            var token = tuple.Item2;
            var player = new TestHelper().CreateGenericPlayer();
            token.owner = player;

            var spaceShipsBefore = tradeStation.dockedSpaceships.Count;
            tradeStation.OnTokenSettled(token);
            var spaceShipsAfter = tradeStation.dockedSpaceships.Count;

            Assert.AreEqual(spaceShipsAfter, spaceShipsBefore + 1);
        }

        [Test]
        public void TradeStationHighestPlayer()
        {
            var center = new SpacePoint(new HexCoordinates(4, 4), 0);
            var tokenPos = new SpacePoint(new HexCoordinates(4, 4), 0);
            var tuple = CreateTradeTokenAndTradeStation(tokenPos, center);
            var tradeStation = tuple.Item1;
            var ownToken = tuple.Item2;
            var player = new TestHelper().CreateGenericPlayer();
            ownToken.owner = player;

            var opponent = new Player(Color.cyan, new SFElement());
            var opponentToken = new TradeBaseToken();
            opponentToken.owner = opponent;

            Assert.True(tradeStation.medal.owner == null);

            tradeStation.OnTokenSettled(ownToken);
            Assert.True(tradeStation.medal.owner == player); //1:0 
            Assert.True(player.GetVictoryPointsFromFriendships() == 2); 

            tradeStation.OnTokenSettled(opponentToken);
            Assert.True(tradeStation.medal.owner == player); //1:1, still the same

            tradeStation.OnTokenSettled(opponentToken);
            Assert.True(tradeStation.medal.owner == opponent); //1:2, now the owner changes because he has more ships docked
            Assert.True(player.GetVictoryPointsFromFriendships() == 0);
            Assert.True(opponent.GetVictoryPointsFromFriendships() == 2);

            tradeStation.OnTokenSettled(ownToken);
            Assert.True(tradeStation.medal.owner == opponent); // 2:2

            tradeStation.OnTokenSettled(ownToken);
            Assert.True(tradeStation.medal.owner == player); // 3:2, the owner changes back to player
        }

    }
}
