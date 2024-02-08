using UnityEngine;
using UnityEngine.InputSystem;

public class LapManager : MonoBehaviour
{

    //Makes Class a Singleton Class.
    #region Singleton
    private static LapManager instance;
    public static LapManager Instance
    {
        get
        {
            if (Instance == null)
                instance = FindAnyObjectByType(typeof(GameManager)) as LapManager;
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion

    [SerializeField]
    int totalLaps = 3;

    private int currentLapsPlayer1;
    private int currentLapsPlayer2;

    private int lastTriggerHitPlayer1;
    private int lastTriggerHitPlayer2;


    private bool player1Won;
    // Start is called before the first frame update

    //!TODO
    //Set start positions of the cars
    //Add laps
    //Add triggers
    //Add Countdown at start
    //Make game playable
    //Make everything start when countdown ends
    void Start()
    {
        player1Won = false;
        currentLapsPlayer1 = 0;
        currentLapsPlayer2 = 0;

        lastTriggerHitPlayer1 = 0;
        lastTriggerHitPlayer2 = 0;
    }

    /**
     * Detects when the Lap trigger is collided with
     * @Param collision - the object that collided with the lap trigger
     */
    private void OnTriggerEnter(Collider other)
    {
        //If the car with the Player1 tag collides
        if (other.gameObject.tag == "Player1")
        {
            //If the third trigger was the last one touched, add one lap
            if (lastTriggerHitPlayer1 == 3)
            {
                currentLapsPlayer1++;
                print("P1 LAP PASSED! Current laps:" + currentLapsPlayer1);
                lastTriggerHitPlayer1 = 0;
            }

            //If current laps equal total laps, end the game
            if (currentLapsPlayer1 == totalLaps)
            {
                player1Won = true;
                print("PLAYER 1 WINS");
            }
        }

        //If the car with the Player2 tag collides
        if (other.gameObject.tag == "Player2")
        {
            //If the third trigger was the last one touched, add one lap
            if (lastTriggerHitPlayer2 == 3)
            {
                currentLapsPlayer2++;
                print("P2 LAP PASSED! Current laps:" + currentLapsPlayer2);
                lastTriggerHitPlayer2 = 0;
            }
            
            //If the current laps equal the total laps, end the game
            if (currentLapsPlayer2 == totalLaps)
            {
                player1Won = false;
                print("PLAYER 2 WINS");
            }
        }
    }

    /**
     * Changes the trigger counter for each player
     */
    public void TriggerHit(int triggerID, int playerID)
    {
        //If the player1 ID contacts the lap trigger
        if (playerID == 1)
        {
            //If the first trigger is touched
            if (triggerID == 1 && lastTriggerHitPlayer1 == 0)
            {
                print("P1 Triggered 1");
                lastTriggerHitPlayer1 = 1;
            }
            //If the second trigger is touched
            if (triggerID == 2 && lastTriggerHitPlayer1 == 1)
            {
                print("P1 Triggered 2");
                lastTriggerHitPlayer1 = 2;
            }
            //If the third trigger is touched
            if (triggerID == 3 && lastTriggerHitPlayer1 == 2)
            {
                print("P1 Triggered 3");
                lastTriggerHitPlayer1 = 3;
            }
        
        //If the player2 ID contacts the lap trigger
        } else if (playerID == 2)
        {
            //If the first trigger is touched
            if (triggerID == 1 && lastTriggerHitPlayer2 == 0)
                print("P2 Triggered 1");
            {
                lastTriggerHitPlayer2 = 1;
            }
            //If the second trigger is touched
            if (triggerID == 2 && lastTriggerHitPlayer2 == 1)
            {
                print("P2 Triggered 2");
                lastTriggerHitPlayer2 = 2;
            }
            //If the third trigger is touched
            if (triggerID == 3 && lastTriggerHitPlayer2 == 2)
            {
                print("P2 Triggered 3");
                lastTriggerHitPlayer2 = 3;
            }
        }
    }

    private void Update()
    {
        //print(lastTriggerHitPlayer2);
    }

    public bool getPlayer1Won()
    {
        return player1Won;
    }
}