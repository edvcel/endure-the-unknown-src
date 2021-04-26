using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    [Header("Other Scripts")]
    public PlayerScript player;
    public Turn turn;
    public Log log;
    public Fade fade;
    public GameObject status_go;
    public GameObject choices_go;
    public GameObject distance_go;
    public GameObject event_go;
    public GameObject dice_go;

    [Header("Pacing")]
    public float delay;
    private float delay_start;
    public float passed;

    [Header("Audio")]
    public AudioClip lift_off;
    public AudioClip explosion;

    // Trigger chain
    [HideInInspector] public bool ready;
    [HideInInspector] public int stage;

    void Awake () {
        delay_start = delay;

        log.queue = new Queue<string>();
        log.addEntry("Commence Mission 0x002F1");
        log.addEntry("Briefing: The entire surface of the subject planet is covered with a labyrinth");
        log.addEntry("Our scans show that the labyrinth also extends deeper into the caves of the planet");
        log.addEntry("You, as well as several other explorers, have been sent out into different parts of the labyrinth to investigate");
        log.addEntry("You have all been equipped with a low tech transmitter and receiver that can communicate with the spacecraft");
        log.addEntry("Good Luck!", false);
        ready = true;
    }

    void Update () {
        if (log.empty && ready) {
            if (passed <= delay) passed += Time.deltaTime;
            else {
                ready = false;
                passed = 0f;
                switch(stage) {
                    case 0:
                        Crash();
                        break;
                    case 1:
                        StartCoroutine(CrashOff());
                        break;
                    case 2:
                        Status();
                        break;
                    case 3:
                        StatusOn();
                        break;
                    case 4:
                        Choice();
                        break;
                    case 5:
                        ChoiceOn();
                        break;
                    case 6:
                        Rest();
                        break;
                    case 7:
                        StartCoroutine(CrashOff());
                        break;
                    case 8:
                        Distance();
                        break;
                    case 9:
                        DistanceOn();
                        break;
                    case 10:
                        Event();
                        break;
                    case 11:
                        EventOn();
                        break;
                    case 12:
                        Dice();
                        break;
                    case 13:
                        DiceOn();
                        break;
                    case 14:
                        End();
                        break;

                }
            }
        }
    }

    void Crash () {
        stage++;
        turn.play(lift_off, 1f);
        log.addEntry("\nEntering atmosphere");
        log.addEntry("Predicted trajectory seems accurate");
        log.addEntry("Ejecting parachute");
        log.addEntry("Warning! Several systems offline");
        log.addEntry("Warning! Reserve power going out");
        log.addEntry("Commence emergency ejection!", false);
        ready = true;
    }

    IEnumerator CrashOff() {
        stage++;
        if (stage == 2) turn.play(explosion, 0.5f);
        fade.loop();
        yield return new WaitForSecondsRealtime(1f);
        log.log_text.text = "";
        yield return new WaitForSecondsRealtime(1f);
        delay = 0;
        ready = true;
    }

    void Status () {
        stage++;
        player.energy = 2;
        player.food = 3;
        player.oxygen = 27;
        
        log.addEntry("\nAnalyzing astronaut status");
        log.addEntry("Oxygen storage undamaged");
        log.addEntry("Food levels average");
        log.addEntry("Energy levels low");
        log.addEntry("Launching status view");
        delay = 0;
        ready = true;
    }

    void StatusOn () {
        stage++;
        delay = delay_start;
        status_go.SetActive(true);
        ready = true;
    }

    void Choice() {
        stage++;
        log.addEntry("\nAttention! You need to set up camp and rest to regain energy");
        log.addEntry("To take an action take a look at the \"Your Choices\" panel and press the appropriate number on your keyboard");
        turn.enabled = true;
        turn.setCurrent(turn.tutorial_str[0]);
        delay = 0;
        ready = true;
    }

    void ChoiceOn () {
        stage++;
        delay = delay_start;
        choices_go.SetActive(true);
        turn.setAvailable(true);
    }

    void Rest () {
        stage++;
        turn.setAvailable(false);
        log.addEntry("\nInformation! \"(-1; -1; -1)\" this is a shorthand to give you information on what is the toll of an action");
        log.addEntry("They represent (Energy, Food, Oxygen)");
        log.addEntry("Now you should rest up and regain some energy");
        turn.enabled = true;
        turn.setCurrent(turn.tutorial_str[1]);
        delay = 0;
    }

    void Distance () {
        stage++;
        log.addEntry("You should consider exploring your surroundings");
        log.addEntry("This panel will show at which coordinate the camp, the astronaut and the furthest explored distance are");
        delay = 0;
        ready = true;
    }

    void DistanceOn () {
        stage++;
        delay = delay_start;
        distance_go.SetActive(true);
        log.addEntry("\nFor every increment of distance you will use up (-1; 0; -1) so keep in mind that you can pick up your camp and relocate it");
        ready = true;
    }

    void Event () {
        stage++;
        log.addEntry("\nWhen exploring you will be able to encounter events");
        log.addEntry("Some may help you survive, other escape, others still - complete your mission");
        log.addEntry("I will track your progress on your mission and your escape on this panel");
        log.addEntry("Completing either row will count as victory");
        delay = 0;
        ready = true;
    }

    void EventOn () {
        stage++;
        delay = delay_start;
        event_go.SetActive(true);
        ready = true;
    }

    void Dice () {
        stage++;
        log.addEntry("\nSome choices will not always be successful");
        log.addEntry("The dice panel will help you understand how close you were to success (the higher - the better)");
        delay = 0;
        ready = true;
    }

    void DiceOn () {
        stage++;
        delay = delay_start;
        dice_go.SetActive(true);
        ready = true;
    }

    void End() {
        log.lockQueue("\nGood luck! I hope you survive!");
        Destroy(gameObject);
    }
}
