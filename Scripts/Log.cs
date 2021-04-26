using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Log : MonoBehaviour {

    [Header("Other Scripts")]
    public Turn turn;
    public TMP_Text dice_text;
    public Fade fade;

    [Header("UI")]
    public TMP_Text log_text;

    [Header("Queue")]
    public float init_delay;
    private bool initialized;

    public Queue<string> queue;
    public string pre_text;

    private float passed;
    public bool letter_available;
    public float entry_delay;
    public float print_delay;

    [Header("Status")]
    private bool locked;
    public bool empty;

    // Update is called once per frame
    void Update () {
        if (!initialized) {
            empty = false;
            if (passed < init_delay) passed += Time.deltaTime;
            else {
                initialized = true;
                passed = 0;
            }
        } else {
            if (letter_available) {
                empty = false;
                if (passed < print_delay) passed += Time.deltaTime;
                else {
                    letter_available = nextLetter();
                    passed = 0;
                }
            } else if (queue != null && queue.Count > 0) {
                empty = false;
                if (passed < entry_delay) passed += Time.deltaTime;
                else {
                    letter_available = nextEntry();
                    passed = 0;
                }
            } else {
                empty = true;
                if (locked) {
                    fade.exit();
                    turn.turn_text.text = "";
                    Destroy(this);
                }
                turn.setAvailable(true);
                dice_text.text = "";
            }
        }
    }

    // Enqueues entry
    public void addEntry(string str, bool dot = true) {
        if (locked) return;
        if (dot) queue.Enqueue(str + '.');
        else queue.Enqueue(str);
    }

    public void lockQueue(string str) {
        queue = new Queue<string>();
        queue.Enqueue(str);
        locked = true;
    }

    // Dequeues next entry into string
    bool nextEntry() {
        if (queue.Count == 0) return false;
        string str = queue.Dequeue();
        pre_text += '\n' + str;
        return true;
    }

    // Returns false if letter was not passed, true if no error
    bool nextLetter() {
        if (pre_text.Length == 0) return false;
        log_text.text += pre_text[0];
        if (log_text.text.Length % 2 == 0) turn.play(turn.type_clip, 0.5f);
        pre_text = pre_text.Substring(1);
        return pre_text.Length != 0;
    }
}
