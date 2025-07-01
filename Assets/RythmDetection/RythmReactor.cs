using UnityEngine;

public class RythmReactor : MonoBehaviour
{

    private SongManager songManager;
    

  public GameObject[] meshes;
  [SerializeField] private float sizeFactor = 10;

  [SerializeField] private float minSize = 0;
  [SerializeField] private float maxSize = 500;

  [SerializeField] private float randomness = 1.3f;

  private SongManager.PointData currentDataPoint;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        songManager = FindAnyObjectByType<SongManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!songManager.songPaused)
        {
            currentDataPoint = songManager.songData.currentPoint;

            var pointAmplitude = currentDataPoint.pointPosition.y;

            pointAmplitude *= sizeFactor;

            pointAmplitude = Mathf.Clamp(pointAmplitude, minSize, maxSize);

            foreach (var mesh in meshes)
            {
                var randomOffset = Random.Range(0.7f, randomness);
                LeanTween.scale(mesh, new Vector3(pointAmplitude * randomOffset, pointAmplitude * randomOffset, pointAmplitude * randomOffset), 0.2f);
            }
        }
        
    }
}
