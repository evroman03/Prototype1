using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{

    //Makes Class a Singleton Class.
    #region Singleton
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (Instance== null)
                instance = FindAnyObjectByType(typeof(GameManager)) as GameManager;
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion

    private GameObject player1, player2;

    [SerializeField]
    private GameObject trackCamera;

    [SerializeField]
    private GameObject singlePlayerCanvas, multiplayerCanvas, trackCanvas;

    [SerializeField]
    private TextMeshProUGUI startText, waitText, countDownText;

    [SerializeField]
    private Vector3 player1StartingPos = new Vector3(-1644, 470, 532),
                player2StartingPos = new Vector3(-1644, 470, 544);

    // Start is called before the first frame update
    void Start()
    {
        trackCanvas.SetActive(true);
        startText.enabled = true;
        waitText.enabled = false;
        countDownText.enabled = false;
        
        player1 = null;
        player2 = null;

        singlePlayerCanvas.SetActive(false);
        multiplayerCanvas.SetActive(false);
    }
    public void onPlayerJoined(PlayerInput obj)
    {
        if (player1 == null)
        {
            singlePlayerCanvas.SetActive(true);
            multiplayerCanvas.SetActive(false);
            trackCanvas.SetActive(false);

            waitText.enabled = true;

            trackCamera.SetActive(false);
            player1 = obj.gameObject;
            player1.transform.position = player1StartingPos;
            player1.transform.rotation = Quaternion.identity;
            player1.transform.rotation *= Quaternion.Euler(0, 90, 0);
        }
        else if (player2 == null)
        {
            singlePlayerCanvas.SetActive(false);
            multiplayerCanvas.SetActive(true);
            trackCanvas.SetActive(false);

            waitText.enabled= false;

            trackCamera.SetActive(false);
            player2 = obj.gameObject;

            player1.transform.position = player1StartingPos;
            player1.transform.rotation = Quaternion.identity;
            player1.transform.rotation *= Quaternion.Euler(0, 90, 0);
            //player1.GetComponent<Rigidbody>().velocity = Vector3.zero;

            player2.transform.position = player2StartingPos;
            player2.transform.rotation = Quaternion.identity;
            player2.transform.rotation *= Quaternion.Euler(0, 90, 0);

            countDownText.enabled = true;
            StartCoroutine(CountDown());
        }
        else
        {
            print("ERROR: COULD NOT FIND PLAYER OR MAX NUMBER OF PLAYERS REACHED");
        }
    }

    IEnumerator CountDown()
    {
        //i = countdown in seconds (lower i = lower countdown time in game)
        for (int i = 3; i >= 0; i--)
        {
            if (i > 0)
                countDownText.text = "Starts in... " + i;
            else
                countDownText.text = "GO!";
            yield return new WaitForSeconds(1);
        }

        //Makes Text invisible
        countDownText.enabled = false;

        //Activates the controls for both players after the countdown
        player1.GetComponent<PlayerBehavior>().activateControls(true);
        player2.GetComponent<PlayerBehavior>().activateControls(true);

        StopCoroutine(CountDown());
        yield return null;
    }
}
