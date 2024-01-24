using UnityEngine;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{
    public GameObject SpeedBar;
    public GameObject AttackBar;
    public GameObject ShieldBar;
    private int speedValue = 1;
    private int attackValue = 1;
    private int shieldValue = 1;


    [SerializeField] private int maxValue = 3;
    [SerializeField] private Color energyColor;
    [SerializeField] private Color speedColor;
    [SerializeField] private Color attackColor;
    [SerializeField] private Color shieldColor;

    float energyheard; 
    // Start is called before the first frame update
    void Start()
    {
        PlayerBehavior.EnergyUpdated += Handle_EnergyUpdated;
    }

    // Update is called once per frame
    void UpdateUI()
    {
        //Updates Energy

        ///TODO

        //Updates Speed UI
        for (int i = 0; i < speedValue; i++)
        {
            SpeedBar.transform.GetChild(i).GetComponent<Image>().color = speedColor;
        }
        for (int i = speedValue; i < maxValue; i++)
        {
            SpeedBar.transform.GetChild(i).GetComponent<Image>().color = Color.gray;
        }

        //Updates Attack UI
        for (int i = 0; i < attackValue; i++)
        {
            AttackBar.transform.GetChild(i).GetComponent<Image>().color = attackColor;
        }
        for (int i = attackValue; i < maxValue; i++)
        {
            AttackBar.transform.GetChild(i).GetComponent<Image>().color = Color.gray;
        }

        //Updates Shield UI
        for (int i = 0; i < shieldValue; i++)
        {
            ShieldBar.transform.GetChild(i).GetComponent<Image>().color = shieldColor;
        }
        for (int i = shieldValue; i < maxValue; i++)
        {
            ShieldBar.transform.GetChild(i).GetComponent<Image>().color = Color.gray;
        }
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

    

    /**
     * Changes values in the UI
     * @param UIType The UI type to change - "speed", "attack", and "shield" are accepted
     * @param If the value should be increased or decreased
     */
    public void ToggleUI(string UIType, bool isIncreasing)
    {
        //Checks for what UI component is being changed
        switch (UIType) {
            case "speed":
                //Checks if the speed should be increased
                if (isIncreasing) {
                    if (speedValue < maxValue)
                        speedValue++;
                    //Decreases the speed
                } else {
                    if (speedValue > 0)
                        speedValue--;
                }
                break;
            case "attack":
                //Checks if the attack should be increased
                if (isIncreasing)
                {
                    if (attackValue < maxValue)
                        attackValue++;
                }
                //Decreases the attack
                else
                {
                    if (attackValue > 0)
                        attackValue--;
                }
                break;
            case "shield":
                //Checks if the shield should be increased
                if (isIncreasing)
                {
                    if (shieldValue < maxValue)
                        shieldValue++;
                }
                //Decreases the shield
                else
                {
                    if (shieldValue > 0)
                        shieldValue--;
                }
                break;
            //If no valid input is entered
            default:
                print("ERROR: Invalid UI type entered! Failed to change UI value");
                break;
        }
        UpdateUI();
    }
}
