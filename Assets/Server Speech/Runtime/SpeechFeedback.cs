using UnityEngine;

public class SpeechFeedback : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip sttAudioClip;
    private AudioClip ttsAudioClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    
    public void StartSTTPlayback()
    {
        audioSource.PlayOneShot(sttAudioClip);
    }

    public void StopSTTPlayback()
    {
        audioSource.Stop();
    }

    public void PauseSTTPlayback()
    {
        audioSource.Pause();
    }
    
    public void StartTTSPlayback()
    {
        audioSource.PlayOneShot(ttsAudioClip);
    }

    public void StopTTSPlayback()
    {
        audioSource.Stop();
    }

    public void PauseTTSPlayback()
    {
        audioSource.Pause();
    }
}
