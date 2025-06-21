using UnityEngine;

public class BatCapsule : MonoBehaviour
{
    [SerializeField] private BatCapsuleFollower batCapsuleFollower;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnBatCapsuleFollower();
    }


  private void SpawnBatCapsuleFollower()
  {
      var follower = Instantiate(batCapsuleFollower);
      follower.transform.parent = null;
      follower.transform.position = transform.position;
      follower.SetFollowTarget(this);
  }
}
