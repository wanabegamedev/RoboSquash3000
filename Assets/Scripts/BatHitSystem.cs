using System;
using UnityEngine;

public class BatHitSystem : MonoBehaviour
{

    [SerializeField] private float batHitForce = 20;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {
        transform.position =   OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand);
        transform.rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand);
        
        //print("Z: " + transform.position.z);
        //print("Y: " + transform.position.y);
        //print("X: " + transform.position.x);
    }
    



    private void OnTriggerEnter(Collider other)
    {
        print("HIT");
        // if (other.transform.TryGetComponent(out BallPongPhysics ball))
        // {
        //     ball.ballZDir = 1;
        //     ball.ballXDir = GetFullNumber(transform.localPosition.x);
        //     ball.ballYDir = GetFullNumber(transform.localPosition.y);
        // }
        
        other.GetComponent<Rigidbody>().AddForce(transform.InverseTransformDirection(Vector3.forward * batHitForce));

    }

    float  GetFullNumber(float value)
    {
        if (value < 0)
        {
            value = -1;
        }
        else
        {
            value = 1;
        }
        
        return value;
    }
    
    
    
}
