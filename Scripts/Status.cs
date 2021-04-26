using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Status : MonoBehaviour {

    [Header("Other Scripts")]
    public PlayerScript player;

    [Header("Status")]
    public Slider energy;
    public TMP_Text e_text;
    public Slider food;
    public TMP_Text f_text;
    public Slider oxygen;
    public TMP_Text o_text;

    // Refreshes the sliders and info tabs
    public void Refresh() {
        energy.value = player.energy / player.nrg_max;
        food.value = player.food / player.food_max;
        oxygen.value = player.oxygen / player.oxy_max;
        e_text.text = "Energy\n(" + player.energy + " / " + player.nrg_max + ")";
        f_text.text = "Food\n(" + player.food + " / " + player.food_max + ")";
        o_text.text = "Oxygen\n(" + player.oxygen + " / " + player.oxy_max + ")";
    }
}
