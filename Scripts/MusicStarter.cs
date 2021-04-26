using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicStarter : MonoBehaviour {

    public GameObject music_player;

    void Awake () {
        if (!GameObject.Find("MusicPlayer")) {
            GameObject go = Instantiate(music_player);
            DontDestroyOnLoad(go);
        }
    }
}
