using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    [SerializeField]
    private int triggerID;
    [SerializeField]
    private LapManager lapManager;

    /**
     * Detects when a trigger is hit then reports back to LapManager
     */
    private void OnTriggerEnter(Collider other)
    {
        //If Player1 collided with the trigger
        if (other.gameObject.tag == "Player1")
            lapManager.TriggerHit(triggerID, 1);
        //If Player2 collided with the trigger
        if (other.gameObject.tag == "Player2")
            lapManager.TriggerHit(triggerID, 2);
    }
}
