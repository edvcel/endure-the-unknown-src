using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#pragma warning disable IDE1006 // Naming Styles
public class PlayerScript : MonoBehaviour {

    // (0, +2, -1)

    [Header("Other Scripts")]
    public Turn turn;
    public Log log;
    public Status status;
    public TMP_Text dice_text;
    public Distance distance;
    public Progress progress;

    [Header("Manage")]
    public float energy;
    public float food;
    public float oxygen;

    [Header("Max")]
    public float nrg_max;
    public float food_max;
    public float oxy_max;

    [Header("Decrements")]
    public float nrg_dec;
    public float food_dec;
    public float oxy_dec;

    [Header("Flags")]
    public bool camp_available;
    public bool tired;
    public bool malnourished;

    [Header("Exploration")]
    public int cur_pos;
    public int camp_pos;
    public int max_pos;

    // Update is called once per frame
    void Update () {
        // Manages energy range
        if (energy < 0) {
            energy = 0;
        }
        if (tired && energy > 0) {
            tired = false;
            log.addEntry("You are no longer tired!", false);
            turn.setEventPercent();
        }
        if (energy == 0 && !tired) {
            tired = true;
            log.addEntry("Warning! Energy outlets will now consume oxygen!", false);
            turn.setEventPercent();
        }

        if (energy > nrg_max) {
            energy = nrg_max;
        }

        // Manages food range
        if (food < 0) {
            food = 0;
        }
        if (malnourished && food > 0) {
            malnourished = false;
            log.addEntry("You are no longer malnourished!", false);
            turn.setEventPercent();
        }
        if (food == 0 && !malnourished) {
            malnourished = true;
            log.addEntry("Warning! Food outlets will now consume oxygen!", false);
            turn.setEventPercent();
        }

        if (food > food_max) {
            food = food_max;
        }

        // Manages oxygen range
        if (oxygen <= 0) {
            oxygen = 0;
            status.Refresh();
            // GAME OVER
            log.lockQueue("\nThe astronaut did not survive.\nGame Over!");
            Destroy(turn.gameObject);
            turn.turn_text.text = "";
            Destroy(gameObject);
        }
        if (oxygen > oxy_max) {
            oxygen = oxy_max;
        }
        status.Refresh();
    }

    // Updates "Explore" & "Retreat to camp" messages
    public void UpdateTurn () {
        int delta = max_pos - cur_pos;
        turn.choices[0] = "Explore\n(" + ( -1 * delta ) + "; 0; " + ( -1 * delta ) + ")";
        delta = cur_pos - camp_pos;
        turn.choices[1] = "Retreat to camp\n(" + ( -1 * delta ) + "; 0; " + ( -1 * delta ) + ")";
    }

    // Multiplier determines how many times oxy_dec will be taken away
    public void makeTurn (float mult) {
        oxygen -= oxy_dec * mult;
    }

    // Costs food, recovers energy
    public void campRest () {
        energy = nrg_max;

        if (malnourished) oxygen -= oxy_dec;
        else food -= food_dec;

        log.addEntry("You slept for a while");
        log.addEntry("Your energy has fully recovered");
        if (turn.is_tutorial) turn.tutorial.ready = true;
        else turn.inCamp();
    }

    // Unlocks the ability of setting up the camp | while holding camp no events spawn
    public void campTeardown () {
        if (tired) oxygen -= oxy_dec;
        else energy -= nrg_dec;


        if (malnourished) oxygen -= oxy_dec;
        else food -= food_dec;

        camp_available = true;

        log.addEntry("You have collected your camp for transport");
        log.addEntry("Until you set the camp up, you will ignore all events");
        turn.setCurrent(turn.nowhere);
    }

    // Puts up the camp at current position
    public void campBuild () {
        if (tired) oxygen -= oxy_dec;
        else energy -= nrg_dec;

        if (malnourished) oxygen -= oxy_dec;
        else food -= food_dec;

        camp_available = false;
        camp_pos = cur_pos;

        log.addEntry("The camp is set up at your current location");
        if (turn.is_tutorial) turn.tutorial.ready = true;
        else turn.inCamp();
    }

    // Costs energy, increases distance
    public void explore () {
        int delta = max_pos - cur_pos;
        cur_pos = max_pos;
        max_pos++;

        if (tired) oxygen -= oxy_dec * delta;
        else energy -= nrg_dec * delta;

        makeTurn(delta - 1);

        UpdateTurn();
        distance.Refresh();
        if (camp_available) {
            turn.setEvent(turn.event_count);
            camp_pos = cur_pos;
        } else turn.setEvent(dx(100) % ( turn.event_count + 1 ));
    }

    // Goes back to the camp
    public void retreat () {
        int delta = cur_pos - camp_pos;
        cur_pos = camp_pos;

        if (tired) oxygen -= oxy_dec * delta;
        else energy -= nrg_dec * delta;

        makeTurn(delta - 1);

        UpdateTurn();
        distance.Refresh();
        turn.inCamp();
    }

