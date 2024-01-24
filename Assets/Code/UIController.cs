using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
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
}
