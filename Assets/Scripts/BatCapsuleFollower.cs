using System;
using UnityEngine;

public class BatCapsuleFollower : MonoBehaviour
{
    private BatCapsule capsuleToFollow;
    private Rigidbody rigid;

    [SerializeField] private float followerSensitivity;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }


    //Use fixed update as it is better for physics calculations
    private void FixedUpdate()
    {
        Vector3 destinationToMoveTo = capsuleToFollow.transform.position;

        transform.rotation = capsuleToFollow.transform.rotation;
        
        //make sure the rotation matches the bat capsule
        rigid.transform.rotation = transform.rotation;
        
        
        //figure out how far away the physics capsule is and add the appropriate force to the correct vector to make it line up with the bat
        var velocity = (destinationToMoveTo - rigid.transform.position) * followerSensitivity;

        rigid.linearVelocity = velocity;
     
    }

    public void SetFollowTarget(BatCapsule followTarget)
    {
        
        //makes the physics capsule follow the corresponding capsule on the bat
        capsuleToFollow = followTarget;

    }
}
