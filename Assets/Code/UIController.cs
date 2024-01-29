using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{ 
    private bool isSelected = false;
    private float energyForUI=0, speedForUI=0, modifierSelected=0;
    private int speedMod = 1, shieldMod = 1, attackMod = 1;
    private Vector3 rotationEuler;

    [SerializeField] private GameObject speedomter;
    [SerializeField] private GameObject speedLevelUI;
    [SerializeField] private GameObject attackLevelUI;
    [SerializeField] private GameObject shieldLevelUI;
    [SerializeField] private Slider sliderObject;

    [SerializeField] private Color speedUIColor;
    [SerializeField] private Color attackUIColor;
    [SerializeField] private Color shieldUIColor;

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

        //Sets needle of spedometer to starting position (180 degrees)
        //spedomter.transform.GetChild(1).GetComponent<Image>().transform.eulerAngles = new Vector3(0, 0, -90);
    }

    //Max Angle to add is 147 degrees (from needle starting position)

    float minAngle = 0;
    float maxAngle = 237;
    float angle = 0;
    void Update()
    {
        sliderObject.value = energyForUI;

        //Gets how much the needle needs to rotate
        //rotationEuler += Vector3.back * 30 * Time.deltaTime;

        //Changes needle rotation
        angle = Mathf.Lerp(minAngle, maxAngle, Mathf.InverseLerp(minAngle, maxAngle, speedForUI));
        speedomter.transform.GetChild(1).GetComponent<Image>().transform.eulerAngles = new Vector3(0, 0, -angle + 90);
    }
    public void Handle_EnergyUpdated(float energy)
    {
        energyForUI = energy; 
        //print("I heard that i should have this much energy: " + energyheard);
    }
    public void HandleSpeedUpdated(float speed)
    {
        speedForUI= speed;
        print("Current speed: " + speedForUI);
    }
    public void ChangeModifier(int posNeg)
    {
        int var=0;
        switch(modifierSelected)
        {
            case 0:
                if ((speedMod + (1 * posNeg)) > 0 && (speedMod + (1 * posNeg)) < 6) { speedMod += (1 * posNeg); }
                var = speedMod;
                GetUIMOD?.Invoke(0, speedMod);
                print("Speed" + speedMod);
                break;
            case 1:
                if ((shieldMod + (1 * posNeg)) > 0 && (shieldMod + (1 * posNeg)) < 6) { shieldMod += (1 * posNeg); }
                var = shieldMod;
                GetUIMOD?.Invoke(1, shieldMod);
                print("Shield" + shieldMod);
                break;
            case 2:
                if ((attackMod + (1 * posNeg)) > 0 && (attackMod + (1 * posNeg)) < 6) { attackMod += (1 * posNeg); }
                var = attackMod;
                GetUIMOD?.Invoke(2,attackMod);
                print("Attack"+ attackMod);
                break;
            default:
                print("ERROR: ChangeModifier() failed. Invalid modifierSelected");
                break;
        }
        UpdateUI();
    }

    /**
     * Updates the UI components
     */
    public void UpdateUI()
    {
        //Gets which component to be modified
        switch (modifierSelected)
        {
            //Speed Component
            case 0:
                //Changes amount of Speed components to active color depending on the level
                for (int i = 0; i < speedMod; i++)
                {
                    speedLevelUI.transform.GetChild(i).GetComponent<Image>().color = speedUIColor;
                }

                //Sets the rest of the Speed components to gray
                for (int i = speedMod; i < speedLevelUI.transform.childCount; i++)
                {
                    speedLevelUI.transform.GetChild(i).GetComponent<Image>().color = Color.gray;
                }
                break;

            //Shield Component
            case 1:
                //Changes amount of Shield components to active color depending on the level
                for (int i = 0; i < shieldMod; i++)
                {
                    shieldLevelUI.transform.GetChild(i).GetComponent<Image>().color = shieldUIColor;
                }

                //Sets the rest of the Shield components to gray
                for (int i = shieldMod; i < shieldLevelUI.transform.childCount; i++)
                {
                    shieldLevelUI.transform.GetChild(i).GetComponent<Image>().color = Color.gray;
                }
                break;
            case 2:
                //Changes amount of Attack components to active color depending on the level
                for (int i = 0; i < attackMod; i++)
                {
                    attackLevelUI.transform.GetChild(i).GetComponent<Image>().color = attackUIColor;
                }

                //Sets the rest of the Attack components to gray
                for (int i = attackMod; i < attackLevelUI.transform.childCount; i++)
                {
                    attackLevelUI.transform.GetChild(i).GetComponent<Image>().color = Color.gray;
                }
                break;
            //If modifierSelected is none of the above
            default:
                print("ERROR: UpdateUI() failed. Invalid modiferSelected.");
                break;
        }
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
