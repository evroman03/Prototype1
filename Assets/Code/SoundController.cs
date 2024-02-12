using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioClip PowerUp;
    [SerializeField] private AudioClip PowerDown;
    [SerializeField] private AudioClip StartCountdown;
    [SerializeField] private AudioClip Shoot;
    [SerializeField] private AudioClip LapFinish;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Countdown()
    {
        AudioSource.PlayClipAtPoint(StartCountdown, Camera.main.transform.position);
    }

    public void LapComplete(GameObject Player)
    {
        AudioSource.PlayClipAtPoint(LapFinish, Player.transform.position);
    }

    public void PlayPowerUp(GameObject Player)
    {
        AudioSource.PlayClipAtPoint(PowerUp, Player.transform.position);
    }

    public void PlayPowerDown(GameObject Player)
    {
        AudioSource.PlayClipAtPoint(PowerDown, Player.transform.position);
    }

    public void PlayShoot(GameObject Player)
    {
        AudioSource.PlayClipAtPoint(Shoot, Player.transform.position);
    }
}
