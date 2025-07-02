using System;
using UnityEngine;

public class TestBallBehavior : MonoBehaviour
{
    [SerializeField] private float force;

    private Rigidbody rigid;

    private GameManager manager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
       
        manager = FindAnyObjectByType<GameManager>();
        
        rigid.AddForce(-Vector3.forward * 175);
    }

    // Update is called once per frame
    void Update()
    {
        rigid.AddForce(-Vector3.forward * force);
    }

    private void OnCollisionEnter(Collision other)
    {
        //var localVelocity = transform.InverseTransformDirection(rigid.linearVelocity);
       // print(localVelocity);
       
        
       // rigid.AddForce(transform.InverseTransformDirection(-rigid.linearVelocity));

       if (other.transform.CompareTag("Bat"))
       {
           manager.IncreaseMultiplier(1);
       }
       else
       {
           manager.IncreaseScore(10);
           manager.UpdateScore();
       }
       
    }
}
