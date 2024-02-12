using System;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{ 
    private float energyForUI=0, speedForUI=0, modifierSelected=0;
    private int speedMod = 1, shieldMod = 1, attackMod = 1;
    public int ID;
    public PlayerBehavior playerBehavior;
    public SoundController soundController;

    //Grabs the UI Icons
    [SerializeField] private GameObject speedomter;
    [SerializeField] private Slider sliderObject;
    [SerializeField] private GameObject speedIconUI;
    [SerializeField] private GameObject attackIconUI;
    [SerializeField] private GameObject shieldIconUI;

    //Grabs the UI level bars
    
    [SerializeField] private GameObject[] speedLevelUI;
    [SerializeField] private GameObject[] attackLevelUI;
    [SerializeField] private GameObject[] shieldLevelUI;
    

    //The max speed of the car. Calibrates the speedometer off this value
    [SerializeField] private float maxSpeed;

    //Gets the wanted color for the different UI components
    [SerializeField] private Color speedUIColor;
    [SerializeField] private Color attackUIColor;
    [SerializeField] private Color shieldUIColor;

    public Action<int, int> GetUIMOD;

    //Used for math to calibrate speedometer - do not change
    private const int DEFAULT_MAX_SPEED = 237;
    private const int DEFAULT_MAX_MOD = 5;

    float needleAngleModifier;
    void Start()
    {
        if (maxSpeed <= 0)
            print("WARNING: maxSpeed is set to a non-positive number! maxSpeed is currently: " + maxSpeed);
     

        speedForUI = 0;

        //Sets Icons to their starting states
        speedIconUI.transform.Find("SpdIconOff").GetComponent<Image>().enabled = false;
        speedIconUI.transform.Find("SpdIconOn").GetComponent<Image>().enabled = true;

        attackIconUI.transform.Find("AtkIconOn").GetComponent<Image>().enabled = false;

        shieldIconUI.transform.Find("ShIconOn").GetComponent<Image>().enabled = false;

        //Calibrates needle to display speeds from mimimum speed to maximum speed
        needleAngleModifier = maxSpeed / DEFAULT_MAX_SPEED;
    }
    public void InitUI()
    {
        playerBehavior.EnergyUpdated += Handle_EnergyUpdated;
        playerBehavior.SpeedUpdated += HandleSpeedUpdated;
        playerBehavior.SelectAttack += UISelectAttack;
        playerBehavior.SelectShield += UISelectShield;
        playerBehavior.SelectSpeed += UISelectSpeed;
        playerBehavior.SelectLeft += UISelectLeft;
        playerBehavior.SelectRight += UISelectRight;
    }
    //Minimum and maximum angles on the speedometer
    const float MINIMUM_ANGLE = 0;
    const float MAXIMUM_ANGLE = 237;
    float angle = 0;
    void Update()
    {
        sliderObject.value = energyForUI;

        //Changes needle angle
        angle = Mathf.Lerp(MINIMUM_ANGLE, MAXIMUM_ANGLE,  speedForUI / maxSpeed);
        speedomter.transform.Find("Needle").GetComponent<Image>().transform.eulerAngles = new Vector3(0, 0, -angle + 90);
    }
     void Handle_EnergyUpdated(float energy, int playerId)
    {
        if(playerId == ID)
        {
            energyForUI = energy;

        }
        //print("I heard that i should have this much energy: " + energyheard);
    }
    void HandleSpeedUpdated(float speed, int playerId)
    {
        speedForUI= speed;
        //print("Current speed: " + speedForUI);
    }
    void ChangeModifier(int posNeg)
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
                //print("Speed" + speedMod);
                break;
            case 1:
                if ((shieldMod + (1 * posNeg)) > 0 && (shieldMod + (1 * posNeg)) < 6) { 
                    shieldMod += (1 * posNeg);
                }
                var = shieldMod;
                GetUIMOD?.Invoke(1, shieldMod);
                //print("Shield" + shieldMod);
                break;
            case 2:
                if ((attackMod + (1 * posNeg)) > 0 && (attackMod + (1 * posNeg)) < 6) {
                    attackMod += (1 * posNeg);
                }
                var = attackMod;
                GetUIMOD?.Invoke(2,attackMod);
                //print("Attack"+ attackMod);
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
    void UpdateUI()
    {
        
        //Gets which component to be modified
        switch (modifierSelected)
        {
            //Speed Component
            case 0:
                //Changes amount of Speed components to active color depending on the level
                for (int i = 0; i < speedMod; i++)
                {
                    speedLevelUI[i].transform.GetComponent<Image>().color = speedUIColor;
                }

                //Sets the rest of the Speed components to gray
                for (int i = speedMod; i < DEFAULT_MAX_MOD; i++)
                {
                    speedLevelUI[i].transform.GetComponent<Image>().color = Color.gray;
                }
                break;

            //Shield Component
            case 1:
                //Changes amount of Shield components to active color depending on the level
                for (int i = 0; i < shieldMod; i++)
                {
                    shieldLevelUI[i].transform.GetComponent<Image>().color = shieldUIColor;
                }

                //Sets the rest of the Shield components to gray
                for (int i = shieldMod; i < DEFAULT_MAX_MOD; i++)
                {
                    shieldLevelUI[i].transform.GetComponent<Image>().color = Color.gray;
                }
                break;
            case 2:
                //Changes amount of Attack components to active color depending on the level
                for (int i = 0; i < attackMod; i++)
                {
                    attackLevelUI[i].transform.GetComponent<Image>().color = attackUIColor;
                }
                
                //Sets the rest of the Attack components to gray
                for (int i = attackMod; i < DEFAULT_MAX_MOD; i++)
                {
                    attackLevelUI[i].transform.GetComponent<Image>().color = Color.gray;
                }
                break;
            //If modifierSelected is none of the above
            default:
                print("ERROR: UpdateUI() failed. Invalid modiferSelected.");
                break;
        }
    }

    void UISelectSpeed(int playerBehvaiorID)
    {
        if (ID == playerBehvaiorID)
        {
            //Sets the speed icon to active and the rest to inactive
            speedIconUI.transform.Find("SpdIconOff").GetComponent<Image>().enabled = false;
            speedIconUI.transform.Find("SpdIconOn").GetComponent<Image>().enabled = true;

            shieldIconUI.transform.Find("ShIconOff").GetComponent<Image>().enabled = true;
            shieldIconUI.transform.Find("ShIconOn").GetComponent<Image>().enabled = false;

            attackIconUI.transform.Find("AtkIconOff").GetComponent<Image>().enabled = true;
            attackIconUI.transform.Find("AtkIconOn").GetComponent<Image>().enabled = false;
        }
        modifierSelected = 0;
    }
    void UISelectShield(int playerBehvaiorID)
    {
        if (ID == playerBehvaiorID)
        {
            modifierSelected = 1;

            //Sets the shield icon to active and the rest to inactive
            speedIconUI.transform.Find("SpdIconOff").GetComponent<Image>().enabled = true;
            speedIconUI.transform.Find("SpdIconOn").GetComponent<Image>().enabled = false;

            shieldIconUI.transform.Find("ShIconOff").GetComponent<Image>().enabled = false;
            shieldIconUI.transform.Find("ShIconOn").GetComponent<Image>().enabled = true;

            attackIconUI.transform.Find("AtkIconOff").GetComponent<Image>().enabled = true;
            attackIconUI.transform.Find("AtkIconOn").GetComponent<Image>().enabled = false;
        }
    }
    void UISelectAttack(int playerBehvaiorID)
    {
        if (ID == playerBehvaiorID)
        {
            modifierSelected = 2;

            //Sets the attack icon to active and the rest to inactive
            speedIconUI.transform.Find("SpdIconOff").GetComponent<Image>().enabled = true;
            speedIconUI.transform.Find("SpdIconOn").GetComponent<Image>().enabled = false;

            shieldIconUI.transform.Find("ShIconOff").GetComponent<Image>().enabled = true;
            shieldIconUI.transform.Find("ShIconOn").GetComponent<Image>().enabled = false;

            attackIconUI.transform.Find("AtkIconOff").GetComponent<Image>().enabled = false;
            attackIconUI.transform.Find("AtkIconOn").GetComponent<Image>().enabled = true;
        }
    }
    void UISelectRight (int playerBehvaiorID)
    {
        if(ID == playerBehvaiorID)
            ChangeModifier(1);
        //soundController.PlayPowerUp();
        
    }
    void UISelectLeft(int playerBehvaiorID)
    {
        if (ID == playerBehvaiorID)
            ChangeModifier(-1);
        //soundController.PlayPowerDown();
    }



    void OnDestroy()
    {
        /*
        playerBehavior.EnergyUpdated -= Handle_EnergyUpdated;
        playerBehavior.SpeedUpdated -= HandleSpeedUpdated;
        playerBehavior.SelectAttack -= UISelectAttack;
        playerBehavior.SelectShield -= UISelectShield;
        playerBehavior.SelectSpeed -= UISelectSpeed;
        playerBehavior.SelectLeft -= UISelectLeft;
        playerBehavior.SelectRight -= UISelectRight;
        */
    }
}
