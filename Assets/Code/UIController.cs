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

    float energyheard; 
    // Start is called before the first frame update
    void Start()
    {
        PlayerBehavior.EnergyUpdated += Handle_EnergyUpdated;
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Handle_EnergyUpdated(float energy)
    {
        energyheard = energy; 
        print("I heard that i should have this much energy: " + energy);
    }

    public void OnDestroy()
    {
        PlayerBehavior.EnergyUpdated -= Handle_EnergyUpdated;

    }

    


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
