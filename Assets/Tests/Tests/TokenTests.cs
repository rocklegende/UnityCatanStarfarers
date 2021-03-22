using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TokenTests
    {

        [Test]
        public void SettlingMechanisms1()
        {
            Player player = new TestHelper().CreateGenericPlayer();
            var token = new ColonyBaseToken();
            token.attachToken(new ShipToken());
            token.owner = player;
            Assert.True(token.attachedToken != null);
            Assert.True(!token.IsSettled());
            token.settle();
            Assert.True(token.attachedToken == null);
            Assert.True(token.IsSettled());
        }

        [Test]
        public void SettlingTradeShip()
        {
            Player player = new TestHelper().CreateGenericPlayer();
            var token = new TradeBaseToken();
            token.attachToken(new ShipToken());
            token.owner = player;

            var shipsInStorageBeforeSettling = player.tokenStorage.GetTokensOfType(new ShipToken().GetType()).Length;
            var tradeInStorageBeforeSettling = player.tokenStorage.GetTokensOfType(new TradeBaseToken().GetType()).Length;
            token.settle();
            var shipsInStorageAfterSettling = player.tokenStorage.GetTokensOfType(new ShipToken().GetType()).Length;
            var tradeInStorageAfterSettling = player.tokenStorage.GetTokensOfType(new TradeBaseToken().GetType()).Length;

            Assert.AreEqual(shipsInStorageBeforeSettling + 1, shipsInStorageAfterSettling); //we get the ship back
            Assert.AreEqual(tradeInStorageBeforeSettling, tradeInStorageAfterSettling); //we DONT get trade base token back
        }

        [Test]
        public void SettlingColonyShip()
        {
            Player player = new TestHelper().CreateGenericPlayer();
            var token = new ColonyBaseToken();
            token.attachToken(new ShipToken());
            token.owner = player;

            var shipsInStorageBeforeSettling = player.tokenStorage.GetTokensOfType(new ShipToken().GetType()).Length;
            var tradeInStorageBeforeSettling = player.tokenStorage.GetTokensOfType(new ColonyBaseToken().GetType()).Length;
            token.settle();
            var shipsInStorageAfterSettling = player.tokenStorage.GetTokensOfType(new ShipToken().GetType()).Length;
            var tradeInStorageAfterSettling = player.tokenStorage.GetTokensOfType(new ColonyBaseToken().GetType()).Length;

            Assert.AreEqual(shipsInStorageBeforeSettling + 1, shipsInStorageAfterSettling); //we get the ship back
            Assert.AreEqual(tradeInStorageBeforeSettling, tradeInStorageAfterSettling); //we DONT get colony base token back
        }

    }
}
