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
        
    }

    private void OnCollisionEnter(Collision other)
    {
        other.transform.GetComponent<Rigidbody>().AddForce(Vector3.forward* 20, ForceMode.Impulse );
    }
}
