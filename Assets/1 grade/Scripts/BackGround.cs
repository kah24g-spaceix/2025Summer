using UnityEngine;

public class BackGround : MonoBehaviour
{
    public Transform plyaer;
    public float parallaxFactor = 0.3f;

    private Vector3 initialPosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = plyaer.position * parallaxFactor;
        transform.position = new Vector3(initialPosition.x + delta.x, initialPosition.y + delta.y, transform.position.z); 
    }
}
