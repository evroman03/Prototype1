using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    //When single player button is clicked, fire method
    public void SinglePlayerClicked()
    {
        SceneManager.LoadScene(1);
    }

    //When two players button is clicked, fire method
    public void TwoPlayersClicked()
    {
        SceneManager.LoadScene(2);
    }
}