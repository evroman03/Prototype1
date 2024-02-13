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

    [SerializeField] private GameObject audioLocation;
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
        AudioSource.PlayClipAtPoint(StartCountdown, audioLocation.transform.position);
    }

    public void LapComplete()
    {
        AudioSource.PlayClipAtPoint(LapFinish, audioLocation.transform.position);
    }

    public void PlayPowerUp()
    {
        AudioSource.PlayClipAtPoint(PowerUp, audioLocation.transform.position);
    }

    public void PlayPowerDown()
    {
        AudioSource.PlayClipAtPoint(PowerDown, audioLocation.transform.position);
    }

    public void PlayShoot()
    {
        AudioSource.PlayClipAtPoint(Shoot, audioLocation.transform.position);
    }
}
