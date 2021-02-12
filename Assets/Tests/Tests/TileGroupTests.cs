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
        // A Test behaves as an ordinary method
        [Test]
        public void TileGroupSuccessfulCreationWith3Tiles()
        {
            // Use the Assert class to test conditions
            ResourceTile[] tiles = { new FoodResourceTile(), new FoodResourceTile(), new FoodResourceTile() };
            TileGroup group = new ResourceTileGroup(tiles);

        }

        [Test]
        public void TileGroupFailCreationWithLessThan3Tiles()
        {
            try
            {
                ResourceTile[] tiles = { new FoodResourceTile(), new FoodResourceTile() };
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
            var foodTile = new FoodResourceTile();
            var goodsTile = new GoodsResourceTile();
            var oreTile = new OreResourceTile();

            var tg = new ResourceTileGroup(new ResourceTile[] { foodTile, goodsTile, oreTile });
            tg.ShiftTiles(0);
            Assert.True(tg.GetTiles()[0] == foodTile && tg.GetTiles()[1] == goodsTile && tg.GetTiles()[2] == oreTile);

        }

        [Test]
        public void ShiftTilesBy3()
        {
            var foodTile = new FoodResourceTile();
            var goodsTile = new GoodsResourceTile();
            var oreTile = new OreResourceTile();

            var tg = new ResourceTileGroup(new ResourceTile[] { foodTile, goodsTile, oreTile });

            tg.ShiftTiles(3);
            Assert.True(tg.GetTiles()[0] == foodTile && tg.GetTiles()[1] == goodsTile && tg.GetTiles()[2] == oreTile);

        }

        [Test]
        public void ShiftTilesBy1()
        {
            var foodTile = new FoodResourceTile();
            var goodsTile = new GoodsResourceTile();
            var oreTile = new OreResourceTile();

            var tg = new ResourceTileGroup(new ResourceTile[] { foodTile, goodsTile, oreTile });
            tg.ShiftTiles(1);
            Assert.True(tg.GetTiles()[0] == oreTile && tg.GetTiles()[1] == foodTile && tg.GetTiles()[2] == goodsTile);

        }

        [Test]
        public void ShiftTilesBy2()
        {
            var foodTile = new FoodResourceTile();
            var goodsTile = new GoodsResourceTile();
            var oreTile = new OreResourceTile();

            var tg = new ResourceTileGroup(new ResourceTile[] { foodTile, goodsTile, oreTile });
            tg.ShiftTiles(2);
            Assert.True(tg.GetTiles()[0] == goodsTile && tg.GetTiles()[1] == oreTile && tg.GetTiles()[2] == foodTile);

        }

        [Test]
        public void TileGroupFailCreationWithMoreThan3Tiles()
        {
            try
            {
                ResourceTile[] tiles = { new FoodResourceTile(), new FoodResourceTile(), new FoodResourceTile(), new FoodResourceTile() };
                TileGroup group = new ResourceTileGroup(tiles);
                Assert.True(false); // Fail this test if we come to this point
            }
            catch (ArgumentException e)
            {
                Assert.True(true);
            }



        }


    }
}
