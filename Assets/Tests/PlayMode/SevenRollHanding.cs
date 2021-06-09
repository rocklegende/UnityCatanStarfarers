using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using com.onebuckgames.UnityStarFarers;

namespace Tests
{
    [Category("No Photon")]
    public class SevenRollHanding
    {
        PlayModeTestHelper testHelper;
        GameController gameController;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            testHelper = new PlayModeTestHelper();
            yield return testHelper.StartSinglePlayerGame();
            gameController = testHelper.GetGameController();
            yield return null;
        }

        bool GameInteractionIsDeactivated()
        {
            return gameController.GetHUDScript().waitingForOtherPlayersPopup.activeInHierarchy &&
            gameController.GetHUDScript().isInteractionActivated == false &&
            gameController.GetMapScript().isInteractive == false;
        }

        bool GameInteractionIsActivated()
        {
            return !gameController.GetHUDScript().waitingForOtherPlayersPopup.activeInHierarchy &&
            gameController.GetHUDScript().isInteractionActivated == true &&
            gameController.GetMapScript().isInteractive == true;
        }

        [UnityTest]
        public IEnumerator IfPlayerNeedsToDiscardCardsStopAllInteraction()
        {
            gameController.dispatcher = new NoResponseRemoteActionDispatcher(gameController);
            var dispatcher = (NoResponseRemoteActionDispatcher) gameController.dispatcher;
            var targets = new List<Player>() { new Player(new SFColor(Color.black), "Tommy") };
            gameController.RequestActionFromPlayers(new DiscardRemoteClientAction(gameController.mainPlayer), targets, null,
                (responses) => {
                    Assert.True(GameInteractionIsActivated());
                });

            Assert.True(GameInteractionIsDeactivated());
            yield return new WaitForSeconds(1);
            dispatcher.FakeResponseFromAllPlayers(true);            
        }

        [UnityTest]
        public IEnumerator IfWeAreOnlyPlayerThatNeedsToDiscardAllInteractionIsStillPrevented()
        {
            gameController.dispatcher = new NoResponseRemoteActionDispatcher(gameController);
            var dispatcher = (NoResponseRemoteActionDispatcher)gameController.dispatcher;
            var targets = new List<Player>() { gameController.mainPlayer };
            gameController.RequestActionFromPlayers(new DiscardRemoteClientAction(gameController.mainPlayer), targets, null,
                (responses) => {
                    Assert.True(GameInteractionIsActivated());
                });

            Assert.True(GameInteractionIsDeactivated());
            yield return new WaitForSeconds(1);
            dispatcher.FakeResponseFromAllPlayers(true);
        }

        [UnityTest]
        public IEnumerator RequestingADiscardActionActuallyOpensAResourcePicker()
        {
            gameController.dispatcher = new DefaultRemoteActionDispatcher(gameController);
            var targets = new List<Player>() { gameController.mainPlayer };
            gameController.RequestActionFromPlayers(
                new DiscardRemoteClientAction(gameController.mainPlayer),
                targets,
                null,
                null
            );
            yield return new WaitForSeconds(3);
            Assert.True(gameController.GetHUDScript().resourcePicker.activeInHierarchy);
        }

        [UnityTest]
        public IEnumerator IfTwoPlayersNeedToDiscardAndOnlyASingleRespondsAllInteractionIsStillPrevented()
        {
            gameController.dispatcher = new NoResponseRemoteActionDispatcher(gameController);
            var dispatcher = (NoResponseRemoteActionDispatcher)gameController.dispatcher;
            var targets = new List<Player>() {
                new Player(new SFColor(Color.black), "Tommy"),
                new Player(new SFColor(Color.green), "Joachim"),
            };
            gameController.RequestActionFromPlayers(new DiscardRemoteClientAction(gameController.mainPlayer), targets,
                (singleResponse) => {
                    Assert.True(GameInteractionIsDeactivated());
                }, null);

            Assert.True(GameInteractionIsDeactivated());
            yield return new WaitForSeconds(1);
            dispatcher.FakeResponseFromSinglePlayer("Joachim", true);
        }

        [UnityTest]
        public IEnumerator IfITriggerSevenRollHandlingAndNoPlayerHasMoreThan7Cards()
        {
            //no one has more than 7 cards..
            gameController.mainPlayer.hand = new Hand();
            gameController.On7Rolled();
            // we should be immediately able to get back to the game, since there is no one to pick and no one having more than 7 cards
            yield return new WaitForSeconds(1);
            Assert.True(GameInteractionIsActivated());
        }

        [UnityTest]
        public IEnumerator RequestingActionWithEmptyTargetListImmediatelyReturns()
        {
            gameController.dispatcher = new DefaultRemoteActionDispatcher(gameController);
            var targets = new List<Player>() { };
            gameController.RequestActionFromPlayers(
                new DiscardRemoteClientAction(gameController.mainPlayer),
                targets,
                null,
                null
            );
            yield return new WaitForSeconds(3);
            Assert.True(GameInteractionIsActivated());            
        }

        [UnityTest]
        public IEnumerator OpeningPlayerSelectionWithoutAnyPlayersToSelectFromShouldReturnImmediately()
        {
            var didCallbackImmediately = false;
            gameController.GetHUDScript().OpenPlayerSelection(
                new List<Player>() { },
                (selectedIndexes) => {
                    didCallbackImmediately = true;
                });
            Assert.True(didCallbackImmediately);
            yield return null;
        }


    }
}