    // Gather food, costs energy
    public void gather () {
        int dice = dx(100);
        dice_text.text = dice.ToString();

        if (tired) oxygen -= oxy_dec * 2;
        else energy -= nrg_dec * 2;

        int mult = 1;
        if (dice > 90) {
            mult = 3;
            log.addEntry("You found an extraordinary amount of food!!!", false);
        } else if (dice > 75) {
            mult = 2;
            log.addEntry("You gathered twice as much as you expected!", false);
        } else {
            log.addEntry("You managed to collect a fair amount of food");
        }
        food += food_dec * 2 * mult;

        turn.setCurrent(turn.nowhere);
    }

    // Event functions

    // Gives 5 * oxy_dec
    public void eventOxygen () {
        log.addEntry("You gather all the oxygen into your air tanks");
        oxygen += oxy_dec * 6;
        turn.setCurrent(turn.nowhere);
    }

    // Gives food (double of decrement)
    public void eventFilter (int mishap) {
        log.addEntry("You set up a filter and wait");
        int dice = dx(100);
        dice_text.text = dice.ToString();
        if (dice > mishap) {
            log.addEntry("You have gained more water");
            food += food_dec * 2;
        } else {
            log.addEntry("Your new water got contaminated, your water reserves haven't changed");
        }
        turn.setCurrent(turn.nowhere);
    }

    // Gives both food and energy
    public void eventDebris (int mishap) {
        energy -= nrg_dec;
        log.addEntry("You try to find usable supplies");

        int dice = dx(100);
        dice_text.text = dice.ToString();
        if (dice > mishap) {
            log.addEntry("You uncover some energy boosting food supplements");
            energy += nrg_dec * 3;
            food += food_dec;
        } else {
            log.addEntry("None of the supplies are usable, but there was some oxygen in the wreck");
            oxygen += oxy_dec * 3;
        }
        turn.setCurrent(turn.nowhere);
    }

    // Gives both food and energy
    public void eventMap (int mishap) {
        energy -= nrg_dec;
        food -= food_dec;
        log.addEntry("You try to add in the new corridors to your map");

        int dice = dx(100);
        dice_text.text = dice.ToString();
        if (dice > mishap) {
            log.addEntry("You have found a much shorter path back to the camp");
            camp_pos = --cur_pos;
            distance.Refresh();
            UpdateTurn();
            turn.inCamp();
        } else {
            log.addEntry("There was nothing interesting in the new area");
            turn.setCurrent(turn.nowhere);
        }
    }

    // Expansion events
    public void eventExpand(int dice) {
        switch(dice) {
            case 0:
                eventTank();
                break;
            case 1:
                eventBag();
                break;
            case 2:
                eventShot();
                break;
        }
    }

    public void eventTank () {
        log.addEntry("You pick up the oxygen tank and hook it up to your suit");
        oxy_max += oxy_dec * 10;
        oxygen += oxy_dec * 11;
        turn.setCurrent(turn.nowhere);
    }

    public void eventBag () {
        log.addEntry("You add to both your storage and supplies with the new bag");
        food_max += food_dec * 3;
        food += food_dec * 3;
        turn.setCurrent(turn.nowhere);
    }

    public void eventShot () {
        log.addEntry("You inject yourself with the adrenaline");
        log.addEntry("You feel much more energetic");
        nrg_max += nrg_dec * 5;
        energy += nrg_dec * 5;
        turn.setCurrent(turn.nowhere);
    }

    // Escape events
    public void escape (int index, int mishap) {
        int dice;
        switch (index) {
            case 0: // Find broken transmitter
                dice = dx(100);
                dice_text.text = dice.ToString();
                if (dice > mishap) {
                    log.addEntry("It would be possible to fix the internal damages, if you could find some scrap from your crash landing");
                    turn.escape_index++;
                } else log.addEntry("Your prodding of the object has caused damage that could not be fixed without sophisticated machinery");
                break;
            case 1: // Debris for transmitter
                energy -= nrg_dec * 2;
                food -= food_dec;
                log.addEntry("You have found pieces that would help bodge the transmitter back together");
                turn.escape_index++;
                break;
            case 2: // Fix transmitter
                energy -= nrg_dec;
                dice = dx(100);
                dice_text.text = dice.ToString();
                if (dice > mishap) {
                    log.addEntry("The device is working, but needs an area with a clearer sky to send messages");
                    turn.escape_index++;
                } else log.addEntry("Though the inner machinery seems to be fine, the device does not boot up, you will need to spend more time tinkering");
                break;
            case 3: // Send out SOS
                log.addEntry("You leave the device constantly sending an SOS, but you are going to need a receiver to parse the returning messages");
                turn.escape_index++;
                break;
            case 4: // Find receiver
                energy -= nrg_dec;
                food += food_dec;
                oxygen += oxy_dec * 2;
                dice = dx(100);
                dice_text.text = dice.ToString();
                if (dice > mishap) {
                    log.addEntry("You have not only found food and oxygen, but also a receiver (-1; +1; +2)");
                    turn.escape_index++;
                } else log.addEntry("You could only harvest some food and oxygen (-1; +1; +2)");
                break;
            case 5: // Read receiver
                log.addEntry("The message reads: \"To all survivors, send out a message with your codename\"");
                turn.escape_index++;
                break;
            case 6: // Send out codename
                energy -= nrg_dec;
                dice = dx(100);
                dice_text.text = dice.ToString();
                if (dice > mishap) {
                    log.addEntry("With the receiver's help you find the transmitter and send out your codename");
                    turn.escape_index++;
                } else log.addEntry("The transmitter seems to be somewhere else");
                break;
            case 7: // New message
                log.addEntry("They are coordinates encoded in your spacecraft's cipher");
                turn.escape_index++;
                break;
            case 8:
                dice = dx(100);
                dice_text.text = dice.ToString();
                if (dice > mishap) {
                    log.addEntry("You find a plausible route to the rendezvous spot");
                    turn.escape_index++;
                } else log.addEntry("Your map has too many blank spots to determine how to reach the location. You should try again later");
                break;
            case 9:
                energy -= nrg_dec * 3;
                food -= food_dec * 2;
                makeTurn(4);
                if (oxygen > 0) {
                    // VICTORY::ESCAPE
                    progress.locked = true;
                    progress.escape_index++;
                    turn.play(turn.escape_clip, 1f);
                    log.lockQueue("\nYou take off away from the labyrinth and manage to dock with your crew with their help.\n" +
                                    "You managed to survive the planet's labyrinth but its purpose remains unclear!");
                    Destroy(turn.gameObject);
                    Destroy(gameObject);
                }
                break;
        }
        turn.setCurrent(turn.nowhere);
    }

