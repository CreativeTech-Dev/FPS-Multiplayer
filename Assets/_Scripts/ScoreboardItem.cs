using Photon.Realtime;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ScoreboardItem : MonoBehaviourPunCallbacks
{
    public TMP_Text usernameTxt;
    public TMP_Text killsTxt;
    public TMP_Text deathsTxt;

    Player player;

    public void Initialize(Player player) {
        this.player = player;
        usernameTxt.text = player.NickName;
        UpdateScoreboard();
    }

    void UpdateScoreboard() {
        if (player.CustomProperties.TryGetValue("myKills", out object myKills)) {
            killsTxt.text = myKills.ToString();
        }
        if (player.CustomProperties.TryGetValue("deaths", out object deaths)) {
            deathsTxt.text = deaths.ToString();
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        if (targetPlayer == player) {
            if (changedProps.ContainsKey("myKills") || changedProps.ContainsKey("deaths")) {
                UpdateScoreboard();
            }
        }
    }

}
