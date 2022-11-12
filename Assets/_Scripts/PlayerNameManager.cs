using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField usernameInputField;

    private void Start() {
        if (PlayerPrefs.HasKey("Username")) {
            usernameInputField.text = PlayerPrefs.GetString("Username");
        }
        else {
            usernameInputField.text = "Player " + Random.Range(0, 1000).ToString("000");
            OnUsernameValueChanged();
        }
    }

    public void OnUsernameValueChanged() {
        PhotonNetwork.NickName = usernameInputField.text;
        PlayerPrefs.SetString("Username", usernameInputField.text);
    }

}