    // Mission events
    public void mission (int index, int mishap) {
        int dice;
        switch (index) {
            case 0: // Communicate
                dice = dx(100);
                dice_text.text = dice.ToString();
                if (dice > mishap) {
                    log.addEntry("The creature gives you a blank piece of paper and points deeper into the labyrinth");
                    turn.mission_index++;
                } else log.addEntry("The creature does not respond");
                break;
            case 1: // Analyze mural
                energy -= nrg_dec;
                dice = dx(100);
                dice_text.text = dice.ToString();
                if (dice > mishap) {
                    log.addEntry("The mural shows a group of robots burning paper in front of a metal door");
                    turn.mission_index++;
                } else log.addEntry("The mural is too damaged to interpret");
                break;
            case 2: // Burn paper
                makeTurn(2);
                log.addEntry("The fire needed some oxygen, but the door slowly opens after the paper is gone");
                turn.mission_index++;
                break;
            case 3: // Relax
                makeTurn(1);
                energy += nrg_dec * 10;
                food -= food_dec * 3;
                log.addEntry("After resting your eyes for a while you find yourself surrounded by robots who start to flee");
                turn.mission_index++;
                break;
            case 4: // Confront robots
                energy -= nrg_dec * 2;
                dice = dx(100);
                dice_text.text = dice.ToString();
                if (dice > mishap) {
                    food += food_dec * 3;
                    log.addEntry("You damage or grab most of them and recover your food as well as a strange device");
                    turn.mission_index++;
                } else log.addEntry("All manage to evade your grabs and flee deeper into the caves");
                break;
            case 5: // Check radar
                log.addEntry("The device seems to beep when pointing to the north");
                turn.mission_index++;
                break;
            case 6: // Bread down wall
                energy -= nrg_dec;
                food -= food_dec;
                dice = dx(100);
                dice_text.text = dice.ToString();
                if (dice > mishap) {
                    food += food_dec * 3;
                    log.addEntry("The wall easily breaks to reveal a new part of the labyrinth");
                    turn.mission_index++;
                } else log.addEntry("The wall does not give. You need to approach it from a different spot");
                break;
            case 7: // Search the controls
                log.addEntry("Among the many active monitors you find out how they EMPed your pod and destroy those controls");
                turn.mission_index++;
                break;
            case 8: // Fight them off
                energy -= nrg_dec * 2;
                food -= food_dec;
                dice = dx(100);
                dice_text.text = dice.ToString();
                if (dice > mishap) {
                    log.addEntry("You fight them back and find them carrying plans to EMP your spacecraft via nuke");
                    turn.mission_index++;
                } else {
                    energy -= nrg_dec;
                    log.addEntry("The robots get the better of you and after hurting you, retreat (-1; 0; 0)");
                }
                break;

            case 9:
                energy -= nrg_dec * 3;
                food -= food_dec * 2;
                makeTurn(2);
                if (oxygen > 0) {
                    // VICTORY::MISSION
                    progress.locked = true;
                    progress.mission_index++;
                    turn.play(turn.mission_clip, 1f);
                    log.lockQueue("\nYou throw the nuke off its trajectory yet it still manages to disable your spacecraft.\n" +
                                  "The spacecraft crash lands, but you can detect some survivors.\n" +
                                  "You disable the robots and the labyrinth, but are now stranded with the surviving crew on the planet until your signal reaches some spaceship!");
                    Destroy(turn.gameObject);
                    Destroy(gameObject);
                }
                break;
        }
        turn.setCurrent(turn.nowhere);
    }

    // Dice rolls with dx [1; x]
    public int dx (int x) {
        return Random.Range(1, x + 1);
    }
}
#pragma warning restore IDE1006 // Naming Styles