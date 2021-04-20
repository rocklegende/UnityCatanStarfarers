using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using com.onebuckgames.UnityStarFarers;
using Photon.Pun;
using Photon.Realtime;

namespace Tests
{
    [Category("No Photon")]
    public class EncounterCardActions
    {
        GameController gameController;
        PlayModeTestHelper testHelper;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            var testHelper = new PlayModeTestHelper();
            this.testHelper = testHelper;
            yield return testHelper.LoadDefaultScene();
            yield return testHelper.SetupDebugState();
            gameController = testHelper.GetGameController();
        }

        [UnityTest]
        public IEnumerator ReceivingGiveResourcesFromRemoteClientOpensResourcePicker()
        {
            var players = TestHelper.CreateGenericPlayers3();

            var action = new RemoteClientAction(RemoteClientActionType.GIVE_RESOURCE, new object[] { 1 }, 1);
            gameController.RemoteClientRequiresAction(SFFormatter.Serialize(action));

            var hudScript = gameController.GetHUDScript();
            Assert.True(hudScript.resourcePicker.activeInHierarchy);

            //hudScript.resourcePicker.GetComponent<ResourcePicker>().selectButton.onClick.Invoke();
            yield return null;


        }
    }
}
