using System;
using UnityEngine;

public class BallPongPhysics : MonoBehaviour
{
    public float ballZSpeed = 30;
    public float ballXSpeed = 10;
    public float ballYSpeed = 10;
    public float ballXDir = 0;
    public float ballYDir = 0;

    [SerializeField] private float collisionDetectionRange;
    
    //can only be forward or backward
    public float ballZDir = -1;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollisionDirection();
        UpdateBallPosition();
    }

    void UpdateBallPosition()
    {
        Vector3 ballVelocity = new Vector3(ballXDir * ballXSpeed, ballYDir * ballYSpeed, ballZDir * ballZSpeed) *
                               Time.deltaTime;

        transform.position += ballVelocity;
    }

    private void CheckCollisionDirection()
    {
       
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit,  collisionDetectionRange))
        {
            print("HIT BACK WALL");
            print(hit.transform.name);
          ballZDir = FlipValue(ballZDir);

        }
        else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), collisionDetectionRange))
        {
            print("HIT RIGHT WALL");
            ballXDir = FlipValue(ballXDir);

        }
        //Check for left wall by flipping direction
        else if (Physics.Raycast(transform.position, -transform.TransformDirection(Vector3.right), collisionDetectionRange))
        {
            print("HIT LEFT WALL");
            ballXDir = FlipValue(ballXDir);

        }
       else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), collisionDetectionRange))
        {
            print("HIT ROOF WALL");
            ballYDir = FlipValue(ballYDir);

        }
       else if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), collisionDetectionRange))
        {
            print("HIT FLOOR WALL");
            ballYDir = FlipValue(ballYDir);

        }
    }

    float FlipValue(float value)
    {
        if (value < -1)
        {
            value = value * 1;
        }
        else
        {
            value = value * -1;
        }

        return value;
    }
}
