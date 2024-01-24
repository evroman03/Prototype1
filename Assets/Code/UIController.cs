using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public TMP_Text VariableText;
    private int Speed = 1;
    private int Attack = 1;
    private int Shield = 1;
    private int maxValue = 5;


    public void ToggleUI(string variableType, bool increase)
    {
        if (variableType == "speed")
        {
            if (increase && Speed != maxValue)
            {
                ++Speed;
                VariableText.text = "Speed = " + Speed;
            }

            else if (increase == false && Speed != 1)
            {
                 --Speed;
            }
        }

        else if (variableType == "attack")
        {
            if (increase && Attack != maxValue)
            {
                ++Attack;
            }

            else if (increase == false && Attack != 1)
            {
                --Attack;
            }
        }

        else if (variableType == "shield")
        {
            if (increase && Shield != maxValue)
            {
                ++Shield;
            }

            else if (increase == false && Shield != 1)
            {
                --Shield;
            }
        }
    }
}
