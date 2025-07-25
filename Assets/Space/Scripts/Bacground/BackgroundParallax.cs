using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    private float length, startpos;
    private Camera cam;
    public float parallexEffect;
    void Start()
    {
        startpos = transform.position.x;
        cam = FindFirstObjectByType<Camera>();
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    void FixedUpdate()
    {
        float temp = cam.transform.position.x * (1 - parallexEffect);
        float dist = cam.transform.position.x * parallexEffect;
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);
        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }
}