using UnityEngine;
using System.Collections;

public class MultiPhaseAudio : MonoBehaviour {

    public AudioClip[] audioClips;
    public AudioSource audioSource;

    int lastPhase = 0;
    int phaseAmount = 0;

    // Use this for initialization
    void Start () {
        phaseAmount = audioClips.Length;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Play(int phase)
    {
        if(phase > phaseAmount || phase < 1)
        {
            Debug.LogError("Phase " + phase + " doesnt exist! Only "+phaseAmount+" phases added in the script");
            return;
        }
        lastPhase = phase;
        int phaseIndex = phase - 1;
        audioSource.clip = audioClips[phaseIndex];
        audioSource.Play();
        if(audioClips.Length >= (phaseIndex+2))
        {
            audioClips[phaseIndex + 1].LoadAudioData();
        }
    }

    public void PlayOnce(int phase)
    {
        audioSource.loop = false;
        Play(phase);
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }

    public void PlayLooped(int phase)
    {
        audioSource.loop = true;
        Play(phase);
    }
    
    public void Play()
    {
        if (lastPhase < 0)
        {
            Debug.LogError("Calling a negative phase! dont do this!");
            return;
        }
        int nextPhase = lastPhase + 1;
        if(nextPhase > phaseAmount)
        {
            nextPhase = 1;
        }
        Play(nextPhase);
    }

    public void ResetToStart()
    {
        audioSource.Stop();
        lastPhase = 0;
    }

    void StopCurrentPhase()
    {
        audioSource.Stop();
    }

    void StartNextPhase()
    {
        Play();
    }
}
