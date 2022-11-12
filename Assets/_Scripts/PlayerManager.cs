using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{

    PhotonView pv;
    GameObject controller;

    int myKills = 0;
    int deaths = 0;

    private void Awake() {
        pv = GetComponent<PhotonView>();
    }

    private void Start() {
        if (pv.IsMine) {
            CreateController();
        }
    }

    void CreateController() {
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { pv.ViewID });
    }

    public void Die() {
        PhotonNetwork.Destroy(controller);
        CreateController();

        deaths++;

        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void GetKill() {
        pv.RPC(nameof(RPC_GetKill), pv.Owner);
    }

    [PunRPC]
    void RPC_GetKill() {
        myKills++;

        Hashtable hash = new Hashtable();
        hash.Add("myKills", myKills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

    }

    public static PlayerManager Find(Player player) {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.pv.Owner == player);
    }

}
