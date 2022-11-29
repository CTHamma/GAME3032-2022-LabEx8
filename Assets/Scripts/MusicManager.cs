// Singleton MusicManager class to handle playing and crossfading music, which presists bwtween scene transition

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public enum TrackID
    {
        TOWN,
        WILD
    }

    private MusicManager() { }

    private static MusicManager instance = null;
    public static MusicManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new MusicManager();
            }
            return instance;
        }

        private set { instance = value;  }
    }

    [Tooltip("One track for crossfading. The ordr is arbritrary")]
    [SerializeField]
    AudioSource musicSource1;

    [Tooltip("Another track for crossfading. The ordr is arbritrary")]
    [SerializeField]
    AudioSource musicSource2;


    AudioClip[] tracks;

    // Start is called before the first frame update
    void Start()
    {
        //On start, if instance is null, this will set our original. If it's set this will return that one
        MusicManager original = Instance;

        //If I want to have MusicMangers living in the scne, I need to make sure only one stays the same
        MusicManager[] managers = GameObject.FindObjectsOfType<MusicManager>();
        foreach(MusicManager manager in managers)
        {
            //
            if(manager != original)
            {
                Destroy(manager.gameObject);
            }
        }

        if(this == original)
        {
            DontDestroyOnLoad(original);
        }
    }

    /*Add a method for:
     * 1. Playing a track immediately
     * 2. Crossfading between current track and a new Goal track
     * 3. Fading out a track
     * 4. Fading in a track on top of existing one
     * 5. Dip-To-Black transition where current track fades to 0, then Goal track fades from 0
     */

    /// <summary>
    /// Stop everything and play on source 1
    /// </summary>
    /// <param name="whichTrackToPlay"></param>
    public void PlayTrackSolo(TrackID whichTrackToPlay)
    {
        musicSource1.Stop();
        musicSource2.Stop();
        musicSource1.clip = tracks[(int)whichTrackToPlay];
        musicSource1.Play();
    }

    /// <summary>
    /// Assuming one track is already playing, we crossfade to end with another track playing solo on a different source
    /// </summary>
    /// <param name="goalTrack"></param>
    public void CrossFadeTo(TrackID goalTrack, float transitationDurationSec = 3.0f)
    {
        AudioSource oldTrack = null;
        AudioSource newTrack = null;

        if (musicSource1.isPlaying)
        {
            oldTrack = musicSource1;
            newTrack = musicSource2;
        }
        else if (musicSource2.isPlaying)
        {
            oldTrack = musicSource2;
            newTrack = musicSource1;
        }

        newTrack.clip = tracks[(int)goalTrack];
        newTrack.Play();

        StartCoroutine(CrossFadeCoroutine(oldTrack, newTrack, transitationDurationSec));
    }

    private IEnumerator CrossFadeCoroutine(AudioSource oldTrack, AudioSource newTrack, float transitationDurationSec)
    {
        float time = 0.0f;
        while (time < transitationDurationSec)
        {
            float tValue = time / transitationDurationSec;

            // volume from 0 to 1 over duration
            newTrack.volume = tValue;
            oldTrack.volume = 1.0f - tValue;

            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        oldTrack.Stop();
        oldTrack.volume = 1.0f;
    }

    //void OnSceneLoaded(SceneChange newScene, LoadSceneMode loadMode)
    //{
    //    if(newScene.name = "Town")
    //    {
    //        CrossFadeTo(TrackID.Town)
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        
    }
}
