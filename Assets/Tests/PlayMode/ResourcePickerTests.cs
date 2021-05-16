using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    [Category("No Photon")]
    public class ResourcePickerTests
    {

        GameController gameController;
        PlayModeTestHelper testHelper;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            var testHelper = new PlayModeTestHelper();
            this.testHelper = testHelper;
            yield return testHelper.LoadDefaultScene();
            gameController = testHelper.GetGameController();
            gameController.SetUpDebugState(new TwoTradeShipAndOneSpacePort(gameController));
           
        }

        [UnityTest]
        public IEnumerator OpenNormalResourcePickerIncreaseAllTypesBy1()
        {
            var hudScript = gameController.GetHUDScript();
            hudScript.OpenResourcePicker((hand) => Debug.Log(""));

            var singleStacks = hudScript.resourcePicker.GetComponentsInChildren<SingleResourceCardStack>();
            foreach(var singleStack in singleStacks)
            {
                singleStack.AddButton.onClick.Invoke();
            }
            var currentlyPickedHand = hudScript.resourcePicker.GetComponent<ResourcePicker>().GetCurrentlyPickedHand();
            Assert.True(currentlyPickedHand.HasSameCardsAs(Hand.FromResources(1, 1, 1, 1, 1)));
            yield return null;
        }

        [UnityTest]
        public IEnumerator OpenNormalResourcePickerDecreaseAllTypesBy1_Should_Stay_At_Zero()
        {
            var hudScript = gameController.GetHUDScript();
            hudScript.OpenResourcePicker((hand) => Debug.Log(""));

            var singleStacks = hudScript.resourcePicker.GetComponentsInChildren<SingleResourceCardStack>();
            foreach (var singleStack in singleStacks)
            {
                singleStack.RemoveButton.onClick.Invoke();
            }
            var currentlyPickedHand = hudScript.resourcePicker.GetComponent<ResourcePicker>().GetCurrentlyPickedHand();
            Assert.True(currentlyPickedHand.HasSameCardsAs(Hand.FromResources(0, 0, 0, 0, 0)));
            yield return null;
        }

        [UnityTest]
        public IEnumerator OpenNormalResourcePickerIncreaseThenDecrease()
        {
            var hudScript = gameController.GetHUDScript();
            hudScript.OpenResourcePicker((hand) => Debug.Log(""));

            var singleStacks = hudScript.resourcePicker.GetComponentsInChildren<SingleResourceCardStack>();
            foreach (var singleStack in singleStacks)
            {
                singleStack.AddButton.onClick.Invoke();
                singleStack.RemoveButton.onClick.Invoke();
            }
            var currentlyPickedHand = hudScript.resourcePicker.GetComponent<ResourcePicker>().GetCurrentlyPickedHand();
            Assert.True(currentlyPickedHand.HasSameCardsAs(Hand.FromResources(0, 0, 0, 0, 0)));
            yield return null;
        }
    }
}
