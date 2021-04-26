using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enabler : MonoBehaviour {

    [Header("Objects")]
    public GameObject[] gos;

    public void Disable () {
        for (int i = 0; i < gos.Length; i++) {
            gos[i].SetActive(false);
        }
    }

    public void Enable() {
        for (int i = 0; i < gos.Length; i++) {
            gos[i].SetActive(true);
        }
    }
}
