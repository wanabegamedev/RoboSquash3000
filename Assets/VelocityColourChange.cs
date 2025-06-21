using UnityEngine;

public class VelocityColourChange : MonoBehaviour
{

    [SerializeField] private float maximumVelocity = 20f;

    private Renderer materialRenderer;

    private Rigidbody rigid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        materialRenderer = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        materialRenderer.material.color = ColorForVelocity();
    }

    private Color ColorForVelocity()
    {
        float velocity = rigid.linearVelocity.magnitude;
        return Color.Lerp(Color.green, Color.red, velocity / maximumVelocity);
    }
}
