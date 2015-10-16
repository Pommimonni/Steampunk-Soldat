using UnityEngine;
using System.Collections;

public class AudioProgression : MonoBehaviour {

    public AudioSource source;
    public AudioClip menuLoop;
    public AudioClip buildUp;
    public AudioClip gameLoop;
    public AudioClip ambience;

    public float delayPrediction = 0.1f;

    float clipStartTime;

    void OnLevelWasLoaded(int level)
    {
        Debug.Log("loaded level " + level);
        if(level == 0)
        {
            CancelInvoke();
            source.clip = menuLoop;
            PlayClip();
        }
        if (level == 1 || level == 2)
        {
            CancelInvoke();
            Invoke("StartBuildUp", TimeUntilClipEnd());
        }
    }


    float TimeUntilClipEnd() //how long until the current loop is in the end part again
    {
        float timePassed = Time.time - clipStartTime;
        float curClipLength = source.clip.length;
        float timesLooped = timePassed / curClipLength;
        timesLooped = Mathf.Floor(timesLooped);
        float currentLoopTime = timePassed - (timesLooped * curClipLength);
        float timeUntilEnd = curClipLength - currentLoopTime;
        return timeUntilEnd - delayPrediction;
    }

    void StartBuildUp()
    {
        float buildUpLength = buildUp.length;
        source.clip = buildUp;
        PlayClip();
        CancelInvoke();
        Invoke("PlayMainLoop", buildUpLength);
    }

    void PlayMainLoop()
    {
        source.clip = gameLoop;
        PlayClip();
    }

    void PlayClip()
    {
        source.Play();
        clipStartTime = Time.time;
    }


        // Use this for initialization
    void Start () {
        Debug.Log("Audio prog start, Should only be called in splash screen");
        gameObject.SendMessage("OnLevelWasLoaded", Application.loadedLevel); //so it gets called in splash screen also in the beginning
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
