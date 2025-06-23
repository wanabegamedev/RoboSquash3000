using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Text;
using NUnit.Framework;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class Song : MonoBehaviour
{
    #region  -- Helper Functions ---

    public class PointData
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

        public FlagData flags;
        
        public GameObject marker;

        public BeatData calculatedBeat;

        public string StringConvert(bool header)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (header)
            {
                stringBuilder.AppendFormat("Idx".PadRight(20));
                stringBuilder.AppendFormat("Time".PadRight(20));
                stringBuilder.AppendFormat("Start Sample".PadRight(20));
                stringBuilder.AppendFormat("End Sample".PadRight(20));
                stringBuilder.AppendFormat("delta".PadRight(20));
                stringBuilder.AppendFormat("rawBPM".PadRight(20));
                stringBuilder.AppendFormat("BPM".PadRight(20));

            }
            else
            {
                stringBuilder.AppendFormat(idx.ToString().PadRight(20));
                stringBuilder.AppendFormat(time.ToString(CultureInfo.InvariantCulture).PadRight(20));
                stringBuilder.AppendFormat(startSample.ToString().PadRight(20));
                stringBuilder.AppendFormat(endSample.ToString().PadRight(20));

                if (!flags.isBeat) return stringBuilder.ToString();
                
                stringBuilder.AppendFormat(calculatedBeat.delta.ToString(CultureInfo.InvariantCulture).PadRight(20));
                stringBuilder.AppendFormat(calculatedBeat.rawBPM.ToString(CultureInfo.InvariantCulture).PadRight(20));
                stringBuilder.AppendFormat(calculatedBeat.bpm.ToString(CultureInfo.InvariantCulture).PadRight(20));
            }
            
            
            return stringBuilder.ToString();
            
        }
    } 


    public class SongData
    {
        public float[] samples;
        public List<PointData> points;
        public PointData currentPoint;
        public float bpm;

    }
    private struct ChildObjects
    {
        public GameObject bottom;
        public GameObject start;
        public LineRenderer line;
        public LineRenderer simpleLine;
        public GameObject cursor;
        public Camera cam;
        public GameObject quadHolder;
        
    }

    public struct FlagData
    {
        public bool simplePoint;
        public bool isBeat;
    }

    public struct BeatData
    {
        //this is the time between two beats
        public float delta;
        
        //the raw floating point bpm
        public float rawBPM;
        
        //the real usable bpm
        public float bpm;
    }
    
    
    #endregion


    //How many samples to average across per point
    public int samplesPerPoint = 1024;

    //extraplotates the sample size (which is by default, -1 to 1 )
    public float meterScale = 100;
    
    public float tolerance = 20;

    [SerializeField] private float cameraZoomFactor = 1;

    private AudioSource source = null;

    private SongData songData;

    private ChildObjects children;

    private bool songPaused = false;

    //controls the high end 
    public float quantizeValueHigh = 160;
    //controls the low end
    public float quantizeValueLow = 100;

    
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
        children.simpleLine = ReturnChild("simpleline", children.start.transform).GetComponent<LineRenderer>();
        children.quadHolder = ReturnChild("quadholder", children.start.transform);
        
        
        GenerateDataPointList();
        DrawSongGraph();
        
        GenerateSimpleLine();
        
        BeatFind();
        
        DebugDataWrite();
        
      
     
    }

    private void Update()
    {
        float timeSample = source.timeSamples * source.clip.channels;
        songData.currentPoint = songData.points.Single(x => x.startSample <= timeSample && x.endSample > timeSample);
        children.cursor.transform.localPosition = songData.currentPoint.pointPosition;

        DebugInputs();
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

            //meterScale is to make sure the sample value is visible, but also clamped above 1
            tempData.avgSampleValue = Mathf.Abs((sum / samplesPerPoint) * meterScale);
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


    void DebugInputs()
    {
        children.cam.orthographicSize += Input.mouseScrollDelta.y * cameraZoomFactor;
        children.cam.orthographicSize = Math.Clamp(children.cam.orthographicSize, 0, 400);

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!songPaused)
            {
                songPaused = true;
                source.Pause();
            }
            else
            {
                songPaused = false;
                source.UnPause();
            }
        }
    }


    void DebugDataWrite()
    {
        string path = "Assets/Debug.txt";

        StreamWriter sw = new StreamWriter(path, false);
        
        sw.WriteLine("Song:" );
        sw.WriteLine($"\tsamples={source.clip.samples}");
        sw.WriteLine($"\tchannels={source.clip.channels}");
        sw.WriteLine($"\tBPM={songData.bpm}");
        
        sw.WriteLine($"\tpoints={songData.points.Count}");
      


        
        //write beat points data instead of full dats

        sw.WriteLine(songData.points[0].StringConvert(true));
        foreach (var beat in songData.points.Where(x => x.flags.isBeat == true).ToList())
        {
            sw.WriteLine(beat.StringConvert(false));
        }
        
        
        
        
        // //write header
        // sw.WriteLine(songData.points[0].StringConvert(true));
        //   
        // foreach (var point in songData.points)
        // {
        //     sw.WriteLine(point.StringConvert(false));
        // }
        //
        
        sw.Close();
        UnityEditor.AssetDatabase.ImportAsset(path);
        
        
    }


    void GenerateSimpleLine()
    {
        List<int> pointsToKeep = new List<int>();
        //this returns the indexes of the points to keep on a simplified line
        LineUtility.Simplify(songData.points.Select(x => x.pointPosition).ToList(), tolerance, pointsToKeep);

        foreach (var id in pointsToKeep)
        {
            songData.points[id].flags.simplePoint = true;
        }

        children.simpleLine.positionCount = pointsToKeep.Count;
        //This lambda expression selects all the points that are part of the simple line and then gets their positions
        children.simpleLine.SetPositions(songData.points.Where(x => x.flags.simplePoint == true).Select(X => X.pointPosition).ToArray());

    }


    void BeatFind()
    {
        List<PointData> simplePoints = songData.points.Where(x => x.flags.simplePoint == true).ToList();

        for (int i = 0; i < simplePoints.Count; i++)
        {
            //sets up the first beat position
            if (i == 0)
            {
                if (simplePoints[i].avgSampleValue > simplePoints[i + 1].avgSampleValue)
                {
                    simplePoints[i].flags.isBeat = true;
                }
            }
            //checks to see if last position is a beat
            else if (i == simplePoints.Count - 1)
            {
                if (simplePoints[i].avgSampleValue > simplePoints[i - 1].avgSampleValue)
                {
                    simplePoints[i].flags.isBeat = true;
                }
            }
            else
            {
                //calculates if all middle points are beats
                if (simplePoints[i - 1].avgSampleValue < simplePoints[i].avgSampleValue && simplePoints[i].avgSampleValue > simplePoints[i + 1].avgSampleValue )
                {
                    simplePoints[i].flags.isBeat = true;
                }
            }
        }

        //once all the beats have been found, instantiate markers there
        List<PointData> beats = simplePoints.Where(x => x.flags.isBeat == true).ToList();
        
        foreach (var beat in beats)
        {
            if (beat.marker == null)
            {
                beat.marker = CreateDebugBeatVisualiser(beat.pointPositionStrait, Color.black, "beat " + beat.idx);
            }
        }
        
        CalculateBPM(beats);
    }


    private void CalculateBPM(List<PointData> beatList)
    {
        for (int i = 1; i < beatList.Count; i++)
        {
        
            //calculates the bpm between the current and last beats 
            
            //calculates the time between the last beat
            beatList[i].calculatedBeat.delta = beatList[i].time - beatList[i - 1].time;
            beatList[i].calculatedBeat.rawBPM = 60f / beatList[i].calculatedBeat.delta;
            beatList[i].calculatedBeat.bpm = QuantizeBPM(beatList[i].calculatedBeat.rawBPM);
            print(beatList[i].calculatedBeat.rawBPM);
            
        }
        
        //set the final BPM of the average BPM across the whole track
        print( "average: " + beatList.Average(x => x.calculatedBeat.bpm));
        songData.bpm = beatList.Average(x => x.calculatedBeat.bpm);
    }
    
    GameObject CreateDebugBeatVisualiser(Vector3 position, Color col, string quadName)
    {
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);

        quad.transform.parent = children.quadHolder.transform;

        quad.transform.localPosition = position;
        quad.transform.localScale = new Vector3(1, 50, 1);
        Material mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = col;

        quad.GetComponent<Renderer>().material = mat;

        quad.name = quadName;
        
        return  quad;


    }

    private float QuantizeBPM(float bpm)
    {
        
        if (bpm > quantizeValueHigh)
        {
            while (bpm > quantizeValueHigh)
            {
                
                bpm *= 0.5f;
            }
            
            print("RAISED");

      
        }
        else if (bpm < quantizeValueLow)
        {
            while (bpm < quantizeValueLow)
            {
                bpm *= 2f;
            }
            print("LOWERED");

           
        }

        return bpm;
    }
    
    

    
}
