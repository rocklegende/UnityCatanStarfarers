using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace com.onebuckgames.UnityStarFarers
{

    public class SFScenes
    {
        public static int SINGLE_PLAYER_LOBBY_SCENE = 2;
        public static int GAME_SCENE = 1;
        public static int LOBBY_SCENE = 0;
    }

    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields

        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 2;

        [SerializeField]
        private bool autoConnect = true;

        [SerializeField]
        private bool loadGameSceneDirectlyWithoutLogin = true;

        #endregion


        #region Private Fields


        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "1";

        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;


        #endregion

        #region MonoBehaviourPunCallbacks Callbacks


        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
            PhotonNetwork.JoinRandomRoom();
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
            ShowIsNotConnecting();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
            progressLabel.GetComponentInChildren<Text>().text = "Joined a room,  waiting for others..";

            if (GameConstants.isDevelopment)
            {
                StartGame(); // start game immediately when in development mode, dont wait on multiple players
            }
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", newPlayer.NickName);

            if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayersPerRoom)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    StartGame();
                }
            }
        }

        public void StartGame()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Logger.LogError("PhotonNetwork : Trying to start game but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel(SFScenes.GAME_SCENE);

        }


        #endregion


        #region MonoBehaviour CallBacks


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            PhotonNetwork.OfflineMode = GameConstants.isDevelopment;
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            ShowIsNotConnecting();

            if (autoConnect)
            {
                StartCoroutine("Wait1SecondsThenConnect");
            }

            if (GameConstants.isDevelopment && loadGameSceneDirectlyWithoutLogin)
            {
                SceneManager.LoadScene(SFScenes.GAME_SCENE);
            }
        }

        IEnumerator Wait1SecondsThenConnect()
        {
            yield return new WaitForSeconds(1);
            Connect();
        }

        #endregion


        #region Public Methods


        void ShowIsConnecting()
        {
            controlPanel.SetActive(false);
            progressLabel.SetActive(true);
        }

        void ShowIsNotConnecting()
        {
            controlPanel.SetActive(true);
            progressLabel.SetActive(false);
        }

        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            ShowIsConnecting();
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("Is Connected");
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                Debug.Log("Logging in");
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        public void QuickMatchButtonPressed()
        {
            StartQuickMM();
        }

        void StartQuickMM()
        {
            Connect();
        }


        #endregion
    }
}
