using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PirateTokenTests
    {
        TestHelper testHelper = new TestHelper();
        // A Test behaves as an ordinary method
        [Test]
        public void TestFreightPodBeatCondition()
        {
            // Use the Assert class to test conditions
            var player = testHelper.CreateGenericPlayer();

            var condition = new FreightPodsBeatCondition(3);

            Assert.True(!condition.SpaceShipFullfillsCondition(player.ship));

            player.ship.Add(new FreightPodUpgradeToken());
            Assert.True(!condition.SpaceShipFullfillsCondition(player.ship));

            player.ship.Add(new FreightPodUpgradeToken());
            Assert.True(!condition.SpaceShipFullfillsCondition(player.ship));

            player.ship.Add(new FreightPodUpgradeToken());
            Assert.True(condition.SpaceShipFullfillsCondition(player.ship));
        }

        [Test]
        public void TestCannonPodBeatCondition()
        {
            // Use the Assert class to test conditions
            var player = testHelper.CreateGenericPlayer();

            var condition = new CannonBeatCondition(3);

            Assert.True(!condition.SpaceShipFullfillsCondition(player.ship));

            player.ship.Add(new CannonUpgradeToken());
            Assert.True(!condition.SpaceShipFullfillsCondition(player.ship));

            player.ship.Add(new CannonUpgradeToken());
            Assert.True(!condition.SpaceShipFullfillsCondition(player.ship));

            player.ship.Add(new CannonUpgradeToken());
            Assert.True(condition.SpaceShipFullfillsCondition(player.ship));
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator PirateTokenTestsWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
