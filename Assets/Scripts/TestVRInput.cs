using UnityEngine;
using UnityEngine.SceneManagement;

public class TestVRInput : MonoBehaviour
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
        if (OVRInput.Get(OVRInput.Button.One))
        {
            if (!manager.gameInPlay)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            manager.RestartMatch();
        }
    }
}
