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

    public void PlayPowerUp()
    {
        AudioSource.PlayClipAtPoint(PowerUp, Camera.main.transform.position);
    }

    public void PlayPowerDown()
    {
        AudioSource.PlayClipAtPoint(PowerDown, Camera.main.transform.position);
    }

    public void PlayShoot()
    {
        AudioSource.PlayClipAtPoint(Shoot, Camera.main.transform.position);
    }

    public void MuteAcceleration(AudioSource accelerationSource)
    {
        accelerationSource.volume = 0;
    }

    public void UnmuteAcceleration(AudioSource accelerationSource)
    {
        accelerationSource.volume = 1;
    }


}
