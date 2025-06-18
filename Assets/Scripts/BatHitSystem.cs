using System;
using UnityEngine;

public class BatHitSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {
        transform.position =   OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand);
        transform.rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand);
    }

    private void OnCollisionEnter(Collision other)
    {
        other.transform.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.right) * 20, ForceMode.Impulse );
    }
    
    
    
}
