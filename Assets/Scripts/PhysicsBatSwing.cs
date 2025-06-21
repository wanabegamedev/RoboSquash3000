
using UnityEngine;

public class PhysicsBatSwing : MonoBehaviour
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
        
        //print("Z: " + transform.position.z);
        //print("Y: " + transform.position.y);
        //print("X: " + transform.position.x);
    }
}
