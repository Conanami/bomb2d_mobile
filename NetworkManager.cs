using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject readyButton;
    public void ReadyToPlay()
    {
        readyButton.SetActive(false);
        PhotonNetwork.Instantiate("PlayerControl", new Vector3(-11, 1, 0), Quaternion.identity, 0);
    }
}
