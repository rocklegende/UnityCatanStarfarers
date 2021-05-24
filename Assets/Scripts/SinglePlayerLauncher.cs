using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using com.onebuckgames.UnityStarFarers;
using Photon.Pun;
using Photon.Realtime;

public class SinglePlayerLauncher : MonoBehaviourPunCallbacks
{

    public void Launch()
    {
        PhotonNetwork.OfflineMode = true;
        PhotonNetwork.NickName = "Tim";
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room, starting game now!");
        StartCoroutine("LoadGameScene");
    }

    public IEnumerator LoadGameScene()
    {
        SceneManager.LoadScene(SFScenes.GAME_SCENE);
        yield return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        Launch();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
