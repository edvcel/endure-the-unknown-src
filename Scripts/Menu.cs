using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {

    [Header("Other Scripts")]
    public Camera cam_main;
    public Fade fade;

    [Header("Audio")]
    public AudioClip click;

    private KeyCode[] keycodes = { KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3 };
    private KeyCode[] alternate = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(keycodes[0]) || Input.GetKeyDown(alternate[0])) load(1);
        else if (Input.GetKeyDown(keycodes[1]) || Input.GetKeyDown(alternate[1])) load(2);
    }

    void load(int scene) {
        fade.scene = scene;
        play(click);
        fade.exit();
        Destroy(gameObject);
    }

    // Plays the given clip at the camera's position
    public void play (AudioClip clip) {
        AudioSource.PlayClipAtPoint(clip, cam_main.transform.position);
    }
}
