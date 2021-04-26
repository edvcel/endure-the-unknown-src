using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour {

    [Header("Animation")]
    public float delay;
    public float length;
    public Animator anim;
    public int scene;

    public void exit() {
        StartCoroutine(Exit());
    }

    public void loop() {
        StartCoroutine(Loop());
    }

    IEnumerator Exit () {
        yield return new WaitForSecondsRealtime(delay);
        anim.SetBool("out", true);
        yield return new WaitForSecondsRealtime(length);
        SceneManager.LoadScene(scene);
    }

    IEnumerator Loop() {
        anim.SetBool("out", true);
        anim.SetBool("loop", true);
        yield return new WaitForSecondsRealtime(0.5f);
        anim.SetBool("out", false);
        yield return new WaitForSecondsRealtime(2 * length - 0.5f);
        anim.SetBool("loop", false);
    }
}
