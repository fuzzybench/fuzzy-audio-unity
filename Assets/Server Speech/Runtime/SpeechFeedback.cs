using UnityEngine;

public class SpeechFeedback : MonoBehaviour
{
    private SpeechLogic _speechLogic;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        _speechLogic = GetComponent<SpeechLogic>();
    }

    
    public void StartSTTPlayback()
    {
        audioSource.PlayOneShot(_speechLogic.sttAudioClip);
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
        audioSource.PlayOneShot(_speechLogic.ttsAudioClip);
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
