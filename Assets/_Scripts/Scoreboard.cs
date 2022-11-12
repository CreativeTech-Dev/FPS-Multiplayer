using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System;

public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreboardItemPrefab;
    [SerializeField] CanvasGroup canvasGroup;

    Dictionary<Player, ScoreboardItem> scoreboardItems = new Dictionary<Player, ScoreboardItem>();

    private void Start() {
        foreach (Player p in PhotonNetwork.PlayerList) {
            AddScoreboardItem(p);
        }
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void AddScoreboardItem(Player player) {
        ScoreboardItem item = Instantiate(scoreboardItemPrefab, container).GetComponent<ScoreboardItem>();
        item.Initialize(player);
        scoreboardItems[player] = item;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        AddScoreboardItem(newPlayer);
    }
    private void RemoveScoreboardItem(Player player) {
        Destroy(scoreboardItems[player].gameObject);
        scoreboardItems.Remove(player);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        RemoveScoreboardItem(otherPlayer);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            canvasGroup.alpha = 1;
        }
        else if (Input.GetKeyUp(KeyCode.Tab)) {
            canvasGroup.alpha = 0;
        }

    }
}
