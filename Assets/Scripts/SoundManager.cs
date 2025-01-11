using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public bool isSoundOn = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsSoundOn => isSoundOn;

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
    }

    public void SetButtonSound(AudioSource buttonAudioSource)
    {
        buttonAudioSource.mute = !isSoundOn; // Mute the AudioSource when sound is off
    }

}