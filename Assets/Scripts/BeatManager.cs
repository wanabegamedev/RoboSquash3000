using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    public UnityEvent onBeat;

    [SerializeField] private SongManager songManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        StartCoroutine(SendBeat(songManager.songData.bpm / 60));
    }

    // Update is called once per frame

    IEnumerator SendBeat(float beatDelay)
    {
        while (!songManager.songPaused)
        {
            onBeat.Invoke();
            yield return new WaitForSeconds(beatDelay);
        }
        
        yield return new WaitForEndOfFrame();
    }
}
