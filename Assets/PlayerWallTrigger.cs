using System;
using UnityEngine;

public class PlayerWallTrigger : MonoBehaviour
{
    private GameManager manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = FindAnyObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Ball"))
        {
            manager.turns -= 1;
            manager.RestartMatch();
            Destroy(other.gameObject);
        }
    }
}

