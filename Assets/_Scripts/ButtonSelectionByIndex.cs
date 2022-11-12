using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ButtonSelectionByIndex : MonoBehaviour
{
    Button btn;
    private void Start() {
        btn = GetComponent<Button>();
    }
    public void SetButtonImage(Sprite img) {
        if (btn == null) {
            btn = GetComponent<Button>();
        }
        btn.image.sprite = img;
        //btn.GetComponentInChildren<TMPro.TMP_Text>().text = i.ToString();
    }

    public void SetButtonColor(Color color) {
        if (btn == null) {
            btn = GetComponent<Button>();
        }
        btn.image.color = color;
        //btn.GetComponentInChildren<TMPro.TMP_Text>().text = i.ToString();
    }

    public void SetButtonClickEventWithInt(string method, int index) {
        if (method == "image") {
            btn.onClick.AddListener(() => SetCrosshairImage(index));//(delegate { SetCrosshairImage(i); });}
            btn.GetComponentInChildren<TMPro.TMP_Text>().text = index.ToString();

        }
        if (method == "color") {
            btn.onClick.AddListener(() => SetCrosshairColor(index));//(delegate { SetCrosshairImage(i); });}
        }
    }
    public void SetCrosshairImage(int index) {
        Debug.Log("Image index: " + index);
        PlayerPrefs.SetInt("crosshairImages", index);
    }



    public void SetCrosshairColor(int index) {
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("CrosshairImgBtn")) {
            item.GetComponent<Image>().color = CrosshairManager.Instance.crosshairColors[index];
        }
        Debug.Log(CrosshairManager.Instance.crosshairColors[index].ToString());
        PlayerPrefs.SetInt("crosshairColor", index);
    }



}