using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Distance : MonoBehaviour {

    [Header("Other Scripts")]
    public PlayerScript player;

    [Header("UI")]
    public TMP_Text camp_text;
    public TMP_Text cur_text;
    public TMP_Text max_text;

    // Refreshes the rader view
    public void Refresh () {
        if (player.camp_available) camp_text.text = "|\n" + player.cur_pos;
        else camp_text.text = "|\n" + player.camp_pos;
        cur_text.text = "|\n" + player.cur_pos;
        max_text.text = "|\n" + (player.max_pos - 1);
    }
}
