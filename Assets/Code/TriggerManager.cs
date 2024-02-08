using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{

    //Makes Class a Singleton Class.
    #region Singleton
    private static TriggerManager instance;
    public static TriggerManager Instance
    {
        get
        {
            if (Instance == null)
                instance = FindAnyObjectByType(typeof(GameManager)) as TriggerManager;
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion

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
