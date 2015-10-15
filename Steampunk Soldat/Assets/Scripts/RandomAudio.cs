using UnityEngine;

public class RandomAudio : MonoBehaviour {

    public AudioClip[] audioClips;
    public AudioSource audioSource;

    AudioClip chooseRandomClip()
    {
        int options = audioClips.Length;
        int rand = Random.Range(0, options);
        return audioClips[rand];
    }

    public void PlayRandomClip()
    {
        audioSource.clip = chooseRandomClip();
        audioSource.Play();
    }

    public void Play()
    {
        PlayRandomClip();
    }
}
