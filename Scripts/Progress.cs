using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Progress : MonoBehaviour {

    [Header("Other Scripts")]
    public Turn turn;

    [Header("Event Variables")]
    public int length;
    public int escape_index;
    public int mission_index;
    public bool locked;

    [Header("UI")]
    public TMP_Text escape_prg;
    public TMP_Text mission_prg;

    // Update is called once per frame
    void Update () {
        if (!locked && turn) {
            escape_index = turn.escape_index;
            mission_index = turn.mission_index;
        }
        string new_escape = "", new_mission = "";
        for (int i = 0; i < length; i++) {
            if (i < escape_index) new_escape += "+ ";
            else new_escape += "- ";
            if (i < mission_index) new_mission += "+ ";
            else new_mission += "- ";

            if (i == 4) {
                new_escape += '\n';
                new_mission += '\n';
            }
            escape_prg.text = new_escape.Substring(0, new_escape.Length - 1);
            mission_prg.text = new_mission.Substring(0, new_mission.Length - 1);
        }
    }
}
