using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class NetworkManager : Photon.PunBehaviour
{
    public string GameVersion = "v1.0";
    public Button StartBtn;
    public GameObject NamePanel, RoomPanel;
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(GameVersion);
    }

    public void SetName(TMP_InputField NameIF)
    {
        SetName(NameIF.text);
    }

    public void SetName(string name)
    {
        if (name.Length > 0)
        {
            PhotonNetwork.player.NickName = name;
            NamePanel.SetActive(false);
            RoomPanel.SetActive(true);
        }
            
    }

    public void CreateRoom(TMP_InputField RoomIF)
    {
        CreateRoom(RoomIF.text);
    }

    public void CreateRoom(string text)
    {
        if (text.Length > 0)
            PhotonNetwork.CreateRoom(text,new RoomOptions() { MaxPlayers=4},typedLobby:TypedLobby.Default);
    }

    public void JoinRoom(TMP_InputField RoomIF)
    {
        JoinOrCreate(RoomIF.text);
    }

    public void JoinOrCreate(string text)
    {
        if (text.Length > 0)
            PhotonNetwork.JoinOrCreateRoom(text,new RoomOptions() { MaxPlayers=4},typedLobby:TypedLobby.Default);
    }

    #region callbacks

    public override void OnConnectedToMaster()
    {
        StartBtn.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room " + PhotonNetwork.room.Name);
        Debug.Log("Players " + PhotonNetwork.room.PlayerCount);
        PhotonNetwork.LoadLevel(1);
    }

    #endregion
}
