using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class UIController : MonoBehaviour
{ 

    private bool isSelected = false;
    private float energyheard;
    private int SpeedMod = 1, ShieldMod = 1, AttackMod = 1;
    public static Action<int, int> GetUIMOD;
    
    void Start()
    {
        PlayerBehavior.EnergyUpdated += Handle_EnergyUpdated;
    }

    void Update()
    {
        
    }

    public void Handle_EnergyUpdated(float energy)
    {
        energyheard = energy; 
        print("I heard that i should have this much energy: " + energy);
    }
    public int ChangeModifier(int modifierSelected, int modifierLevel, int posNeg)
    {
        int var=0;
        switch(modifierSelected)
        {
            case 0:
                if (SpeedMod !<= 1) { SpeedMod += (1*posNeg); }
                var = SpeedMod;
                break;
            case 1:
                if (ShieldMod !<= 1) { ShieldMod += (1 * posNeg); }
                var = ShieldMod;
                break;
            case 2:
                if (AttackMod !<= 1) { AttackMod += (1 * posNeg); }
                var = AttackMod;
                break;
        }
        return var;
    }
    



    public void OnDestroy()
    {
        PlayerBehavior.EnergyUpdated -= Handle_EnergyUpdated;
    }
}
