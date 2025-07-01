using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pulse : MonoBehaviour
{
    [Range(0, 10)] public float minPulse = 1;
    [Range(1, 10)] public float maxPulse = 1;

    [Header("Pulse Options")] public bool pulseScale = false;
    public bool pulseMovementWave = false;
    public bool pulseRotation = false;
    public bool pulseColour = false;

    private BeatManager beatManager;


    public void PulseScale()
    {
        var randomValue = Random.Range(minPulse, maxPulse);
        LeanTween.scale(gameObject, new Vector3(randomValue, randomValue, randomValue), 0.1f);

    }

    public void PulseRotation()
    {
        var randomValue = Random.Range(minPulse, maxPulse);
        LeanTween.rotate(gameObject, new Vector3(randomValue, randomValue, randomValue), 0.1f);

    }


    private void Start()
    {
        beatManager = FindAnyObjectByType<BeatManager>();

    }

}
