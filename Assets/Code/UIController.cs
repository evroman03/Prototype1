using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class UIController : MonoBehaviour
{ 
    private bool isSelected = false;
    private float energyForUI=0, speedForUI=0, modifierSelected=0;
    private int SpeedMod = 1, ShieldMod = 1, AttackMod = 1;
    public static Action<int, int> GetUIMOD;
    void Start()
    {
        PlayerBehavior.EnergyUpdated += Handle_EnergyUpdated;
        PlayerBehavior.SpeedUpdated += HandleSpeedUpdated;
        PlayerBehavior.SelectAttack += UISelectAttack;
        PlayerBehavior.SelectShield += UISelectShield;
        PlayerBehavior.SelectSpeed += UISelectSpeed;
        PlayerBehavior.SelectLeft += UISelectLeft;
        PlayerBehavior.SelectRight += UISelectRight;
    }

    void Update()
    {

    }
    public void Handle_EnergyUpdated(float energy)
    {
        energyForUI = energy; 
        //print("I heard that i should have this much energy: " + energyheard);
    }
    public void HandleSpeedUpdated(float speed)
    {
        speedForUI= speed;
        //print("Current speed: " + speedForUI);
    }
    public void ChangeModifier(int posNeg)
    {
        int var=0;
        switch(modifierSelected)
        {
            case 0:
                if ((SpeedMod + (1 * posNeg)) > 0 && (SpeedMod + (1 * posNeg)) < 6) { SpeedMod += (1 * posNeg); }
                var = SpeedMod;
                GetUIMOD?.Invoke(0, SpeedMod);
                print("Speed" + SpeedMod);
                break;
            case 1:
                if ((ShieldMod + (1 * posNeg)) > 0 && (ShieldMod + (1 * posNeg)) < 6) { ShieldMod += (1 * posNeg); }
                var = ShieldMod;
                GetUIMOD?.Invoke(1, ShieldMod);
                print("Shield" + ShieldMod);
                break;
            case 2:
                if ((AttackMod + (1 * posNeg)) > 0 && (AttackMod + (1 * posNeg)) < 6) { AttackMod += (1 * posNeg); }
                var = AttackMod;
                GetUIMOD?.Invoke(2,AttackMod);
                print("Attack"+ AttackMod);
                break;
               
        }
        //CAN YOU READ THIS
    }
    public void UISelectSpeed()
    {
        modifierSelected = 0;
    }
    public void UISelectShield()
    {
        modifierSelected = 1;
    }
    public void UISelectAttack()
    {
        modifierSelected = 2;
    }
    public void UISelectRight()
    {
        ChangeModifier(1);
    }
    public void UISelectLeft()
    {
        ChangeModifier(-1);
    }



    public void OnDestroy()
    {
        PlayerBehavior.EnergyUpdated -= Handle_EnergyUpdated;
        PlayerBehavior.SpeedUpdated -= HandleSpeedUpdated;
        PlayerBehavior.SelectAttack -= UISelectAttack;
        PlayerBehavior.SelectShield -= UISelectShield;
        PlayerBehavior.SelectSpeed -= UISelectSpeed;
        PlayerBehavior.SelectLeft -= UISelectLeft;
        PlayerBehavior.SelectRight -= UISelectRight;
    }
}
