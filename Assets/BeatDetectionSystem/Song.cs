using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class Song : MonoBehaviour
{
    #region  -- Helper Functions ---

    private class PointData
    {
        public int idx = -1;
        public float time = -1;
        public int startSample = -1;
        public int endSample = -1;

        //holds the averaged out sample value
        public float avgSampleValue = 0;
        
        //holds the position of where the point should be in 3D space
        public Vector3 pointPosition;
        //holds the position of where the point should be in 3D space, but without the Y axis shift
        public Vector3 pointPositionStrait;
    }


    private class SongData
    {
        public float[] samples;
        public List<PointData> points;
        public PointData currentPoint;
        
    }
    private struct childObjects
    {
        public GameObject bottom;
        public GameObject start;
        public LineRenderer line;
        public GameObject cursor;
        public Camera cam;
        
    }
    #endregion


    //How many samples to average across per point
    public int samplesPerPoint = 1024;

    //extraplotates the sample size (which is by default, -1 to 1 )
    public float meterScale = 100;

    [SerializeField] private float cameraZoomFactor = 1;

    private AudioSource source = null;

    private SongData songData;

    private childObjects children;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        songData = new SongData();
        source = GetComponent<AudioSource>();
       
        source.Play();
        
        //get the children required for the debug visualiser
        children.bottom = ReturnChild("bottom", transform);
        children.start = ReturnChild("start", transform);
        children.line = ReturnChild("line", children.start.transform).GetComponent<LineRenderer>();
        children.cursor = ReturnChild("cursor", children.start.transform);
        children.cam = ReturnChild("main camera", children.cursor.transform).GetComponent<Camera>();
        
        
        GenerateDataPointList();
        DrawSongGraph();
    }

    private void Update()
    {
        float timeSample = source.timeSamples * source.clip.channels;
        songData.currentPoint = songData.points.Single(x => x.startSample <= timeSample && x.endSample > timeSample);
        children.cursor.transform.localPosition = songData.currentPoint.pointPosition;

    }

    private void GenerateDataPointList()
    {
        //loads the samples from the clips
        songData.samples = new float[source.clip.samples * source.clip.channels];
        source.clip.GetData(songData.samples, 0);

        songData.points = new List<PointData>();
        songData.points.Capacity = songData.samples.Length / samplesPerPoint + 1;


       
        int idCount = 0;
        float sum;
        for (int sample = 0; sample < songData.samples.Length; sample += samplesPerPoint)
        {
           
            PointData tempData = new PointData();
            tempData.idx = idCount;
            idCount += 1;

            
            //gets both the end and start sample of the range to average between
            tempData.startSample = tempData.idx * samplesPerPoint;
            
            //we substract one to make sure there is no overlap between the data point's start and end
            tempData.endSample = ((tempData.idx + 1) * samplesPerPoint) - 1;

            //we divide by the number of channels, to make sure we get an accurate result depending on if the track is mono or stereo, when we divide it by the frequency it gives the time of the point in seconds.
            tempData.time = (tempData.startSample / (float)source.clip.channels) / (float)source.clip.frequency;
            
            
            //Checks how many channels the track has and adds to the sum accordingly. If it is mono then we count the samples and increment by 1, if it is stereo, then we get the current sample and the sample after it, along with incrementing by 2, to not count the same sample twice.
            sum = 0;
            if (source.clip.channels == 1)
            {
                for (int i = tempData.startSample; i <= tempData.endSample; i++)
                {
                    //a check to make sure we don't go pass the end of the song
                    if (i > songData.samples.Length - 1)
                    {
                        break;
                    }
                    sum += songData.samples[i];
                }
            }
            else
            {
                for (int i = tempData.startSample; i <= tempData.endSample; i += source.clip.channels)
                {
                    //a check to make sure we don't go pass the end of the song
                    if (i > songData.samples.Length - 1)
                    {
                        break;
                    }
                    sum += (songData.samples[i] + songData.samples[i + 1]) * 0.5f;
                    
                  
                }
            }

            //meterScale is to make sure the sample value is visible
            tempData.avgSampleValue = (sum / samplesPerPoint) * meterScale;
            print(tempData.avgSampleValue);
            
            //idx gives the position along the song, and sample value gives the height
            tempData.pointPosition = new Vector3(tempData.idx, tempData.avgSampleValue, 0);
            tempData.pointPositionStrait = new Vector3(tempData.idx, 0, 0);
           
            songData.points.Add(tempData);

        }
    }

    private GameObject ReturnChild(string name, Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (string.Equals(child.name, name, StringComparison.CurrentCultureIgnoreCase))
            {
                return child.gameObject;
            }
        }

        return null;
    }


    void DrawSongGraph()
    {
        children.line.positionCount = songData.points.Count;
        
        //uses lambda expressions to return just an array of the point positions
        children.line.SetPositions(songData.points.Select(x => x.pointPosition).ToArray());

        children.bottom.transform.localScale = new Vector3(songData.points.Count,
            children.bottom.transform.localScale.y, children.bottom.transform.localScale.z);

        children.start.transform.localPosition = new Vector3(children.bottom.transform.localScale.x * -0.5f,
            children.bottom.transform.localScale.y, children.bottom.transform.localScale.z);
    }
    

    
}
