using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;


public class NetworkLauncher : MonoBehaviourPunCallbacks
{
    public GameObject nameUI;
    public GameObject loginUI;
    public InputField roomName;
    public InputField playerName;
    public GameObject roomListUI;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        nameUI.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

    public void PlayButton()
    {
        nameUI.SetActive(false);
        
        PhotonNetwork.NickName = playerName.text;
        loginUI.SetActive(true);
        if(PhotonNetwork.InLobby)
            roomListUI.SetActive(true);
    }

    public void CreateOrJoinButton()
    {
        if (roomName.text.Length < 2)
            return;
        
        loginUI.SetActive(false);
        roomListUI.SetActive(false);
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 4 };
        PhotonNetwork.JoinOrCreateRoom(roomName.text, roomOptions, default);
        
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
