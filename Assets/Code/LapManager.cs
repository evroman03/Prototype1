using UnityEngine;
using UnityEngine.SceneManagement;

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

    //Declares variables
    [SerializeField]
    int totalLaps = 3;

    private int currentLapsPlayer1;
    private int currentLapsPlayer2;

    private int lastTriggerHitPlayer1;
    private int lastTriggerHitPlayer2;

    // Start is called before the first frame update
    void Start()
    {
        //Initializes variables
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
                lastTriggerHitPlayer1 = 0;
            }

            //If current laps equal total laps, end the game
            if (currentLapsPlayer1 == totalLaps)
            {
                print("PLAYER 1 WINS");
                SceneManager.LoadScene("");
            }
        }

        //If the car with the Player2 tag collides
        if (other.gameObject.tag == "Player2")
        {
            //If the third trigger was the last one touched, add one lap
            if (lastTriggerHitPlayer2 == 3)
            {
                currentLapsPlayer2++;
                lastTriggerHitPlayer2 = 0;
            }
            
            //If the current laps equal the total laps, end the game
            if (currentLapsPlayer2 == totalLaps)
            {
                print("PLAYER 2 WINS");
                SceneManager.LoadScene("");
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
                lastTriggerHitPlayer1 = 1;
            
            //If the second trigger is touched
            if (triggerID == 2 && lastTriggerHitPlayer1 == 1)
                lastTriggerHitPlayer1 = 2;
            
            //If the third trigger is touched
            if (triggerID == 3 && lastTriggerHitPlayer1 == 2)
                lastTriggerHitPlayer1 = 3;
            
        
        //If the player2 ID contacts the lap trigger
        } else if (playerID == 2)
        {
            //If the first trigger is touched
            if (triggerID == 1 && lastTriggerHitPlayer2 == 0) 
                lastTriggerHitPlayer2 = 1;
            
            //If the second trigger is touched
            if (triggerID == 2 && lastTriggerHitPlayer2 == 1)
                lastTriggerHitPlayer2 = 2;
            
            //If the third trigger is touched
            if (triggerID == 3 && lastTriggerHitPlayer2 == 2)
                lastTriggerHitPlayer2 = 3;
        }
    }
}