using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private void Awake() {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
