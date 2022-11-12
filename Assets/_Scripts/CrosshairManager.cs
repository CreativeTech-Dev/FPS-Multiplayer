using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    public static CrosshairManager Instance;
    public Sprite[] crosshairImages;
    [SerializeField] Transform crosshairContainer;
    [SerializeField] GameObject crosshairBtnPrefab;

    public Color[] crosshairColors;
    [SerializeField] Transform crosshairColorContainer;
    [SerializeField] GameObject crosshairColorBtnPrefab;


    

    private void Start() {
        Instance = this;

        if (!PlayerPrefs.HasKey("crosshairImages")) {
            PlayerPrefs.SetInt("crosshairImages", 0);
        }
        if (!PlayerPrefs.HasKey("crosshairColor")) {
            PlayerPrefs.SetInt("crosshairColor", 0);
        }

        for (int i = 0; i < crosshairImages.Length; i++) {
            ButtonSelectionByIndex crosshairItem = Instantiate(crosshairBtnPrefab, crosshairContainer).GetComponent<ButtonSelectionByIndex>();
            if (PlayerPrefs.HasKey("crosshairColor")) {
                crosshairItem.SetButtonColor(crosshairColors[PlayerPrefs.GetInt("crosshairColor")]);
            }
            crosshairItem.SetButtonImage(crosshairImages[i]);
            crosshairItem.SetButtonClickEventWithInt("image", i); //onClick.AddListener(() => SetCrosshairImage(i));//(delegate { SetCrosshairImage(i); });
        }

        for (int j = 0; j < crosshairColors.Length; j++) {
            ButtonSelectionByIndex crosshairColorItem = Instantiate(crosshairColorBtnPrefab, crosshairColorContainer).GetComponent<ButtonSelectionByIndex>();
            crosshairColorItem.SetButtonColor(crosshairColors[j]); //image.color = crosshairColors[j];
            crosshairColorItem.SetButtonClickEventWithInt("color", j); //onClick.AddListener(() => SetCrosshairColor(j));
        }
    }

/*
    public void SetCrosshairImage(int index) {
        Debug.Log("Image index: " + index);
        PlayerPrefs.SetInt("crosshairImages", index);
    }
*/
/*
    public void SetCrosshairColor(int colorIndex) {
        Debug.Log("colorindex " + colorIndex);
        PlayerPrefs.SetString("crosshairColor", crosshairColors[colorIndex].ToString());
        foreach (Button item in crosshairContainer.GetComponentsInChildren<Button>()) {
            item.image.color = crosshairColors[colorIndex];
        }
    }
*/
}
