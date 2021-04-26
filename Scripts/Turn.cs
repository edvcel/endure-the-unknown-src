using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Turn : MonoBehaviour {

    // Current max available size is 3 !

    [Header("Other Scripts")]
    public PlayerScript player;
    public Log log;
    public Status status;
    public Camera cam_main;
    public Tutorial tutorial;

    [Header("Current")]
    public string[] turns;
    public bool available;
    // The following 2 have to have the same length
    private KeyCode[] keycodes = { KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3 };
    private KeyCode[] alternate = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };

    [Header("Constant")]
    public string camp;
    public string nowhere;
    public string nowhere_wcamp;

    [Header("Choices")]
    [TextArea] public string[] choices;

    [Header("Events")]
    public string[] event_log;
    public string[] event_choices;
    [Range(0, 100)] public int[] event_mishap;
    public int[] event_indices;
    private int mishap;
    public int event_count;

    [Header("Expansions")]
    public string[] expand_text;
    [TextArea] public string[] expand_choices;
    public int expand_index;
    public int expand_dice;

    [Header("Escape")]
    [TextArea] public string[] escape_text;
    [TextArea] public string[] escape_choices;
    [Range(0, 100)] public int[] escape_mishap;
    public int escape_start;
    public int escape_index;

    [Header("Mission")]
    [TextArea] public string[] mission_text;
    [TextArea] public string[] mission_choices;
    [Range(0, 100)] public int[] mission_mishap;
    public int mission_start;
    public int mission_index;

    [Header("Tutorial")]
    public bool is_tutorial;
    public string[] tutorial_str;

    [Header("UI")]
    public TMP_Text turn_text;

    [Header("Audio")]
    public AudioClip select_clip;
    public AudioClip type_clip;
    public AudioClip water_clip;
    public AudioClip escape_clip;
    public AudioClip mission_clip;

    // The awake that manages initialization for PlayerScript Turn & Log
    void Awake () {
        if (is_tutorial) return;
        log.queue = new Queue<string>();
        player.max_pos = 1;
        status.Refresh();
        player.UpdateTurn();
        player.distance.Refresh();
        setEventPercent();
        setAvailable(false);
        inCamp();
    }

    // Update is called once per frame
    void Update () {
        if (available) {
            for (int i = 0; i < turns.Length; i++) {
                if (Input.GetKeyDown(keycodes[i]) || Input.GetKeyDown(alternate[i])) analyze(turns[i]);
            }
        }
    }


    // Calls the appropriate function for the string
    void analyze(string str) {
        play(select_clip, 1f);
        turn_text.text = "";
        setAvailable(false);
        int index = 0;
        for (int i = 0; i < choices.Length; i++) {
            if (str == choices[i]) {
                index = i;
                break;
            }
        }

        player.makeTurn(1);
        switch (index) {
            case 0: // Explore
                player.explore();
                break;
            case 1: // Retreat to camp
                player.retreat();
                break;
            case 2: // Rest at camp
                player.campRest();
                break;
            case 3: // Tear down camp
                player.campTeardown();
                break;
            case 4: // Set up camp
                player.campBuild();
                break;
            case 5: // Gather
                player.gather();
                break;
            case 6: // Filter water
                player.eventFilter(mishap);
                break;
            case 7: // Gather oxygen
                player.eventOxygen();
                break;
            case 8: // Investigate debris
                player.eventDebris(mishap);
                break;
            case 9: // Map out area
                player.eventMap(mishap);
                break;
            case 10: // Collect tank
                player.eventExpand(expand_dice);
                break;
            case 11: // Escape event
                player.escape(escape_index, mishap);
                break;
            case 12: // Mission event
                player.mission(mission_index, mishap);
                break;
        }
        status.Refresh();
    }

    // Resets the probability of mishaps
    public void setEventPercent() {
        int mult = 1;
        if (player.tired) mult *= 2;
        if (player.malnourished) mult *= 2;

        // Changes the percentage in choices to be copied to frontend
        for (int i = 0; i < event_indices.Length; i++) {
            if (event_mishap[i] != 0) {
                int prob = 100 - ( event_mishap[i] * mult );
                int index = choices[event_indices[i]].IndexOf('(');
                string str = choices[event_indices[i]].Substring(0, index + 1);
                str += ( prob < 10 ) ? 0.ToString() : ( prob / 10 ).ToString();
                str += ( prob % 10 ).ToString();
                str += choices[event_indices[i]].Substring(index + 3);
                choices[event_indices[i]] = str;
            }
        }

        // Does the same for escape_choices
        for (int i = 0; i < escape_choices.Length; i++) {
            if (escape_mishap[i] != 0) {
                int prob = 100 - ( escape_mishap[i] * mult );
                int index = escape_choices[i].IndexOf('(');
                string str = escape_choices[i].Substring(0, index + 1);
                str += ( prob < 10 ) ? 0.ToString() : ( prob / 10 ).ToString();
                str += ( prob % 10 ).ToString();
                str += escape_choices[i].Substring(index + 3);
                escape_choices[i] = str;
            }
        }

        // And again for mission_choices
        for (int i = 0; i < mission_choices.Length; i++) {
            if (mission_mishap[i] != 0) {
                int prob = 100 - ( mission_mishap[i] * mult );
                int index = mission_choices[i].IndexOf('(');
                string str = mission_choices[i].Substring(0, index + 1);
                str += ( prob < 10 ) ? 0.ToString() : ( prob / 10 ).ToString();
                str += ( prob % 10 ).ToString();
                str += mission_choices[i].Substring(index + 3);
                mission_choices[i] = str;
            }
        }
    }

    // Sets the current possible choices
    public void setCurrent(string str) {
        if (player.camp_available && str == nowhere) str = nowhere_wcamp;

        int length = 0; str += ' ';
        for (int i = 0; i < str.Length; i++) if (str[i] == ' ') length++;
        turns = new string[length];

        int value = 0, index = 0;
        for (int i = 0; i < str.Length; i++) {
            if (str[i] == ' ') {
                turns[index++] = choices[value];
                value = 0;
            } else {
                value *= 10;
                value += getValue(str[i]);
            }
        }
        setText();
    }

    // Call when in camp
    public void inCamp() {
        setCurrent(camp);
        log.addEntry("\nYou are in your camp");
    }

    // Copies the current 'turns' into 'turn_text'
    void setText() {
        turn_text.text = "";
        for (int i = 0; i < turns.Length; i++) {
            turn_text.text += (i + 1) + ". " + turns[i] + '\n';
        }
    }

    // Plays the given clip at the camera's position
    public void play(AudioClip clip, float volume) {
        AudioSource.PlayClipAtPoint(clip, cam_main.transform.position, volume);
    }

    // Allows choosing the turn
    public void setAvailable(bool test) {
        available = test;
        turn_text.enabled = test;
    }

    // Sets the choices and log entries for an event
    public void setEvent (int id) {

        if (id == event_count) {
            log.addEntry("\nYou have found nothing of interest");
            setCurrent(nowhere);
            return;
        }

        if (id == 0) play(water_clip, 1f);

        if (id == expand_index) {

            expand_dice = player.dx(3) - 1;
            log.addEntry('\n' + expand_text[expand_dice]);
            choices[event_indices[id]] = expand_choices[expand_dice];
            setCurrent(event_choices[id]);

        } else if (id == escape_start) {

            log.addEntry("\nProspect for escape!", false);
            log.addEntry(escape_text[escape_index]);
            choices[event_indices[id]] = escape_choices[escape_index];
            int index = escape_choices[escape_index].IndexOf('(');
            mishap = 100 - int.Parse(escape_choices[escape_index].Substring(index + 1, 2));
            setCurrent(event_choices[id]);

        } else if (id == mission_start) {

            log.addEntry("\nProspect for insight!", false);
            log.addEntry(mission_text[mission_index]);
            choices[event_indices[id]] = mission_choices[mission_index];
            int index = mission_choices[mission_index].IndexOf('(');
            mishap = 100 - int.Parse(mission_choices[mission_index].Substring(index + 1, 2));
            setCurrent(event_choices[id]);

        } else {

            log.addEntry('\n' + event_log[id]);
            int index = choices[event_indices[id]].IndexOf('(');
            mishap = 100 - int.Parse(choices[event_indices[id]].Substring(index + 1, 2));
            setCurrent(event_choices[id]);

        }
    }

    // Returns the numeric value of a char
    int getValue(char c) {
        return (int)c - (int)'0';
    }
}
