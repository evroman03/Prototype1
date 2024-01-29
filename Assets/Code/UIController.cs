using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{ 
    private float energyForUI=0, speedForUI=0, modifierSelected=0;
    private int speedMod = 1, shieldMod = 1, attackMod = 1;
    private Vector3 rotationEuler;

    //Grabs the UI Icons
    [SerializeField] private GameObject speedomter;
    [SerializeField] private Slider sliderObject;
    [SerializeField] private GameObject speedIconUI;
    [SerializeField] private GameObject attackIconUI;
    [SerializeField] private GameObject shieldIconUI;

    //Grabs the UI level bars
    
    [SerializeField] private GameObject speedLevelUI;
    [SerializeField] private GameObject attackLevelUI;
    [SerializeField] private GameObject shieldLevelUI;
    

    //The max speed of the car. Calibrates the speedometer off this value
    [SerializeField] private float maxSpeed;

    //Gets the wanted color for the different UI components
    [SerializeField] private Color speedUIColor;
    [SerializeField] private Color attackUIColor;
    [SerializeField] private Color shieldUIColor;

    public static Action<int, int> GetUIMOD;

    //Used for math to calibrate speedometer - do not change
    private const int DEFAULT_MAX_SPEED = 237;

    float needleAngleModifier;
    void Start()
    {
        PlayerBehavior.EnergyUpdated += Handle_EnergyUpdated;
        PlayerBehavior.SpeedUpdated += HandleSpeedUpdated;
        PlayerBehavior.SelectAttack += UISelectAttack;
        PlayerBehavior.SelectShield += UISelectShield;
        PlayerBehavior.SelectSpeed += UISelectSpeed;
        PlayerBehavior.SelectLeft += UISelectLeft;
        PlayerBehavior.SelectRight += UISelectRight;

        speedIconUI.transform.GetChild(0).GetComponent<Image>().enabled = false;
        speedIconUI.transform.GetChild(1).GetComponent<Image>().enabled = true;

        //Calibrates needle to display speeds from mimimum speed to maximum speed
        needleAngleModifier = maxSpeed / DEFAULT_MAX_SPEED;
    }

    //Minimum and maximum angles on the speedometer
    const float MINIMUM_ANGLE = 0;
    const float MAXIMUM_ANGLE = 237;
    float angle = 0;
    void Update()
    {
        sliderObject.value = energyForUI;

        //Gets how much the needle needs to rotate
        //rotationEuler += Vector3.back * 30 * Time.deltaTime;

        //Changes needle angle
        angle = Mathf.Lerp(MINIMUM_ANGLE, MAXIMUM_ANGLE, Mathf.InverseLerp(MINIMUM_ANGLE, MAXIMUM_ANGLE, speedForUI / needleAngleModifier));
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
                if ((speedMod + (1 * posNeg)) > 0 && (speedMod + (1 * posNeg)) < 6) {
                    speedMod += (1 * posNeg);
                }
                var = speedMod;
                GetUIMOD?.Invoke(0, speedMod);
                print("Speed" + speedMod);

                speedIconUI.transform.GetChild(0).GetComponent<Image>().enabled = false;
                speedIconUI.transform.GetChild(1).GetComponent<Image>().enabled = true;

                attackIconUI.transform.GetChild(0).GetComponent<Image>().enabled = false;
                attackIconUI.transform.GetChild(1).GetComponent<Image>().enabled = false;

                shieldIconUI.transform.GetChild(0).GetComponent<Image>().enabled = false;
                shieldIconUI.transform.GetChild(1).GetComponent<Image>().enabled = false;
                break;
            case 1:
                if ((shieldMod + (1 * posNeg)) > 0 && (shieldMod + (1 * posNeg)) < 6) { 
                    shieldMod += (1 * posNeg);
                }
                var = shieldMod;
                GetUIMOD?.Invoke(1, shieldMod);
                print("Shield" + shieldMod);

                speedIconUI.transform.GetChild(0).GetComponent<Image>().enabled = false;
                speedIconUI.transform.GetChild(1).GetComponent<Image>().enabled = false;

                attackIconUI.transform.GetChild(0).GetComponent<Image>().enabled = false;
                attackIconUI.transform.GetChild(1).GetComponent<Image>().enabled = true;

                shieldIconUI.transform.GetChild(0).GetComponent<Image>().enabled = false;
                shieldIconUI.transform.GetChild(1).GetComponent<Image>().enabled = false;
                break;
            case 2:
                if ((attackMod + (1 * posNeg)) > 0 && (attackMod + (1 * posNeg)) < 6) {
                    attackMod += (1 * posNeg);
                }
                var = attackMod;
                GetUIMOD?.Invoke(2,attackMod);
                print("Attack"+ attackMod);

                speedIconUI.transform.GetChild(0).GetComponent<Image>().enabled = false;
                speedIconUI.transform.GetChild(1).GetComponent<Image>().enabled = false;

                attackIconUI.transform.GetChild(0).GetComponent<Image>().enabled = false;
                attackIconUI.transform.GetChild(1).GetComponent<Image>().enabled = false;

                shieldIconUI.transform.GetChild(0).GetComponent<Image>().enabled = false;
                shieldIconUI.transform.GetChild(1).GetComponent<Image>().enabled = true;
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
