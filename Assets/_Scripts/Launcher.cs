using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text roomNameTxt;
    [SerializeField] TMP_Text errorTxt;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameBtn;

    private void Awake() {
        if (Instance) { //checks if a RoomManager allredy exists
            Destroy(gameObject); //destroys this if it does There can only be ONE
            return;
        }
        //DontDestroyOnLoad(gameObject); //Keeps the gameobject on scene changes
        Instance = this; //sets the instance to this script instance. I am The ONE
    }
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        PhotonNetwork.JoinLobby();
        Debug.Log("Master joined");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby() {
        Debug.Log("lobby joined");
        MenuManager.Instance.OpenMenu("Title");
    }

    public void CreateRoom() { 
        if (string.IsNullOrEmpty(roomNameInputField.text)) {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.Instance.OpenMenu("Loading");
    }
    public override void OnJoinedRoom() {
        roomNameTxt.text = PhotonNetwork.CurrentRoom.Name;
        MenuManager.Instance.OpenMenu("RoomMenu");
         
        startGameBtn.SetActive(PhotonNetwork.IsMasterClient); //Hides the button if you arent the room owner

        foreach (Transform item in playerListContent) {
            Destroy(item.gameObject);
        }

        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++) {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient) {
        startGameBtn.SetActive(PhotonNetwork.IsMasterClient); //Shows the button if you are the new room owner
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("Loading");
    }
    public override void OnLeftRoom() {
        MenuManager.Instance.OpenMenu("TitleMenu");
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        MenuManager.Instance.OpenMenu("Error");
        errorTxt.text = "An error has occured when trying to create your room with the following code: " + returnCode + "\n" + message;

    }

    //Finding rooms
    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        foreach (Transform item in roomListContent) {
            Destroy(item.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++) {
            if (roomList[i].RemovedFromList) {
                continue;
            }
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public void JoinRoom(RoomInfo info) {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("Loading");

    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void StartGame() {
        PhotonNetwork.LoadLevel(1);
    }
}
