using UnityEngine;

public class ButtonSounds : MonoBehaviour
{
    public AudioClip pressedSound;
    public AudioClip rolloverSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPressedSound()
    {
        audioSource.clip = pressedSound;
        audioSource.Play();
    }

    public void PlayRolloverSound()
    {
        audioSource.clip = rolloverSound;
        audioSource.Play();
    }
}
