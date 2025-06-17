using UnityEngine;

public class TestBallBehavior : MonoBehaviour
{
    [SerializeField] private float force;

    private Rigidbody rigid;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.AddForce(-Vector3.forward * force);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
